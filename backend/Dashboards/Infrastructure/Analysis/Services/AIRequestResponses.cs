namespace Infrastructure.Analysis.Services
{
    using System.Text.Json.Serialization;

    public class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = Environment.GetEnvironmentVariable("AI_MODEL") ?? String.Empty;
        [JsonPropertyName("messages")]
        public Message[] Messages { get; set; }
        [JsonPropertyName("reasoning")]
        public Reasoning Reasoning { get; set; } = new Reasoning { Enabled = true };
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class Reasoning
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }

    public class ChatCompletionChunk
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public Delta Delta { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Delta
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
