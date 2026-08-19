using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KnowledgeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public KnowledgeController(ApplicationDbContext context) => _context = context;

    // new category creat (ex. Troubleshooting)
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateKnowledgeCategoryRequest request)
    {
        var category = new KnowledgeCategory { Id = Guid.NewGuid(), Name = request.Name, Description = request.Description };
        _context.KnowledgeCategories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    //artical  (ex. VPN Guide)
    [HttpPost("articles")]
    public async Task<IActionResult> CreateArticle([FromBody] CreateKnowledgeArticleRequest request)
    {
        var article = new KnowledgeArticle
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            CategoryId = request.CategoryId,
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow
        };
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Knowledge Article Published!", ArticleId = article.Id });
    }

    // show all artical
    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles()
    {
        var articles = await _context.KnowledgeArticles.Include(a => a.Category).ToListAsync();
        return Ok(articles);
    }
}