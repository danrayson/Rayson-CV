using Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.RAG;

public class CvChunkRepository(RaysonCVDbContext context) : ICvChunkRepository
{
    private readonly RaysonCVDbContext _context = context;

    public async Task AddRangeAsync(IEnumerable<Domain.CvChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            var embeddingStr = string.Join(",", chunk.Embedding.Select(e => e.ToString("G10", System.Globalization.CultureInfo.InvariantCulture)));
            var deletedAtStr = chunk.DeletedAt.HasValue 
                ? $"'{chunk.DeletedAt.Value:u}'" 
                : "NULL";
            
            await _context.Database.ExecuteSqlRawAsync($@"
                INSERT INTO ""CvChunks"" (""Content"", ""Section"", ""Embedding"", ""ChunkIndex"", ""CreatedAt"", ""DeletedAt"")
                VALUES ('{chunk.Content.Replace("'", "''")}', '{chunk.Section.Replace("'", "''")}', '[{embeddingStr}]'::vector, {chunk.ChunkIndex}, '{chunk.CreatedAt:u}', {deletedAtStr})");
        }
    }

    public async Task<bool> ExistsAsync()
    {
        return await _context.CvChunks.AnyAsync();
    }

    public async Task<IEnumerable<Domain.CvChunk>> GetMostSimilarAsync(float[] embedding, int topK)
    {
        var embeddingString = string.Join(",", embedding.Select(e => e.ToString("G10", System.Globalization.CultureInfo.InvariantCulture)));
        
        var sql = $@"
            SELECT ""Id"", ""Content"", ""Section"", ""Embedding"", ""ChunkIndex"", ""CreatedAt"", ""DeletedAt""
            FROM ""CvChunks""
            ORDER BY ""Embedding"" <-> '[{embeddingString}]'::vector
            LIMIT {topK}";

        var result = await _context.CvChunks
            .FromSqlRaw(sql)
            .ToListAsync();

        return result;
    }
}
