namespace Domain;

public class CvChunk : Entity
{
    public required string Content { get; set; }
    public required string Section { get; set; }
    public required double[] Embedding { get; set; }
    public int ChunkIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
