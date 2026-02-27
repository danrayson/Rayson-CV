namespace Infrastructure.RAG;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task EnsureModelAvailableAsync();
}
