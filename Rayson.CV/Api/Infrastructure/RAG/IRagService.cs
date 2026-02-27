namespace Infrastructure.RAG;

public interface IRagService
{
    Task InitializeAsync();
    Task<IEnumerable<string>> SearchAsync(string query, int topK = 3);
}
