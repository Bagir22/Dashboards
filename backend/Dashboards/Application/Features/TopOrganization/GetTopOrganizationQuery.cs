using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TopOrganization
{
    public record GetTopOrganizationQuery( FilterParams filterParams ): IRequest<List<TopOrganizationResponseDto>>;

    public class GetTopOrganizationQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetTopOrganizationQuery, List<TopOrganizationResponseDto>>
    {
        public async Task<List<TopOrganizationResponseDto>> Handle( GetTopOrganizationQuery organizationQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.Organization != null )
                .Filter<Student>( organizationQuery.filterParams )
                .GroupBy( s => s.Organization.Name )
                .OrderByDescending( g => g.Count() )
                .Take( 10 )
                .Select( g => new TopOrganizationResponseDto
                {
                    Name = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
