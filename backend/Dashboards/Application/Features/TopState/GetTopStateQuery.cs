using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TopState
{
    public record GetTopStateQuery( FilterParams filterParams ): IRequest<List<TopStateResponseDto>>;

    public class GetTopStateQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetTopStateQuery, List<TopStateResponseDto>>
    {
        public async Task<List<TopStateResponseDto>> Handle( GetTopStateQuery topStateQuery, CancellationToken cancellationToken )
        {
            topStateQuery.filterParams.isForeignStudents = false;

            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.AddressState != null )
                .Filter<Student>( topStateQuery.filterParams )
                .GroupBy( s => s.AddressState.Name )
                .OrderByDescending( g => g.Count() )
                .Take( 10 )
                .Select( g => new TopStateResponseDto
                {
                    Name = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
