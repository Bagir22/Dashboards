using Microsoft.AspNetCore.Mvc;
using Dashboards.DTOs;
using Dashboards.Services;

namespace Dashboards.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiAssistantController: ControllerBase
    {
        private readonly IAiAssistantService _aiAssistantService;
        private readonly ILogger<AiAssistantController> _logger;

        public AiAssistantController(
            IAiAssistantService aiAssistantService,
            ILogger<AiAssistantController> logger)
        {
            _aiAssistantService = aiAssistantService;
            _logger = logger;
        }

        [HttpPost("query")]
        public async Task<ActionResult<AiQueryResponse>> ProcessQuery([FromBody] AiQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new AiQueryResponse
                {
                    Success = false,
                    ErrorMessage = "Запрос не может быть пустым"
                });
            }

            var response = await _aiAssistantService.ProcessQueryAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
