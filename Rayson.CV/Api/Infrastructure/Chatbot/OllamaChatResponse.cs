namespace Infrastructure.Chatbot;

internal class OllamaChatResponse
{
    public OllamaMessage? Message { get; set; }
    public bool Done { get; set; }
}
