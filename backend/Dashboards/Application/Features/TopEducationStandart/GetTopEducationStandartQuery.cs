using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TopEducationStandart
{
    public record GetTopEducationStandartQuery( FilterParams filterParams ): IRequest<List<TopEducationStandartResponseDto>>;

    public class GetTopEducationStandartQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetTopEducationStandartQuery, List<TopEducationStandartResponseDto>>
    {
        public async Task<List<TopEducationStandartResponseDto>> Handle( GetTopEducationStandartQuery educationStandartQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.EducationStandard != null )
                .Filter<Student>( educationStandartQuery.filterParams )
                .GroupBy( s => s.EducationStandard.Name )
                .OrderByDescending( g => g.Count() )
                .Take( 15 )
                .Select( g => new TopEducationStandartResponseDto
                {
                    Name = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
