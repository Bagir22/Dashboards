using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dictionaries
{
    public record GetAllStudyFormsQuery: IRequest<List<DictionaryItemDto>>;

    public class GetAllStudyFormsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetAllStudyFormsQuery, List<DictionaryItemDto>>
    {
        public async Task<List<DictionaryItemDto>> Handle( GetAllStudyFormsQuery getAllStudyFormsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.StudyForms
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
