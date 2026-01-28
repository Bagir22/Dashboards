using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TopCitizenship
{
    public record GetTopCitizenshipQuery( FilterParams filterParams ): IRequest<List<TopCitizenshipResponseDto>>;

    public class GetTopCitizenshipQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetTopCitizenshipQuery, List<TopCitizenshipResponseDto>>
    {
        public async Task<List<TopCitizenshipResponseDto>> Handle( GetTopCitizenshipQuery topCitizenshipQuery, CancellationToken cancellationToken )
        {
            topCitizenshipQuery.filterParams.isForeignStudents = true;

            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Filter<Student>( topCitizenshipQuery.filterParams )
                .GroupBy( s => s.Citizenship.Name )
                .OrderByDescending( g => g.Count() )
                .Take( 10 )
                .Select( g => new TopCitizenshipResponseDto
                {
                    Country = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
