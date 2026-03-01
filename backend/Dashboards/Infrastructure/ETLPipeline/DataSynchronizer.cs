using Application.Contracts;
using Domain.Entities;
using Infrastructure.ETLPipeline.Extract.Achivment;
using Infrastructure.ETLPipeline.Extract.AchivmentCategory;
using Infrastructure.ETLPipeline.Extract.ApiAuth;
using Infrastructure.ETLPipeline.Extract.Benifit;
using Infrastructure.ETLPipeline.Extract.Branch;
using Infrastructure.ETLPipeline.Extract.Citizenship;
using Infrastructure.ETLPipeline.Extract.Discipline;
using Infrastructure.ETLPipeline.Extract.EducationProgram;
using Infrastructure.ETLPipeline.Extract.EducationStandard;
using Infrastructure.ETLPipeline.Extract.Faculty;
using Infrastructure.ETLPipeline.Extract.Mark;
using Infrastructure.ETLPipeline.Extract.Order;
using Infrastructure.ETLPipeline.Extract.OrderCategory;
using Infrastructure.ETLPipeline.Extract.Organization;
using Infrastructure.ETLPipeline.Extract.Student;
using Infrastructure.ETLPipeline.Extract.StudentAcademicState;
using Infrastructure.ETLPipeline.Extract.StudyForm;
using Infrastructure.ETLPipeline.Extract.TrainingLevel;
using Infrastructure.ETLPipeline.Synchronize.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ETLPipeline
{
    internal class DataSynchronizer(
        IUniDashDbContext dbContext,
        IMetabaseService metabaseService,
        IApiAuthRequest apiAuthRequest,
        ICitizenshipRequest citizenshipRequest,
        IFacultyRequest facultyRequest,
        IStudyFormRequest studyFormRequest,
        IStudentAcademicStateRequest studentAcademicStateRequest,
        IStudentRequest studentRequest,
        IEducationProgramRequest educationProgramRequest,
        IEducationStandardRequest educationStandardRequest,
        IBenefitRequest benefitRequest,
        IBranchRequest branchRequest,
        ITrainingLevel trainingLevelRequest,
        IDiscipline disciplineRequest,
        IMark markRequest,
        IOrganizationRequest organizationRequest,
        IAchivmentCategoryRequest achivmentCategoryRequest,
        IAchivmentRequest achivmentRequest,
        IOrderCategoryRequest orderCategoryRequest,
        IOrderRequest orderRequest,
        IMemoryCache memoryCache,
        ILogger<DataSynchronizer> logger ): IDataSynchronizer
    {
        private static readonly SemaphoreSlim _lock = new( 1, 1 );
        private enum CacheKeys
        {
            Citizenships,
            Faculties,
            StudyForms,
            EducationPrograms,
            EducationStandards,
            Benefits,
            Organizations,
            AddressStates,
            AchivmentCategories,
            OrderCategories,
            Branches,
            TrainingLevels,
            Disciplines,
            Mark
        }

        private readonly Dictionary<string, int> _genders = new() {
            { "Мужской", 1 }, { "Женский", 0 }, { "Небинарный", -1}
        };

        public async Task InitialCreate()
        {
            var token = await apiAuthRequest.GetTokenAsync();

            await SynchronizeReferenceDataAsync( token );

            var dates = DateUtils.GetMonthlyDatesFrom2023();

            foreach ( var date in dates )
            {
                await AddStudentsLoopAsync( token, date );
            }
        }

        public async Task UpdateData()
        {
            var token = await apiAuthRequest.GetTokenAsync();

            await SynchronizeReferenceDataAsync( token );

            var date = DateUtils.GetCurrentDate;
            await AddStudentsLoopAsync( token, date );
        }

        private async Task SynchronizeReferenceDataAsync( string token )
        {
            await SynchronizeFacultiesAsync( token );
            await SynchronizeCitizenshipsAsync( token );
            await SynchronizeStudyFormsAsync( token );
            await SynchronizeStudentAcademicStatesAsync( token );
            await SynchronizeEducationProgramsAsync( token );
            await SynchronizeEducationStandardsAsync( token );
            await SynchronizeBenefitsAsync( token );
            await SynchronizeOrganizationsAsync( token );
            await SynchronizeAchivmentCategoriesAsync( token );
            await SynchronizeOrderCategoriesAsync( token );
            await SynchronizeBranchesAsync( token );
            await SynchronizeTrainingLevelsAsync( token );
            await SynchronizeDisciplinesAsync( token );
            await SynchronizeMarkAsync( token );
        }

        private async Task AddStudentsLoopAsync( string token, DateTime date )
        {
            for ( int pageNumber = 1; pageNumber <= int.MaxValue; pageNumber++ )
            {
                bool studentsExist = await TryAddStudentsAsync( token, date, pageNumber );
                if ( !studentsExist )
                {
                    break;
                }
            }
        }

        private Dictionary<string, Guid> GetCachedDictionary( CacheKeys cacheKey )
        {
            return memoryCache.Get<Dictionary<string, Guid>>( cacheKey ) ?? new();
        }

        /// <summary>
        /// Add students to db. Returns true if list is not empty.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="dateTime"></param>
        /// <param name="pagesCount"></param>
        /// <returns></returns>
        private async Task<bool> TryAddStudentsAsync( string token, DateTime dateTime, int pagesCount )
        {
            var externalStudents = await studentRequest.GetAllStudentsByDateAsync( token, dateTime, pagesCount );

            if ( externalStudents.Items.Count == 0 )
                return false;

            var cache = new
            {
                Citizenships = GetCachedDictionary( CacheKeys.Citizenships ),
                StudyForms = GetCachedDictionary( CacheKeys.StudyForms ),
                Faculties = GetCachedDictionary( CacheKeys.Faculties ),
                EducationPrograms = GetCachedDictionary( CacheKeys.EducationPrograms ),
                EducationStandards = GetCachedDictionary( CacheKeys.EducationStandards ),
                Benefits = GetCachedDictionary( CacheKeys.Benefits ),
                Organizations = GetCachedDictionary( CacheKeys.Organizations ),
                AddressStates = GetCachedDictionary( CacheKeys.AddressStates ),
                TrainingLevels = GetCachedDictionary( CacheKeys.TrainingLevels ),
            };

            await SynchronizeAddressStatesAsync( externalStudents.Items );

            var existingContingentIds = await dbContext.ContingentStudents
                .Where(s => s.ContingentDate == dateTime.ToUniversalTime())
                .Select(s => s.StudentExternalId)
                .ToHashSetAsync();

            var newContingentStudents = externalStudents.Items
                .Where(f => !existingContingentIds.Contains(Guid.Parse(f.StudentExternalId)))
                .ToList();

            if (!newContingentStudents.Any())
                return true;

            var existingIds = await dbContext.Students
                .Select(s => s.Id)
                .ToHashSetAsync();

            var newStudents = newContingentStudents
                .Where(cs => !existingIds.Contains(Guid.Parse(cs.StudentExternalId)))
                .ToList();

            List<Student> students = new();

            if (newStudents.Any())
            {
                students = newStudents.Select(s => new Student
                {
                    Id = Guid.Parse(s.StudentExternalId),
                    Gender = _genders[s.IsMaleName],
                    Fio = s.FullName,
                    AdmissionYear = s.YearStart,
                    CitizenshipId = cache.Citizenships[s.Citizenship],
                    StudyFormId = cache.StudyForms.GetNullableValue(s.StudyForm),
                    FacultyId = cache.Faculties.GetNullableValue(s.FacultyName),
                    EducationProgramId = cache.EducationPrograms.GetNullableValue(s.EducationProgramName),
                    EducationStandardId = cache.EducationStandards.GetNullableValue(s.EducationStandard),
                    BenefitId = cache.Benefits.GetNullableValue(s.Benefit),
                    OrganizationId = cache.Organizations.GetNullableValue(s.TargetOrganizationName),
                    TrainingLevelId = cache.TrainingLevels.GetNullableValue(s.TrainingLevel),
                }).ToList();
            }

            var contingentStudents = newContingentStudents.Select(s => new ContingentStudent
            {
                Id = Guid.NewGuid(),
                StudentExternalId = Guid.Parse(s.StudentExternalId),
                ContingentDate = dateTime.ToUniversalTime(),
                AcademicStateId = Guid.Parse(s.AcademicStateId),
                Course = s.CourseNum,
                AddressStateId = cache.AddressStates.GetNullableValue(s.AddressState),
                Ball = s.Ball,
                Budget = s.StudentBudgetName
            }).ToList();

            await _lock.WaitAsync();
            try
            {
                if(students.Any())
                    await dbContext.Students.AddRangeAsync(students);
                await dbContext.ContingentStudents.AddRangeAsync( contingentStudents );
                await dbContext.SaveChangesAsync();

                var allStudentIds = await dbContext.Students
                    .Select(s => s.Id)
                    .ToListAsync();

                await SynchronizeOrdersAndAchivmentsAsync(token, allStudentIds);

                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task SynchronizeAddressStatesAsync( List<StudentResponse> students )
        {
            var uniqueStateNames = students
                .Select( s => s.AddressState?.Trim() )
                .Where( name => !string.IsNullOrEmpty( name ) )
                .Distinct()
                .ToList();

            if ( uniqueStateNames.Count == 0 )
                return;

            var existingStates = await dbContext.AddressStates
                .Where( state => uniqueStateNames.Contains( state.Name ) )
                .ToListAsync();

            var existingStateNames = existingStates.Select( s => s.Name ).ToHashSet();

            var newStates = uniqueStateNames
                .Where( name => !existingStateNames.Contains( name ) )
                .Select( name => new AddressState
                {
                    Id = Guid.NewGuid(),
                    Name = name
                } )
                .ToList();

            if ( newStates.Any() )
            {
                await dbContext.AddressStates.AddRangeAsync( newStates );
                await dbContext.SaveChangesAsync();
            }

            memoryCache.Remove( CacheKeys.AddressStates );

            Dictionary<string, Guid> parsed = existingStates
                .Concat( newStates )
                .ToDictionary( sh => sh.Name, sh => sh.Id );

            memoryCache.Set( CacheKeys.AddressStates, parsed );
        }

        private async Task SynchronizeAchivmentCategoriesAsync(string token)
        {
            var externalCategories = await achivmentCategoryRequest.GetAllAchivmentCategoriesAsync(token);
            var existingCategories = await dbContext.AchivmentCategories.ToListAsync();

            var parsedCategories = externalCategories
                .Select(f => new AchivmentCategory
                {
                    Id = Guid.Parse(f.Id),
                    Name = f.Name
                }).ToList();

            var existingIds = existingCategories.Select(f => f.Id).ToHashSet();
            var externalIds = parsedCategories.Select(f => f.Id).ToHashSet();

            var newCategories = parsedCategories
                .Where(f => !existingIds.Contains(f.Id))
                .ToList();

            if (newCategories.Any())
                await dbContext.AchivmentCategories.AddRangeAsync(newCategories);

            var toDelete = existingCategories
                .Where(f => !externalIds.Contains(f.Id))
                .ToList();

            if (toDelete.Any())
                dbContext.AchivmentCategories.RemoveRange(toDelete);

            memoryCache.Remove(CacheKeys.AchivmentCategories);

            Dictionary<string, Guid> parsed = new Dictionary<string, Guid>();
            parsedCategories.ForEach(c => parsed.TryAdd(c.Name, c.Id));
            memoryCache.Set(CacheKeys.AchivmentCategories, parsed);

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeOrderCategoriesAsync(string token)
        {
            var externalCategories = await orderCategoryRequest.GetAllOrderCategoriesAsync(token);
            var existingCategories = await dbContext.OrderCategories.ToListAsync();

            var parsedCategories = externalCategories
                .Select(f => new OrderCategory
                {
                    Id = Guid.Parse(f.Id),
                    Name = f.Name
                }).ToList();

            var existingIds = existingCategories.Select(f => f.Id).ToHashSet();
            var externalIds = parsedCategories.Select(f => f.Id).ToHashSet();

            var newCategories = parsedCategories
                .Where(f => !existingIds.Contains(f.Id))
                .ToList();

            if (newCategories.Any())
                await dbContext.OrderCategories.AddRangeAsync(newCategories);

            var toDelete = existingCategories
                .Where(f => !externalIds.Contains(f.Id))
                .ToList();

            if (toDelete.Any())
                dbContext.OrderCategories.RemoveRange(toDelete);

            memoryCache.Remove(CacheKeys.OrderCategories);

            Dictionary<string, Guid> parsed = new Dictionary<string, Guid>();
            parsedCategories.ForEach(c => parsed.TryAdd(c.Name, c.Id));
            memoryCache.Set(CacheKeys.OrderCategories, parsed);

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeOrdersAndAchivmentsAsync(string token, List<Guid> studentIds)
        {
            if (studentIds.Count == 0)
                return;

            var orderCategoryCache = GetCachedDictionary(CacheKeys.OrderCategories);
            var achivmentCategoryCache = GetCachedDictionary(CacheKeys.AchivmentCategories);

            var existingOrders = await dbContext.Orders
                .Where(o => studentIds.Contains(o.StudentId))
                .Select(o => o.Id)
                .ToHashSetAsync();

            var existingAchivments = await dbContext.Achivments
                .Where(a => studentIds.Contains(a.StudentId))
                .Select(a => a.Id)
                .ToHashSetAsync();

            List<Order> newOrders = new();
            List<Achivment> newAchivments = new();

            foreach (var studentId in studentIds)
            {
                await ProcessStudentOrdersAsync(
                    token,
                    studentId,
                    orderCategoryCache,
                    existingOrders,
                    newOrders);

                await ProcessStudentAchivmentsAsync(
                    token,
                    studentId,
                    achivmentCategoryCache,
                    existingAchivments,
                    newAchivments);
            }

            await SaveNewEntitiesAsync(newOrders, newAchivments);
        }

        private async Task ProcessStudentOrdersAsync(
            string token,
            Guid studentId,
            Dictionary<string, Guid> categoryCache,
            HashSet<Guid> existingOrders,
            List<Order> newOrders)
        {
            List<OrderResponse> externalOrders;

            externalOrders = await orderRequest.GetAllOrdersAsync(token, studentId.ToString());

            if (externalOrders == null || externalOrders.Count == 0)
                return;

            foreach (var order in externalOrders)
            {
                if (order.DictOrdersCategories.DictOrderType == null ||
                   !order.DictOrdersCategories.DictOrderType.Enums.Contains(4))
                    continue;

                var orderId = Guid.Parse(order.Id);

                if (existingOrders.Contains(orderId))
                    continue;

                var categoryId = categoryCache.GetNullableValue(order.DictOrdersCategories.Category);

                if (categoryId == null)
                {
                    logger.LogWarning(
                        $"Order category not found in cache: {order.DictOrdersCategories.Category}");
                    continue;
                }
                    

                newOrders.Add(CreateOrder(orderId, order, studentId, (Guid)categoryId));
            }
        }

        private async Task ProcessStudentAchivmentsAsync(
            string token,
            Guid studentId,
            Dictionary<string, Guid> achivmentCategoryCache,
            HashSet<Guid> existingAchivments,
            List<Achivment> newAchivments)
        {
            foreach (var category in achivmentCategoryCache)
            {
                List<AchivmentResponse> externalAchivments;

                externalAchivments = await achivmentRequest
                        .GetAllAchivmentsAsync(token, studentId.ToString(), category.Value.ToString());

                if (externalAchivments == null || externalAchivments.Count == 0)
                    continue;

                foreach (var ach in externalAchivments)
                {
                    if (ach.Status.StatusIn.Value != 1)
                        continue;

                    var achId = Guid.Parse(ach.Id);

                    if (existingAchivments.Contains(achId))
                        continue;

                    newAchivments.Add(CreateAchivment(achId, ach, studentId, category.Value));
                }
            }
        }

        private static Order CreateOrder(Guid orderId, OrderResponse order, Guid studentId, Guid categoryId)
        {
            return new Order
            {
                Id = orderId,
                Date = order.Date.ToUniversalTime(),
                StudentId = studentId,
                CategoryId = categoryId
            };
        }

        private static Achivment CreateAchivment(Guid achId, AchivmentResponse ach, Guid studentId, Guid categoryId)
        {
            return new Achivment
            {
                Id = achId,
                BeginDate = ach.Period.Begin.ToUniversalTime(),
                StudentId = studentId,
                CategoryId = categoryId
            };
        }

        private async Task SaveNewEntitiesAsync(List<Order> newOrders, List<Achivment> newAchivments)
        {
            if (newOrders.Any())
                await dbContext.Orders.AddRangeAsync(newOrders);

            if (newAchivments.Any())
                await dbContext.Achivments.AddRangeAsync(newAchivments);

            if (newOrders.Any() || newAchivments.Any())
                await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeFacultiesAsync( string token )
        {
            var externalFaculties = await facultyRequest.GetAllFacultiesAsync( token );
            var existingFaculties = await dbContext.Faculties.ToListAsync();

            var parsedFaculties = externalFaculties
                .Select( f => new Faculty
                {
                    Id = Guid.Parse( f.FacultyExternalId ),
                    Name = f.Name
                } ).ToList();

            var existingIds = existingFaculties.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedFaculties.Select( f => f.Id ).ToHashSet();

            var newFaculties = parsedFaculties
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( newFaculties.Any() )
                await dbContext.Faculties.AddRangeAsync( newFaculties );

            var toDelete = existingFaculties
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Faculties.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Faculties );

            Dictionary<string, Guid> parsed = parsedFaculties.ToDictionary( f => f.Name, f => f.Id );
            memoryCache.Set( CacheKeys.Faculties, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeBranchesAsync( string token )
        {
            var externalBranches = await branchRequest.GetAllBranchesAsync( token );
            var existingBranches = await dbContext.Branches.ToListAsync();

            var parsedBranches = externalBranches
                .Select( f => new Branch()
                {
                    Id = Guid.Parse( f.BranchExternalId ),
                    Name = f.Name
                } ).ToList();

            var existingIds = existingBranches.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedBranches.Select( f => f.Id ).ToHashSet();

            var newBranches = parsedBranches
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( newBranches.Any() )
                await dbContext.Branches.AddRangeAsync( newBranches );

            var toDelete = existingBranches
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Branches.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Branches );

            Dictionary<string, Guid> parsed = parsedBranches.ToDictionary( f => f.Name, f => f.Id );
            memoryCache.Set( CacheKeys.Branches, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeTrainingLevelsAsync( string token )
        {
            var externalTrainingLevels = await trainingLevelRequest.GetAllTrainingLevelsAsync( token );
            var existingTrainingLevels = await dbContext.TrainingLevels.ToListAsync();

            var parsedTrainingLevels = externalTrainingLevels
                .Select( f => new TrainingLevel()
                {
                    Id = Guid.Parse( f.TrainingLevelExternalId),
                    Name = f.Name
                } ).ToList();

            var existingIds = existingTrainingLevels.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedTrainingLevels.Select( f => f.Id ).ToHashSet();

            var newTrainingLevels = parsedTrainingLevels
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( newTrainingLevels.Any() )
                await dbContext.TrainingLevels.AddRangeAsync( newTrainingLevels );

            var toDelete = existingTrainingLevels
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.TrainingLevels.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.TrainingLevels );

            Dictionary<string, Guid> parsed = parsedTrainingLevels.ToDictionary( f => f.Name, f => f.Id );
            memoryCache.Set( CacheKeys.TrainingLevels, parsed );

            await dbContext.SaveChangesAsync();
        }
        
        private async Task SynchronizeDisciplinesAsync( string token )
        {
            var externalDisciplines = await disciplineRequest.GetAllDisciplinesAsync( token );
            var existingDisciplines = await dbContext.Disciplines.AsNoTracking().ToListAsync();

            var parsedDisciplines = externalDisciplines
                .Select( f => new Discipline()
                {
                    Id = Guid.Parse( f.DisciplineId ),
                    Name = f.Name
                } )
                .DistinctBy( d => d.Id )
                .ToList();

            var existingIds = existingDisciplines.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedDisciplines.Select( f => f.Id ).ToHashSet();

            var newDisciplines = parsedDisciplines
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( newDisciplines.Any() )
                await dbContext.Disciplines.AddRangeAsync( newDisciplines );

            var toDelete = existingDisciplines
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Disciplines.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Disciplines );

            Dictionary<string, Guid> parsed = parsedDisciplines
                .GroupBy(f => f.Name)
                .ToDictionary(g => g.Key, g => g.First().Id);

            memoryCache.Set( CacheKeys.Disciplines, parsed );

            await dbContext.SaveChangesAsync();
        }
        
        private async Task SynchronizeMarkAsync( string token )
        {
            var externalMarks = await markRequest.GetAllMarksAsync( token );
            var existingMarks = await dbContext.Marks.ToListAsync();

            var parsedMarks = externalMarks
                .Select( f => new Mark()
                {
                    Id = Guid.Parse( f.MarkId ),
                    Name = f.Name,
                    Value = f.Value,
                    IsGoodMark = f.isGoodMark, 
                } )
                .ToList();

            var existingIds = existingMarks.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedMarks.Select( f => f.Id ).ToHashSet();

            var newMarks = parsedMarks
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( newMarks.Any() )
                await dbContext.Marks.AddRangeAsync( newMarks );

            var toDelete = existingMarks
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Marks.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Mark );

            Dictionary<string, Guid> parsed = parsedMarks.ToDictionary( f => f.Name, f => f.Id );

            memoryCache.Set( CacheKeys.Mark, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeCitizenshipsAsync( string token )
        {
            var externalCitizenships = await citizenshipRequest.GetAllCitizenshipsAsync( token );
            var existingCitizenships = await dbContext.Citizenships.ToListAsync();

            var parsedCitizenships = externalCitizenships
                .Select( cr => new Citizenship
                {
                    Name = cr.Country,
                    Id = Guid.Parse( cr.CitizenshipExternalId )
                } ).ToList();

            var existingIds = existingCitizenships.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedCitizenships.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedCitizenships
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.Citizenships.AddRangeAsync( toAdd );

            var toDelete = existingCitizenships
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Citizenships.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Citizenships );

            Dictionary<string, Guid> parsed = parsedCitizenships.ToDictionary( sh => sh.Name, sh => sh.Id );
            memoryCache.Set( CacheKeys.Citizenships, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeStudyFormsAsync( string token )
        {
            var externalStudyForms = await studyFormRequest.GetAllStudyFormsAsync( token );
            var existingForms = await dbContext.StudyForms.ToListAsync();

            var parsedForms = externalStudyForms
                .Select( f => new StudyForm
                {
                    Id = Guid.Parse( f.DictStudyFormExternalId ),
                    Name = f.StudyFormName
                } ).ToList();

            var existingIds = existingForms.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedForms.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedForms
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.StudyForms.AddRangeAsync( toAdd );

            var toDelete = existingForms
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.StudyForms.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.StudyForms );

            Dictionary<string, Guid> parsed = parsedForms.ToDictionary( sf => sf.Name, sf => sf.Id );
            memoryCache.Set( CacheKeys.StudyForms, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeStudentAcademicStatesAsync( string token )
        {
            var externalStates = await studentAcademicStateRequest.GetAllStudentAcademicStatesAsync( token );
            var existingStates = await dbContext.StudentAcademicStates.ToListAsync();

            var parsedStates = externalStates
                .Select( f => new AcademicState
                {
                    Id = Guid.Parse( f.DictStudentAcademicStateExternalId ),
                    Name = f.AcademicStateName
                } ).ToList();

            var existingIds = existingStates.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedStates.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedStates
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.StudentAcademicStates.AddRangeAsync( toAdd );

            var toDelete = existingStates
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.StudentAcademicStates.RemoveRange( toDelete );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeEducationProgramsAsync( string token )
        {
            var externalPrograms = await educationProgramRequest.GetAllEducationProgramsAsync( token );
            var existingPrograms = await dbContext.EducationPrograms.ToListAsync();

            var distinctExternalPrograms = externalPrograms
                .GroupBy( p => p.Name )
                .Select( g => g.First() )
                .ToList();

            var parsedPrograms = distinctExternalPrograms
                .Select( f => new EducationProgram
                {
                    Id = Guid.Parse( f.EducationProgramId ),
                    Name = f.Name
                } ).ToList();

            var existingIds = existingPrograms.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedPrograms.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedPrograms
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.EducationPrograms.AddRangeAsync( toAdd );

            var toDelete = existingPrograms
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.EducationPrograms.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.EducationPrograms );

            Dictionary<string, Guid> parsed = parsedPrograms.ToDictionary( sf => sf.Name, sf => sf.Id );
            memoryCache.Set( CacheKeys.EducationPrograms, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeEducationStandardsAsync( string token )
        {
            var externalStandards = await educationStandardRequest.GetAllEducationStandardsAsync( token );
            var existingStandards = await dbContext.EducationStandards.ToListAsync();

            var distinctExternalStandards = externalStandards
                .GroupBy( p => p.Name )
                .Select( g => g.First() )
                .ToList();

            var parsedStandards = distinctExternalStandards
                .Select( f => new EducationStandard
                {
                    Id = Guid.Parse( f.EducationStandardExternalId ),
                    Name = f.Name
                } ).ToList();

            var existingIds = existingStandards.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedStandards.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedStandards
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.EducationStandards.AddRangeAsync( toAdd );

            var toDelete = existingStandards
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.EducationStandards.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.EducationStandards );

            Dictionary<string, Guid> parsed = parsedStandards.ToDictionary( sf => sf.Name, sf => sf.Id );
            memoryCache.Set( CacheKeys.EducationStandards, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeBenefitsAsync( string token )
        {
            var externalBenefits = await benefitRequest.GetAllBenefitsAsync( token );
            var existingBenefits = await dbContext.Benefits.ToListAsync();

            var parsedBenefits = externalBenefits
                .Select( f => new Benefit
                {
                    Id = Guid.Parse( f.DictBenefitExternalId ),
                    Name = f.BenefitName
                } ).ToList();

            var existingIds = existingBenefits.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedBenefits.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedBenefits
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.Benefits.AddRangeAsync( toAdd );

            var toDelete = existingBenefits
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Benefits.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Benefits );

            Dictionary<string, Guid> parsed = parsedBenefits.ToDictionary( sf => sf.Name, sf => sf.Id );
            memoryCache.Set( CacheKeys.Benefits, parsed );

            await dbContext.SaveChangesAsync();
        }

        private async Task SynchronizeOrganizationsAsync( string token )
        {
            var externalOrganizations = await organizationRequest.GetAllOrganizationsAsync( token );
            var existingOrganizations = await dbContext.Organizations.ToListAsync();

            var parsedOrganizations = externalOrganizations
                .Select( f => new Organization
                {
                    Id = Guid.Parse( f.DictOrganizationExternalId ),
                    Name = f.OrganizationName
                } ).ToList();

            var existingIds = existingOrganizations.Select( f => f.Id ).ToHashSet();
            var externalIds = parsedOrganizations.Select( f => f.Id ).ToHashSet();

            var toAdd = parsedOrganizations
                .Where( f => !existingIds.Contains( f.Id ) )
                .ToList();

            if ( toAdd.Any() )
                await dbContext.Organizations.AddRangeAsync( toAdd );

            var toDelete = existingOrganizations
                .Where( f => !externalIds.Contains( f.Id ) )
                .ToList();

            if ( toDelete.Any() )
                dbContext.Organizations.RemoveRange( toDelete );

            memoryCache.Remove( CacheKeys.Organizations );

            Dictionary<string, Guid> parsed = parsedOrganizations.ToDictionary( sf => sf.Name, sf => sf.Id );
            memoryCache.Set( CacheKeys.Organizations, parsed );

            await dbContext.SaveChangesAsync();
        }
    }
}
