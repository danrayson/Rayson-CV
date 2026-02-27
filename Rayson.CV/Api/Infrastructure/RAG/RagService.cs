using Application.Chatbot;
using Domain;
using Microsoft.Extensions.Logging;

namespace Infrastructure.RAG;

public class RagService(
    ICvChunkRepository cvChunkRepository,
    IEmbeddingService embeddingService,
    ICvProvider cvProvider,
    ILogger<RagService> logger) : IRagService
{
    private const int ChunkSize = 150;
    private const int Overlap = 50;

    public async Task InitializeAsync()
    {
        if (await cvChunkRepository.ExistsAsync())
        {
            logger.LogInformation("CV chunks already exist in database, skipping RAG initialization");
            return;
        }

        logger.LogInformation("Starting RAG initialization...");

        await embeddingService.EnsureModelAvailableAsync();

        var cvContent = cvProvider.GetCvContent();
        var chunks = ChunkCvContent(cvContent);

        logger.LogInformation("Generated {Count} chunks from CV", chunks.Count);

        var cvChunks = new List<CvChunk>();
        foreach (var (content, section, index) in chunks)
        {
            logger.LogDebug("Generating embedding for chunk {Index} ({Section})", index, section);
            var embedding = await embeddingService.GenerateEmbeddingAsync(content);

            cvChunks.Add(new CvChunk
            {
                Content = content,
                Section = section,
                Embedding = [.. embedding.Select(e => (double)e)],
                ChunkIndex = index,
                CreatedAt = DateTime.UtcNow
            });
        }

        await cvChunkRepository.AddRangeAsync(cvChunks);

        logger.LogInformation("RAG initialization complete. Saved {Count} chunks to database", cvChunks.Count);
    }

    public async Task<IEnumerable<string>> SearchAsync(string query, int topK = 3)
    {
        var queryEmbedding = await embeddingService.GenerateEmbeddingAsync(query);
        var results = await cvChunkRepository.GetMostSimilarAsync(queryEmbedding, topK);

        return results.Select(r => r.Content);
    }

    private static List<(string Content, string Section, int Index)> ChunkCvContent(string cvContent)
    {
        var chunks = new List<(string Content, string Section, int Index)>();
        var lines = cvContent.Split('\n');
        var currentSection = "General";
        var sectionContent = new List<string>();
        var chunkIndex = 0;

        var sectionHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PERSONAL DETAILS",
            "EDUCATION",
            "PROFESSIONAL SKILLS SUMMARY",
            "WORK EXPERIENCE",
            "HOME EXPERIENCE",
            "PERSONAL DESCRIPTION",
            "References"
        };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (sectionHeaders.Contains(trimmedLine))
            {
                if (sectionContent.Count > 0)
                {
                    var sectionText = string.Join("\n", sectionContent);
                    var sectionChunks = SplitIntoChunks(sectionText, currentSection);
                    chunks.AddRange(sectionChunks.Select((content, i) => (content, currentSection, chunkIndex++)));
                    sectionContent.Clear();
                }

                currentSection = trimmedLine;
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(line))
                    sectionContent.Add(line);
            }
        }

        if (sectionContent.Count > 0)
        {
            var sectionText = string.Join("\n", sectionContent);
            var sectionChunks = SplitIntoChunks(sectionText, currentSection);
            chunks.AddRange(sectionChunks.Select((content, i) => (content, currentSection, chunkIndex++)));
        }

        return chunks;
    }

    private static List<string> SplitIntoChunks(string text, string section)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        if (text.Length <= ChunkSize)
        {
            chunks.Add(text.Trim());
            return chunks;
        }

        for (var start = 0; start < text.Length; start += ChunkSize - Overlap)
        {
            var end = Math.Min(start + ChunkSize, text.Length);
            var chunk = text[start..end].Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                chunks.Add(chunk);
            }
        }

        return chunks;
    }
}
