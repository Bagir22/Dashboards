using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FacultyDistribution
{
    public record GetFacultyDistributionQuery( FilterParams filterParams ): IRequest<List<FacultyDistributionResponseDto>>;

    public class GetFacultyDistributionQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetFacultyDistributionQuery, List<FacultyDistributionResponseDto>>
    {
        private const string BudgetFormName = "Бюджет";
        private const string PaidFormName = "Коммерция";

        public async Task<List<FacultyDistributionResponseDto>> Handle( GetFacultyDistributionQuery distributionQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.Faculty != null )
                .Filter<Student>( distributionQuery.filterParams )
                .GroupBy( s => s.Faculty.Name )
                .Select( g => new FacultyDistributionResponseDto
                {
                    Faculty = g.Key,
                    Total = g.Count(),
                    Budget = g.Count( s => s.Budget == BudgetFormName ),
                    Paid = g.Count( s => s.Budget == PaidFormName )
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
