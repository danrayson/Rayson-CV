namespace Infrastructure.RAG;

public interface ICvChunkRepository
{
    Task AddRangeAsync(IEnumerable<Domain.CvChunk> chunks);
    Task<bool> ExistsAsync();
    Task<IEnumerable<string>> GetMostSimilarAsync(float[] embedding, int topK);
}
