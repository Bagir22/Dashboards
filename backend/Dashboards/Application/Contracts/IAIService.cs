using System.Runtime.CompilerServices;

namespace Application.Contracts
{ 
    public interface IAIService
    {
        IAsyncEnumerable<string> GetCompletionAsync(string search, [EnumeratorCancellation] CancellationToken ct);
    }
}
