using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllFacultiesQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllFacultiesQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllFacultiesQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllFacultiesQuery getAllFacultiesQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Faculties
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
