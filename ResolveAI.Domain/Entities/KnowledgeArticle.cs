using ResolveAI.Domain.Enums;

namespace ResolveAI.Domain.Entities;

public class KnowledgeArticle
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public KnowledgeStatus Status { get; set; } = KnowledgeStatus.Draft;
    public int Version { get; set; } = 1;

    public string? FilePath { get; set; }
    public string? FileType { get; set; }

    public Guid CategoryId { get; set; }
    public KnowledgeCategory? Category { get; set; }

    public Guid CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}