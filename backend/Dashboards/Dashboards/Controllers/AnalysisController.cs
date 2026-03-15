using Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Dashboards.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController(IAIService service, IMetabaseService metabaseService): ControllerBase
    {

        [HttpPost("stream")]
        public async Task GetStream([FromBody] int cardId, CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");

            try
            {
                var email = Environment.GetEnvironmentVariable("METABASE_USER");
                var password = Environment.GetEnvironmentVariable("METABASE_PASS");

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception("Metabase credentials are not configured in environment variables.");
                }

                await metabaseService.AuthenticateAsync(email, password);

                var cardData = await metabaseService.GetCardDataJsonAsync(cardId, cancellationToken);
                Console.WriteLine(cardData);
                await foreach (var chunk in service.GetCompletionAsync(cardData, HttpContext.RequestAborted))
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
