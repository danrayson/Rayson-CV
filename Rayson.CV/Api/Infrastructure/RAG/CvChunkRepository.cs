using Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.RAG;

public class CvChunkRepository(RaysonCVDbContext context) : ICvChunkRepository
{
    private readonly RaysonCVDbContext _context = context;
    private const double SectionBoostAmount = 0.15;

    private static readonly Dictionary<string, string[]> SectionKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["PERSONAL DETAILS"] = ["personal", "details", "contact", "email", "phone", "address", "name"],
        ["EDUCATION"] = ["education", "school", "university", "degree", "qualification", "college", "gcse", "a-level", "mathematics"],
        ["PROFESSIONAL SKILLS SUMMARY"] = ["skill", "skills", "tech", "technology", "stack", "experience", "expertise", "language", "framework", "tool"],
        ["WORK EXPERIENCE"] = ["work", "job", "employment", "career", "position", "role", "company", "employer", "professional"],
        ["HOME EXPERIENCE"] = ["home", "personal", "project", "projects", "hobby", "interest", "side"],
        ["PERSONAL DESCRIPTION"] = ["about", "description", "profile", "background", "summary", "character"],
        ["References"] = ["reference", "references", "referee", "contact"]
    };

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

    public async Task<IEnumerable<string>> GetMostSimilarAsync(float[] embedding, int topK)
    {
        return await GetMostSimilarAsync(embedding, topK, null);
    }

    public async Task<IEnumerable<string>> GetMostSimilarAsync(float[] embedding, int topK, string? query)
    {
        var embeddingString = string.Join(",", embedding.Select(e => e.ToString("G10", System.Globalization.CultureInfo.InvariantCulture)));
        
        var (matchedSection, boost) = query != null 
            ? FindMatchingSection(query) 
            : (null, 0.0);

        string sql;
        if (matchedSection != null && boost > 0)
        {
            sql = $@"
                SELECT ""Content""
                FROM ""CvChunks""
                ORDER BY ""Embedding"" <-> '[{embeddingString}]'::vector + 
                         (CASE WHEN ""Section"" = '{matchedSection.Replace("'", "''")}' THEN {boost.ToString("G10", System.Globalization.CultureInfo.InvariantCulture)} ELSE 0 END)
                LIMIT {topK}";
        }
        else
        {
            sql = $@"
                SELECT ""Content""
                FROM ""CvChunks""
                ORDER BY ""Embedding"" <-> '[{embeddingString}]'::vector
                LIMIT {topK}";
        }

        var result = await _context.Database
            .SqlQueryRaw<string>(sql)
            .ToListAsync();

        return result;
    }

    private static (string? Section, double Boost) FindMatchingSection(string query)
    {
        var queryLower = query.ToLowerInvariant();
        var queryWords = queryLower.Split([' ', ',', '.', '!', '?', ';', ':', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        if (queryWords.Length == 0)
            return (null, 0);

        var sectionScores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var (section, keywords) in SectionKeywords)
        {
            var score = 0.0;

            foreach (var queryWord in queryWords)
            {
                if (string.IsNullOrWhiteSpace(queryWord) || queryWord.Length < 3)
                    continue;

                foreach (var keyword in keywords)
                {
                    var similarity = CalculateSimilarity(queryWord, keyword);
                    if (similarity > 0.6)
                    {
                        score += similarity;
                    }
                    else if (keyword.Contains(queryWord, StringComparison.OrdinalIgnoreCase) || 
                             queryWord.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        score += 0.5;
                    }
                }
            }

            if (score > 0)
            {
                sectionScores[section] = score;
            }
        }

        if (sectionScores.Count == 0)
            return (null, 0);

        var bestSection = sectionScores.OrderByDescending(x => x.Value).First();
        
        var maxPossibleScore = queryWords.Length * 1.0;
        var normalizedBoost = Math.Min(bestSection.Value / maxPossibleScore, 1.0) * SectionBoostAmount;

        return (bestSection.Key, normalizedBoost);
    }

    private static double CalculateSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0;

        var distance = LevenshteinDistance(s1, s2);
        var maxLength = Math.Max(s1.Length, s2.Length);

        return 1.0 - (double)distance / maxLength;
    }

    private static int LevenshteinDistance(string s1, string s2)
    {
        var m = s1.Length;
        var n = s2.Length;
        var d = new int[m + 1, n + 1];

        for (var i = 0; i <= m; i++)
            d[i, 0] = i;
        for (var j = 0; j <= n; j++)
            d[0, j] = j;

        for (var i = 1; i <= m; i++)
        {
            for (var j = 1; j <= n; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[m, n];
    }
}
