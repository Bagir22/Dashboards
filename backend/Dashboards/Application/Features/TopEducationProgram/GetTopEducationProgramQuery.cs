using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TopEducationProgram
{
    public record GetTopEducationProgramQuery( FilterParams filterParams ): IRequest<List<TopEducationProgramResponseDto>>;

    public class GetTopEducationProgramQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetTopEducationProgramQuery, List<TopEducationProgramResponseDto>>
    {
        public async Task<List<TopEducationProgramResponseDto>> Handle( GetTopEducationProgramQuery educationProgramQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.EducationProgram != null )
                .Filter<Student>( educationProgramQuery.filterParams )
                .GroupBy( s => s.EducationProgram.Name )
                .OrderByDescending( g => g.Count() )
                .Take( 15 )
                .Select( g => new TopEducationProgramResponseDto
                {
                    Name = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
