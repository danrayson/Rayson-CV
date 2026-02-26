namespace Application.Chatbot;

public class ChatbotRequest
{
    public required string Message { get; set; }
    public List<ChatMessage>? History { get; set; }
}

public class ChatMessage
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}
