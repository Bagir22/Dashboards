using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.GenderDistribution
{
    public record GetGenderDistributionQuery( FilterParams filterParams ): IRequest<List<GenderDistributionResponseDto>>;

    public class GetGenderDistributionQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetGenderDistributionQuery, List<GenderDistributionResponseDto>>
    {
        public async Task<List<GenderDistributionResponseDto>> Handle( GetGenderDistributionQuery genderDistributionQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Filter<Student>( genderDistributionQuery.filterParams )
                .GroupBy( s => s.Gender == 1 ? "М" : "Ж" )
                .Select( g => new GenderDistributionResponseDto
                {
                    Gender = g.Key,
                    Count = g.Count()
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
