using Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetabaseController(IMetabaseService metabaseService): ControllerBase
    {
        /// <summary>
        /// Получает данные конкретной карточки Metabase по её ID
        /// </summary>
        /// <param name="cardId">ID карточки (число из URL в Metabase)</param>
        [HttpGet("card-data/{cardId}")]
        public async Task<IActionResult> GetCardData(int cardId)
        {
            try
            {
                // Аторизуемся
                await metabaseService.AuthenticateAsync("admin@example.com", "Admin123Qwerty");

                // Запрашиваем данные карточки
                var jsonResult = await metabaseService.GetCardDataJsonAsync(cardId);

                // Возвращаем результат как чистый JSON
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении данных карточки {cardId}: {ex.Message}");
            }
        }
    }
}
