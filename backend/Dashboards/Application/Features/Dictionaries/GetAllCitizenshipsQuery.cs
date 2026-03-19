using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllCitizenshipsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllCitizenshipsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllCitizenshipsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllCitizenshipsQuery getAllCitizenshipsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Citizenships
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
