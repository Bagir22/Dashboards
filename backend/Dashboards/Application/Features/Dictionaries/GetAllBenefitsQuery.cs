using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllBenefitsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllBenefitsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllBenefitsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllBenefitsQuery getAllBenefitsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Benefits
                .AsNoTracking()
                .Select( s => new DictionaryItemDto
                {
                    Id = s.Id,
                    Name = s.Name
                } )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
