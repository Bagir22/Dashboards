using Domain.Entities;

namespace Application.Filters
{
    internal static class QueryableStudentFilterExtension
    {
        public static IQueryable<Student> Filter<T>( this IQueryable<Student> query, FilterParams filterParams )
        {
            return query
            .ApplyIf( filterParams.Year.HasValue && filterParams.Month.HasValue, q => q.Where( s => s.ContingentDate == new DateTime( filterParams.Year.Value, filterParams.Month.Value, 1 ).ToUniversalTime() ) )
            .ApplyIf( filterParams.StartDate.HasValue, q => q.Where( s => s.ContingentDate >= filterParams.StartDate.Value.ToUniversalTime() ) )
            .ApplyIf( filterParams.EndDate.HasValue, q => q.Where( s => s.ContingentDate <= filterParams.EndDate.Value.ToUniversalTime() ) )
            .ApplyIf( filterParams.BenefitId.HasValue, q => q.Where( s => s.BenefitId == filterParams.BenefitId ) )
            .ApplyIf( filterParams.EducationStandartId.HasValue, q => q.Where( s => s.EducationStandardId == filterParams.EducationStandartId ) )
            .ApplyIf( filterParams.EducationProgrammId.HasValue, q => q.Where( s => s.EducationProgramId == filterParams.EducationProgrammId ) )
            .ApplyIf( filterParams.CitizenshipId.HasValue, q => q.Where( s => s.CitizenshipId == filterParams.CitizenshipId ) )
            .ApplyIf( filterParams.OrganizationId.HasValue, q => q.Where( s => s.OrganizationId == filterParams.OrganizationId ) )
            .ApplyIf( filterParams.FacultyId.HasValue, q => q.Where( s => s.FacultyId == filterParams.FacultyId ) )
            .ApplyIf( filterParams.StudyFormId.HasValue, q => q.Where( s => s.StudyFormId == filterParams.StudyFormId ) )
            .ApplyIf( filterParams.Gender.HasValue, q => q.Where( s => s.Gender == filterParams.Gender ) )
            .ApplyIf( filterParams.AcademicStateId.HasValue, q => q.Where( s => s.AcademicStateId == filterParams.AcademicStateId ) )
            .ApplyIf( filterParams.CourseNum.HasValue, q => q.Where( s => s.Course == filterParams.CourseNum ) )
            .ApplyIf( filterParams.isForeignStudents.HasValue && filterParams.isForeignStudents.Value, q => q.Where( s => s.Citizenship.Name != "Российская Федерация" ) )
            .ApplyIf( filterParams.isForeignStudents.HasValue && !filterParams.isForeignStudents.Value, q => q.Where( s => s.Citizenship.Name == "Российская Федерация" ) );
        }

        private static IQueryable<T> ApplyIf<T>( this IQueryable<T> query, bool condition,
            Func<IQueryable<T>, IQueryable<T>> action )
        {
            return condition ? action( query ) : query;
        }
    }
}
