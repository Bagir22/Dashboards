using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AverageBallByInstitutes
{
    public record GetAverageBallByInstitutesQuery( FilterParams filterParams ): IRequest<List<AverageBallByInstitutesResponseDto>>;

    public class GetDisabilityQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAverageBallByInstitutesQuery, List<AverageBallByInstitutesResponseDto>>
    {
        public async Task<List<AverageBallByInstitutesResponseDto>> Handle( GetAverageBallByInstitutesQuery averageBallByInstitutesQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.Faculty != null && s.Ball.HasValue )
                .Filter<Student>( averageBallByInstitutesQuery.filterParams )
                .GroupBy( s => new { s.ContingentDate, s.Faculty.Name } )
                .Select( g => new AverageBallByInstitutesResponseDto
                {
                    Date = g.Key.ContingentDate,
                    AverageBall = Math.Round( g.Average( s => s.Ball ?? 0d ), 2 ),
                    Institute = g.Key.Name
                } )
                .OrderBy( x => x.Institute )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
