using Microsoft.Extensions.AI;
using OllamaSharp;

namespace LocalAIWeb.Services;

public class ChatService
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _chatHistory = [];

    public ChatService()
    {
        _chatClient = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");
    }

    public IReadOnlyList<ChatMessage> ChatHistory => _chatHistory;

    public async IAsyncEnumerable<string> SendMessageAsync(string userMessage)
    {
        _chatHistory.Add(new ChatMessage(ChatRole.User, userMessage));

        var response = "";
        await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatHistory))
        {
            if (item.Text is not null)
            {
                response += item.Text;
                yield return item.Text;
            }
        }

        _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
    }
}
