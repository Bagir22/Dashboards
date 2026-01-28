using Application.Contracts;
using Application.Filters;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StudentDynamic
{
    public record GetStudentDynamicsQuery( FilterParams filterParams ): IRequest<List<StudentDynamicsResponseDto>>;

    public class GetStudentDynamicsQueryHandler( IUniDashDbContext uniDashDbContext ): IRequestHandler<GetStudentDynamicsQuery, List<StudentDynamicsResponseDto>>
    {
        private const string StudyingState = "Учится";
        private const string TransferredState = "Переведен";
        private const string AwaitingState = "Ожидает зачисления";
        private const string OnLeaveState = "Академический отпуск";
        private const string ExpelledState = "Отчислен";

        public async Task<List<StudentDynamicsResponseDto>> Handle( GetStudentDynamicsQuery studentDynamicsQuery, CancellationToken cancellationToken )
        {
            var result = await uniDashDbContext.Students
                .AsNoTracking()
                .Filter<Student>( studentDynamicsQuery.filterParams )
                .GroupBy( s => new { s.ContingentDate.Year, s.ContingentDate.Month } )
                .Select( g => new StudentDynamicsResponseDto
                {
                    Date = new DateTime( g.Key.Year, g.Key.Month, 1 ),
                    Studying = g.Count( s => s.AcademicState.Name == StudyingState || s.AcademicState.Name == TransferredState ),
                    Awaiting = g.Count( s => s.AcademicState.Name == AwaitingState ),
                    OnLeave = g.Count( s => s.AcademicState.Name == OnLeaveState ),
                    Expelled = g.Count( s => s.AcademicState.Name == ExpelledState )
                } )
                .OrderBy( x => x.Date )
                .ToListAsync( cancellationToken );

            return result;
        }
    }
}
