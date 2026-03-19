using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllEducationStandardsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllEducationStandardsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllEducationStandardsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllEducationStandardsQuery getAllEducationStandardsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.EducationStandards
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
