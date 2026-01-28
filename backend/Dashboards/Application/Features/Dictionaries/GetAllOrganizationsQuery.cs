using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllOrganizationsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllOrganizationsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllOrganizationsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllOrganizationsQuery getAllOrganizationsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Organizations
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
