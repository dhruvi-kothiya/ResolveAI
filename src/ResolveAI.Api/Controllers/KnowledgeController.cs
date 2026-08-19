using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Domain.Enums;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KnowledgeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public KnowledgeController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // =========================================================
    // 1. CREATE KNOWLEDGE CATEGORY
    // =========================================================
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateKnowledgeCategoryRequest request)
    {
        var category = new KnowledgeCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _context.KnowledgeCategories.Add(category);

        await _context.SaveChangesAsync();

        return Ok(category);
    }


    // =========================================================
    // 2. UPLOAD PDF
    // =========================================================
    [HttpPost("upload-pdf")]
    public async Task<IActionResult> UploadDocument(
        IFormFile file,
        [FromQuery] Guid categoryId,
        [FromQuery] Guid userId)
    {
        // Check file
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Check category exists
        var categoryExists = await _context.KnowledgeCategories
            .AnyAsync(c => c.Id == categoryId);

        if (!categoryExists)
        {
            return BadRequest("Knowledge category not found.");
        }

        // Check user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return BadRequest("User not found.");
        }

        // Create Uploads folder
        var uploadsFolder = Path.Combine(
            _environment.WebRootPath ?? "wwwroot",
            "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique file name
        var fileName =
            Guid.NewGuid().ToString()
            + Path.GetExtension(file.FileName);

        var filePath = Path.Combine(
            uploadsFolder,
            fileName);

        // Save file
        using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create knowledge article
        var article = new KnowledgeArticle
        {
            Id = Guid.NewGuid(),

            Title = file.FileName,

            Content = "Pending AI extraction...",

            FilePath = filePath,

            FileType = Path.GetExtension(file.FileName),

            CategoryId = categoryId,

            CreatedById = userId,

            Status = KnowledgeStatus.Published,

            Version = 1,

            CreatedAt = DateTime.UtcNow
        };

        _context.KnowledgeArticles.Add(article);

        await _context.SaveChangesAsync();

        return Accepted(new
        {
            Message = "Document uploaded and queued for processing.",
            ArticleId = article.Id
        });
    }


    // =========================================================
    // 3. CONVERT RESOLVED TICKET TO KNOWLEDGE
    // =========================================================
    [HttpPost("convert-ticket/{ticketId}")]
    public async Task<IActionResult> ConvertTicket(
        Guid ticketId,
        [FromQuery] Guid categoryId)
    {
        // 1. Find ticket
        var ticket = await _context.Tickets
            .FindAsync(ticketId);

        if (ticket == null)
        {
            return NotFound("Ticket not found.");
        }

        // 2. Ticket must be Resolved
        if (ticket.Status != TicketStatus.Resolved)
        {
            return BadRequest(
                "Only resolved tickets can be converted.");
        }

        // 3. Check category exists
        var categoryExists = await _context.KnowledgeCategories
            .AnyAsync(c => c.Id == categoryId);

        if (!categoryExists)
        {
            return BadRequest(
                "The Category ID you provided does not exist in KnowledgeCategories table.");
        }

        // 4. Check ticket creator exists
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == ticket.CreatedById);

        if (!userExists)
        {
            return BadRequest(
                "The User who created this ticket no longer exists.");
        }

        // 5. Create knowledge article
        var article = new KnowledgeArticle
        {
            Id = Guid.NewGuid(),

            Title = $"Solution for: {ticket.Title}",

            Content =
                $"Description: {ticket.Description}\n\n" +
                $"Solution: {ticket.ResolutionSummary}",

            CategoryId = categoryId,

            CreatedById = ticket.CreatedById,

            Status = KnowledgeStatus.Published,

            Version = 1,

            CreatedAt = DateTime.UtcNow
        };

        // 6. Save article
        _context.KnowledgeArticles.Add(article);

        await _context.SaveChangesAsync();

        // 7. Success response
        return Ok(new
        {
            Message = "Ticket converted to Knowledge Article!",

            ArticleId = article.Id
        });
    }


    // =========================================================
    // 4. ARCHIVE KNOWLEDGE ARTICLE
    // =========================================================
    [HttpPatch("articles/{id}/archive")]
    public async Task<IActionResult> ArchiveArticle(Guid id)
    {
        // Find article
        var article = await _context.KnowledgeArticles
            .FindAsync(id);

        if (article == null)
        {
            return NotFound("Knowledge article not found.");
        }

        // Change status
        article.Status = KnowledgeStatus.Archived;

        // Save
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Article Archived."
        });
    }
}