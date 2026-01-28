using Application.Features.AverageBallByInstitutes;
using Application.Features.Disability;
using Application.Features.FacultyDistribution;
using Application.Features.GenderDistribution;
using Application.Features.StudentDynamic;
using Application.Features.StudyFormCourseStructure;
using Application.Features.TopCitizenship;
using Application.Features.TopEducationProgram;
using Application.Features.TopEducationStandart;
using Application.Features.TopOrganization;
using Application.Features.TopState;
using Application.Filters;
using AutoMapper;

namespace Application.Mappings
{
    public class FilterProfile: Profile
    {
        public FilterProfile()
        {
            CreateMap<FacultyDistributionRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.FacultyId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<GenderDistributionRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.Gender, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<StudentDynamicsRequestDto, FilterParams>()
                .ForMember( dest => dest.Year, opt => opt.Ignore() )
                .ForMember( dest => dest.Month, opt => opt.Ignore() )
                .ForMember( dest => dest.AcademicStateId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<StudyFormCourseRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.StudyFormId, opt => opt.Ignore() )
                .ForMember( dest => dest.CourseNum, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<TopCitizenshipRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.CitizenshipId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<TopEducationProgramRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EducationProgrammId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );


            CreateMap<TopEducationStandartRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EducationStandartId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );


            CreateMap<TopOrganizationRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.OrganizationId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<TopStateRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<AverageBallByInstitutesRequestDto, FilterParams>()
                .ForMember( dest => dest.Year, opt => opt.Ignore() )
                .ForMember( dest => dest.Month, opt => opt.Ignore() )
                .ForMember( dest => dest.FacultyId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );

            CreateMap<DisabilityRequestDto, FilterParams>()
                .ForMember( dest => dest.StartDate, opt => opt.Ignore() )
                .ForMember( dest => dest.EndDate, opt => opt.Ignore() )
                .ForMember( dest => dest.BenefitId, opt => opt.Ignore() )
                .ForMember( dest => dest.isForeignStudents, opt => opt.Ignore() );
        }
    }
}
