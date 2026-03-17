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
    private const int MaxChunkSize = 350;
    private const int MinChunkSize = 100;

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
        var results = await cvChunkRepository.GetMostSimilarAsync(queryEmbedding, topK, query);

        return results;
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
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var chunks = new List<string>();
        var paragraphs = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var paragraph in paragraphs)
        {
            var trimmedParagraph = paragraph.Trim();
            if (string.IsNullOrWhiteSpace(trimmedParagraph))
                continue;

            if (trimmedParagraph.Length <= MaxChunkSize)
            {
                chunks.Add(trimmedParagraph);
                continue;
            }

            var lines = trimmedParagraph.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var currentChunk = new List<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                var potentialChunk = currentChunk.Count > 0
                    ? string.Join("\n", currentChunk) + "\n" + trimmedLine
                    : trimmedLine;

                if (potentialChunk.Length <= MaxChunkSize)
                {
                    currentChunk.Add(trimmedLine);
                }
                else
                {
                    if (currentChunk.Count > 0)
                    {
                        chunks.Add(string.Join("\n", currentChunk));
                    }

                    if (trimmedLine.Length > MaxChunkSize)
                    {
                        chunks.AddRange(SplitBySentences(trimmedLine));
                        currentChunk.Clear();
                    }
                    else
                    {
                        currentChunk = [trimmedLine];
                    }
                }
            }

            if (currentChunk.Count > 0)
            {
                chunks.Add(string.Join("\n", currentChunk));
            }
        }

        return chunks;
    }

    private static List<string> SplitBySentences(string text)
    {
        var chunks = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        var sentences = SplitIntoSentences(text);
        var currentChunk = new List<string>();

        foreach (var sentence in sentences)
        {
            var trimmed = sentence.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            var potentialChunk = currentChunk.Count > 0
                ? string.Join(" ", currentChunk) + " " + trimmed
                : trimmed;

            if (potentialChunk.Length <= MaxChunkSize)
            {
                currentChunk.Add(trimmed);
            }
            else
            {
                if (currentChunk.Count > 0)
                {
                    chunks.Add(string.Join(" ", currentChunk));
                }

                if (trimmed.Length > MaxChunkSize)
                {
                    for (var i = 0; i < trimmed.Length; i += MaxChunkSize - MinChunkSize)
                    {
                        var chunk = trimmed.Substring(i, Math.Min(MaxChunkSize - MinChunkSize, trimmed.Length - i));
                        if (!string.IsNullOrWhiteSpace(chunk))
                            chunks.Add(chunk);
                    }
                    currentChunk.Clear();
                }
                else
                {
                    currentChunk = [trimmed];
                }
            }
        }

        if (currentChunk.Count > 0)
        {
            chunks.Add(string.Join(" ", currentChunk));
        }

        return chunks;
    }

    private static string[] SplitIntoSentences(string text)
    {
        var sentences = new List<string>();
        var current = new System.Text.StringBuilder();

        foreach (var c in text)
        {
            current.Append(c);

            if (c == '.' || c == '!' || c == '?')
            {
                if (current.Length > 0)
                {
                    sentences.Add(current.ToString());
                    current.Clear();
                }
            }
        }

        if (current.Length > 0)
        {
            sentences.Add(current.ToString());
        }

        return sentences.ToArray();
    }
}
