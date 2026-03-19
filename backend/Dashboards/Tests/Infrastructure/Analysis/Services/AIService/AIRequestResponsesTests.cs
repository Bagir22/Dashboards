using FluentAssertions;
using Infrastructure.Analysis.Services;
using Infrastructure.Analysis.Services.AIService;
using System.Text.Json;
using Xunit;
using AIServiceAlias = Infrastructure.Analysis.Services.AIService.AIService;

namespace Tests.Infrastructure.Analysis.Services.AIService;

public class AIRequestResponsesTests
{
    #region ChatRequest Tests

    [Fact]
    public void ChatRequest_ShouldHaveDefaultValues()
    {
        var request = new ChatRequest();

        request.Model.Should().Be("stepfun/step-3.5-flash:free");
        request.Stream.Should().BeFalse();
        request.Reasoning.Should().NotBeNull();
        request.Reasoning.Enabled.Should().BeTrue();
        request.Messages.Should().BeNull();
    }

    [Fact]
    public void ChatRequest_ShouldAllowSettingProperties()
    {
        var messages = new[]
        {
            new Message { Role = "user", Content = "test" }
        };

        var request = new ChatRequest
        {
            Model = "custom-model",
            Messages = messages,
            Reasoning = new Reasoning { Enabled = false },
            Stream = true
        };

        request.Model.Should().Be("custom-model");
        request.Messages.Should().BeSameAs(messages);
        request.Reasoning.Enabled.Should().BeFalse();
        request.Stream.Should().BeTrue();
    }

    [Fact]
    public void ChatRequest_ShouldSerializeToJsonCorrectly()
    {
        var request = new ChatRequest
        {
            Model = "test-model",
            Messages = new[]
            {
                new Message { Role = "user", Content = "Hello" },
                new Message { Role = "assistant", Content = "Hi" }
            },
            Reasoning = new Reasoning { Enabled = true },
            Stream = true
        };

        var json = JsonSerializer.Serialize(request);
        var deserialized = JsonSerializer.Deserialize<ChatRequest>(json);

        deserialized.Should().NotBeNull();
        deserialized.Model.Should().Be("test-model");
        deserialized.Stream.Should().BeTrue();
        deserialized.Reasoning.Enabled.Should().BeTrue();
        deserialized.Messages.Should().HaveCount(2);
        deserialized.Messages[0].Role.Should().Be("user");
        deserialized.Messages[0].Content.Should().Be("Hello");
        deserialized.Messages[1].Role.Should().Be("assistant");
        deserialized.Messages[1].Content.Should().Be("Hi");
    }

    [Fact]
    public void ChatRequest_ShouldSerializeWithJsonPropertyNames()
    {
        var request = new ChatRequest
        {
            Model = "test",
            Stream = true
        };

        var json = JsonSerializer.Serialize(request);

        json.Should().Contain("\"model\":\"test\"");
        json.Should().Contain("\"stream\":true");
        json.Should().Contain("\"reasoning\":");
    }

    #endregion

    #region Message Tests

    [Fact]
    public void Message_ShouldAllowSettingProperties()
    {
        var message = new Message
        {
            Role = "user",
            Content = "test content"
        };

        message.Role.Should().Be("user");
        message.Content.Should().Be("test content");
    }

    [Fact]
    public void Message_ShouldSerializeToJsonCorrectly()
    {
        var message = new Message
        {
            Role = "system",
            Content = "You are a helpful assistant"
        };

        var json = JsonSerializer.Serialize(message);
        var deserialized = JsonSerializer.Deserialize<Message>(json);

        deserialized.Should().NotBeNull();
        deserialized.Role.Should().Be("system");
        deserialized.Content.Should().Be("You are a helpful assistant");
    }

    [Fact]
    public void Message_ShouldSerializeWithJsonPropertyNames()
    {
        var message = new Message
        {
            Role = "user",
            Content = "hello"
        };

        var json = JsonSerializer.Serialize(message);

        json.Should().Contain("\"role\":\"user\"");
        json.Should().Contain("\"content\":\"hello\"");
    }

    #endregion

    #region Reasoning Tests

    [Fact]
    public void Reasoning_ShouldAllowSettingProperties()
    {
        var reasoning = new Reasoning
        {
            Enabled = true
        };

        reasoning.Enabled.Should().BeTrue();
    }

    [Fact]
    public void Reasoning_ShouldSerializeToJsonCorrectly()
    {
        var reasoning = new Reasoning { Enabled = false };

        var json = JsonSerializer.Serialize(reasoning);
        var deserialized = JsonSerializer.Deserialize<Reasoning>(json);

        deserialized.Should().NotBeNull();
        deserialized.Enabled.Should().BeFalse();
    }

    [Fact]
    public void Reasoning_ShouldSerializeWithJsonPropertyName()
    {
        var reasoning = new Reasoning { Enabled = true };

        var json = JsonSerializer.Serialize(reasoning);

        json.Should().Contain("\"enabled\":true");
    }

    #endregion

    #region ChatCompletionChunk Tests

    [Fact]
    public void ChatCompletionChunk_ShouldAllowSettingProperties()
    {
        var chunk = new ChatCompletionChunk
        {
            Id = "chunk123",
            Choices = new List<Choice>
            {
                new Choice
                {
                    Index = 0,
                    Delta = new Delta { Content = "Hello", Role = "assistant" },
                    FinishReason = null
                }
            }
        };

        chunk.Id.Should().Be("chunk123");
        chunk.Choices.Should().HaveCount(1);
        chunk.Choices[0].Index.Should().Be(0);
        chunk.Choices[0].Delta.Content.Should().Be("Hello");
        chunk.Choices[0].Delta.Role.Should().Be("assistant");
        chunk.Choices[0].FinishReason.Should().BeNull();
    }

    [Fact]
    public void ChatCompletionChunk_ShouldSerializeToJsonCorrectly()
    {
        var chunk = new ChatCompletionChunk
        {
            Id = "test-id",
            Choices = new List<Choice>
            {
                new Choice
                {
                    Index = 1,
                    Delta = new Delta { Content = "world" },
                    FinishReason = "stop"
                }
            }
        };

        var json = JsonSerializer.Serialize(chunk);
        var deserialized = JsonSerializer.Deserialize<ChatCompletionChunk>(json);

        deserialized.Should().NotBeNull();
        deserialized.Id.Should().Be("test-id");
        deserialized.Choices.Should().HaveCount(1);
        deserialized.Choices[0].Index.Should().Be(1);
        deserialized.Choices[0].Delta.Content.Should().Be("world");
        deserialized.Choices[0].FinishReason.Should().Be("stop");
    }

    [Fact]
    public void ChatCompletionChunk_ShouldHandleMultipleChoices()
    {
        var chunk = new ChatCompletionChunk
        {
            Id = "multi",
            Choices = new List<Choice>
            {
                new Choice { Index = 0, Delta = new Delta { Content = "first" } },
                new Choice { Index = 1, Delta = new Delta { Content = "second" } },
                new Choice { Index = 2, Delta = new Delta { Content = "third" } }
            }
        };

        var json = JsonSerializer.Serialize(chunk);
        var deserialized = JsonSerializer.Deserialize<ChatCompletionChunk>(json);

        deserialized.Choices.Should().HaveCount(3);
        deserialized.Choices[0].Delta.Content.Should().Be("first");
        deserialized.Choices[1].Delta.Content.Should().Be("second");
        deserialized.Choices[2].Delta.Content.Should().Be("third");
    }

    [Fact]
    public void ChatCompletionChunk_ShouldHandleNullDelta()
    {
        var chunk = new ChatCompletionChunk
        {
            Id = "null-delta",
            Choices = new List<Choice>
            {
                new Choice { Index = 0, Delta = null, FinishReason = "stop" }
            }
        };

        var json = JsonSerializer.Serialize(chunk);
        var deserialized = JsonSerializer.Deserialize<ChatCompletionChunk>(json);

        deserialized.Choices[0].Delta.Should().BeNull();
        deserialized.Choices[0].FinishReason.Should().Be("stop");
    }

    [Fact]
    public void ChatCompletionChunk_ShouldSerializeWithJsonPropertyNames()
    {
        var chunk = new ChatCompletionChunk
        {
            Id = "test",
            Choices = new List<Choice>()
        };

        var json = JsonSerializer.Serialize(chunk);

        json.Should().Contain("\"id\":\"test\"");
        json.Should().Contain("\"choices\":");
    }

    #endregion

    #region Choice Tests

    [Fact]
    public void Choice_ShouldAllowSettingProperties()
    {
        var choice = new Choice
        {
            Index = 5,
            Delta = new Delta { Content = "test" },
            FinishReason = "length"
        };

        choice.Index.Should().Be(5);
        choice.Delta.Content.Should().Be("test");
        choice.FinishReason.Should().Be("length");
    }

    [Fact]
    public void Choice_ShouldSerializeToJsonCorrectly()
    {
        var choice = new Choice
        {
            Index = 2,
            Delta = new Delta { Role = "assistant" },
            FinishReason = null
        };

        var json = JsonSerializer.Serialize(choice);
        var deserialized = JsonSerializer.Deserialize<Choice>(json);

        deserialized.Index.Should().Be(2);
        deserialized.Delta.Role.Should().Be("assistant");
        deserialized.FinishReason.Should().BeNull();
    }

    #endregion

    #region Delta Tests

    [Fact]
    public void Delta_ShouldAllowSettingProperties()
    {
        var delta = new Delta
        {
            Content = "hello",
            Role = "user"
        };

        delta.Content.Should().Be("hello");
        delta.Role.Should().Be("user");
    }

    [Fact]
    public void Delta_ShouldSerializeToJsonCorrectly()
    {
        var delta = new Delta
        {
            Content = "world",
            Role = "assistant"
        };

        var json = JsonSerializer.Serialize(delta);
        var deserialized = JsonSerializer.Deserialize<Delta>(json);

        deserialized.Content.Should().Be("world");
        deserialized.Role.Should().Be("assistant");
    }

    [Fact]
    public void Delta_ShouldHandleNullProperties()
    {
        var delta = new Delta();

        var json = JsonSerializer.Serialize(delta);
        var deserialized = JsonSerializer.Deserialize<Delta>(json);

        deserialized.Content.Should().BeNull();
        deserialized.Role.Should().BeNull();
    }

    #endregion

    #region Полная сериализация

    [Fact]
    public void FullChatRequest_ShouldRoundtripCorrectly()
    {
        var original = new ChatRequest
        {
            Model = "test-model",
            Messages = new[]
            {
                new Message { Role = "system", Content = "You are AI" },
                new Message { Role = "user", Content = "Hello" }
            },
            Reasoning = new Reasoning { Enabled = true },
            Stream = true
        };

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<ChatRequest>(json);

        deserialized.Model.Should().Be(original.Model);
        deserialized.Stream.Should().Be(original.Stream);
        deserialized.Reasoning.Enabled.Should().Be(original.Reasoning.Enabled);
        deserialized.Messages.Should().HaveCount(2);
        deserialized.Messages[0].Role.Should().Be(original.Messages[0].Role);
        deserialized.Messages[0].Content.Should().Be(original.Messages[0].Content);
        deserialized.Messages[1].Role.Should().Be(original.Messages[1].Role);
        deserialized.Messages[1].Content.Should().Be(original.Messages[1].Content);
    }

    [Fact]
    public void FullChunkResponse_ShouldRoundtripCorrectly()
    {
        var original = new ChatCompletionChunk
        {
            Id = "chunk-123",
            Choices = new List<Choice>
            {
                new Choice
                {
                    Index = 0,
                    Delta = new Delta { Content = "Part 1" },
                    FinishReason = null
                },
                new Choice
                {
                    Index = 1,
                    Delta = new Delta { Content = "Part 2" },
                    FinishReason = null
                }
            }
        };

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<ChatCompletionChunk>(json);

        deserialized.Id.Should().Be(original.Id);
        deserialized.Choices.Should().HaveCount(2);
        deserialized.Choices[0].Delta.Content.Should().Be("Part 1");
        deserialized.Choices[1].Delta.Content.Should().Be("Part 2");
    }

    #endregion
}
