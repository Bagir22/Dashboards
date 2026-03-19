using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllAcademicStatesQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllAcademicStatesQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllAcademicStatesQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllAcademicStatesQuery getAllAcademicStatesQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.StudentAcademicStates
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
