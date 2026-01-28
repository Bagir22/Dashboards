using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StudyFormCourseStructure
{
    public record GetStudyFormCourseStructureQuery( FilterParams filterParams ): IRequest<List<StudyFormCourseResponseDto>>;

    public class GetStudyFormCourseStructureQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetStudyFormCourseStructureQuery, List<StudyFormCourseResponseDto>>
    {
        public async Task<List<StudyFormCourseResponseDto>> Handle( GetStudyFormCourseStructureQuery studyFormCourseStructureQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.StudyForm != null && s.Course.HasValue )
                .Filter<Student>( studyFormCourseStructureQuery.filterParams )
                .GroupBy( s => new { s.StudyForm.Name, Course = s.Course ?? 0 } )
                .Select( g => new StudyFormCourseResponseDto
                {
                    StudyForm = g.Key.Name,
                    Course = g.Key.Course,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
