using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllEducationProgramsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllEducationProgramsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllEducationProgramsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllEducationProgramsQuery getAllEducationProgramsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.EducationPrograms
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
