namespace ResolveAI.Domain.Entities;

public class KnowledgeCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<KnowledgeArticle> Articles { get; set; }
        = new List<KnowledgeArticle>();
}