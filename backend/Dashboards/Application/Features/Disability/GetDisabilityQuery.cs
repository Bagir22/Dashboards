using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Disability
{
    public record GetDisabilityQuery( FilterParams filterParams ): IRequest<List<DisabilityResponseDto>>;

    public class GetDisabilityQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetDisabilityQuery, List<DisabilityResponseDto>>
    {
        private static readonly HashSet<string> DisabilityBenefitNames = new()
        {
            "Инвалид 1 группы",
            "Инвалиды вследствие военной травмы или заболевания, полученных в период прохождения военной службы",
            "Инвалид с детства",
            "Инвалид 2 группы",
            "Инвалид 3 группы",
            "Инвалид"
        };

        public async Task<List<DisabilityResponseDto>> Handle( GetDisabilityQuery disabilityQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Where( s => s.Benefit != null && DisabilityBenefitNames.Contains( s.Benefit.Name ) )
                .Filter<Student>( disabilityQuery.filterParams )
                .GroupBy( s => new { s.Benefit.Name } )
                .Select( g => new DisabilityResponseDto
                {
                    StudentsCount = g.Count(),
                    Disability = g.Key.Name
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
