using Application.Features.AverageBallByInstitutes;
using Application.Features.Disability;
using Application.Features.FacultyDistribution;
using Application.Features.GenderDistribution;
using Application.Features.StudentDynamic;
using Application.Features.StudyFormCourseStructure;
using Application.Features.TopCitizenship;
using Application.Features.TopEducationProgram;
using Application.Features.TopEducationStandart;
using Application.Features.TopOrganization;
using Application.Features.TopState;
using Application.Filters;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class DashController(IMediator mediator, IMapper mapper): ControllerBase
    {
        /// <summary>
        /// (1) Получает динамику изменения количества студентов
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /dynamic?startYear=2020&amp;endYear=2023
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список точек динамики</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("dynamic")]
        public async Task<ActionResult<List<StudentDynamicsResponseDto>>> GetStudentDynamicsDiagram(
            [FromQuery] StudentDynamicsRequestDto filters,
            CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetStudentDynamicsQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (2) Получает структуру студентов по формам обучения и курсам
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /study-form-course-structure?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список структуры студентов</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("study-form-course-structure")]
        public async Task<ActionResult<List<StudyFormCourseResponseDto>>> GetStudyFormCourseStructure(
           [FromQuery] StudyFormCourseRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetStudyFormCourseStructureQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (3) Получает распределение по институтам
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /faculty?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список распределения по институтам</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("faculty")]
        public async Task<ActionResult<List<FacultyDistributionResponseDto>>> GetFacultyDistribution(
            [FromQuery] FacultyDistributionRequestDto filters,
            CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetFacultyDistributionQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (4) Получает число мальчиков и девочек в текущем контингенте или по выбранному фильтру
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /gender?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список распределения по полу</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("gender")]
        public async Task<ActionResult<List<GenderDistributionResponseDto>>> GetGenderDistribution(
            [FromQuery] GenderDistributionRequestDto filters,
            CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetGenderDistributionQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (5) Получает ТОП-10 стран гражданства
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-citizenship?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список ТОП-10 стран гражданства</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("top-citizenship")]
        public async Task<ActionResult<List<TopCitizenshipResponseDto>>> GetTopCitizenship(
           [FromQuery] TopCitizenshipRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetTopCitizenshipQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (6) Получает топ-10 регионов регистрации российских студентов
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-state?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список топ-10 регионов регистрации российских студентов</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("top-state")]
        public async Task<ActionResult<List<TopStateResponseDto>>> GetTopState(
           [FromQuery] TopStateRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetTopStateQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (9) Получает ТОП-15 популярных образовательных программ (направлений)
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-education-standart?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список ТОП-15 популярных образовательных программ</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("top-education-standart")]
        public async Task<ActionResult<List<TopEducationStandartResponseDto>>> GetTopEducationStandart(
           [FromQuery] TopEducationStandartRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetTopEducationStandartQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (10) Получает ТОП-15 популярных профилей (специальностей)
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-education-program?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список ТОП-15 популярных профилей</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("top-education-program")]
        public async Task<ActionResult<List<TopEducationProgramResponseDto>>> GetTopEducationProgram(
           [FromQuery] TopEducationProgramRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetTopEducationProgramQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (11) Получает cредний балл по институтам
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-education-program?startYear=2020&amp;endYear=2023
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список cредних баллов по институтам</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("average-ball-by-institytes")]
        public async Task<ActionResult<List<AverageBallByInstitutesResponseDto>>> GetAverageBallByInstitutes(
           [FromQuery] AverageBallByInstitutesRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetAverageBallByInstitutesQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (12) Получает число студентов с инвалидностью (отдельно по льготам)
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /disability?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список кол-ва студентов с инвалидностью по льготам</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("disability")]
        public async Task<ActionResult<List<DisabilityResponseDto>>> GetDisability(
           [FromQuery] DisabilityRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetDisabilityQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// (13) Получает ТОП-10 организаций-заказчиков целевого обучения
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /top-organization?year=2023&amp;month=4
        /// </remarks>
        /// <param name="filters">Параметры фильтрации</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список ТОП-10 организаций-заказчиков целевого обучения</returns>
        /// <response code="200">Данные успешно получены</response>
        /// <response code="404">Данные не найдены</response>
        [HttpGet("top-organization")]
        public async Task<ActionResult<List<TopOrganizationResponseDto>>> GetTopOrganization(
           [FromQuery] TopOrganizationRequestDto filters,
           CancellationToken ct)
        {
            var mappedFilters = mapper.Map<FilterParams>(filters);

            var query = new GetTopOrganizationQuery(mappedFilters);
            var result = await mediator.Send(query, ct);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
