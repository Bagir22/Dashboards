using Application.Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MetabaseController: ControllerBase
{
    private readonly IMetabaseService _metabaseService;
    private readonly IConfiguration _configuration;

    public MetabaseController(
        IMetabaseService metabaseService,
        IConfiguration configuration)
    {
        _metabaseService = metabaseService;
        _configuration = configuration;
    }

    [HttpGet("card-data/{cardId}")]
    public async Task<IActionResult> GetCardData(int cardId)
    {
        try
        {
            var user = _configuration["MetabaseSettings:METABASE_USER"];
            var pass = _configuration["MetabaseSettings:METABASE_PASS"];

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return BadRequest("METABASE_USER or METABASE_PASS not set in appsettings.json");
            }

            await _metabaseService.AuthenticateAsync(user, pass);
            var jsonResult = await _metabaseService.GetCardDataJsonAsync(cardId);
            return Content(jsonResult, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при получении данных карточки {cardId}: {ex.Message}");
        }
    }
}
