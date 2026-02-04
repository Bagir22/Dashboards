using Application.Features.Dictionaries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class DictController( IMediator mediator ): ControllerBase
    {
        /// <summary>
        /// Получает список всех форм обучения
        /// </summary>
        [HttpGet("study-forms")]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllStudyForms()
        {
            var query = new GetAllStudyFormsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех академических статусов студентов
        /// </summary>
        [HttpGet( "academic-states" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllAcademicStates()
        {
            var query = new GetAllAcademicStatesQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех льгот
        /// </summary>
        [HttpGet( "benefits" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllBenefits()
        {
            var query = new GetAllBenefitsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех гражданств
        /// </summary>
        [HttpGet( "citizenships" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllCitizenships()
        {
            var query = new GetAllCitizenshipsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех профилей подготовки
        /// </summary>
        [HttpGet( "education-programs" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllEducationPrograms()
        {
            var query = new GetAllEducationProgramsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех направлений подготовки
        /// </summary>
        [HttpGet( "education-standards" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllEducationStandards()
        {
            var query = new GetAllEducationStandardsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех факультетов/институтов
        /// </summary>
        [HttpGet( "faculties" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllFaculties()
        {
            var query = new GetAllFacultiesQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }

        /// <summary>
        /// Получает список всех заказчиков целевого обучения
        /// </summary>
        [HttpGet( "organizations" )]
        public async Task<ActionResult<List<DictionaryItemDto>>> GetAllOrganizations()
        {
            var query = new GetAllOrganizationsQuery();
            var result = await mediator.Send( query );

            return Ok( result );
        }
    }
}
