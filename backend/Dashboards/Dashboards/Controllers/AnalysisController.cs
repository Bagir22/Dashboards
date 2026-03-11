using Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Dashboards.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController(IAIService service): ControllerBase
    {

        [HttpPost("stream")]
        public async Task GetStream([FromBody] string userRequest)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");

            try
            {
                await foreach (var chunk in service.GetCompletionAsync(userRequest, HttpContext.RequestAborted))
                {
                    await Response.WriteAsync($"data: {chunk}\n\n");
                    await Response.Body.FlushAsync();
                    await Task.Delay(300);
                }
                
                await Response.WriteAsync("data: [DONE]\n\n");
                await Response.Body.FlushAsync();
                
            }
            catch (Exception ex)
            {
                await Response.WriteAsync($"data: Error: {ex.Message}\n\n");
                await Response.Body.FlushAsync();
            }
        }
    }
}
