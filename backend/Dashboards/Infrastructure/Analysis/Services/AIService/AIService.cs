using Application.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Analysis.Services.AIService
{
    public class AIService(HttpClient httpClient): IAIService
    {
        private const string BaseUrl = "https://openrouter.ai/api/v1/chat/completions";

        public async IAsyncEnumerable<string> GetCompletionAsync(string cardData, [EnumeratorCancellation] CancellationToken ct)
        {
            var searchRequest = Prompt.GetFromCardData(cardData);
            var request = new ChatRequest
            {
                Messages = [new Message { Role = "user", Content = searchRequest }],
                Reasoning = new Reasoning { Enabled = true },
                Stream = true,
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseUrl, content);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(ct);
                if (string.IsNullOrWhiteSpace(line) || line == ": OPENROUTER PROCESSING")
                    continue;

                if (line.StartsWith("data: "))
                {
                    var jsonData = line.Substring(6);
                    
                    if (jsonData == "[DONE]")
                        break;

                    var chunk = JsonSerializer.Deserialize<ChatCompletionChunk>(jsonData);
                    if (chunk?.Choices?.Count() > 0 &&
                        !string.IsNullOrEmpty(chunk.Choices[0].Delta?.Content))
                    {
                        yield return chunk.Choices[0].Delta.Content;
                    }
                }
            }
        }
    }
}
