namespace Application.Contracts;

public interface IMetabaseService
{
    Task AuthenticateAsync(string email, string password);
    Task<string> GetCardDataJsonAsync(int cardId);
}
