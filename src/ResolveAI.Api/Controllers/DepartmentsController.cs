using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public DepartmentsController(
        ApplicationDbContext context,
        IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }


    // =========================================================
    // GET ALL DEPARTMENTS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        // Check whether departments are already available in cache
        if (!_cache.TryGetValue(
                "dept_list",
                out List<Department>? departments))
        {
            // Cache માં data નથી,
            // એટલે database માંથી departments લાવો
            departments = await _context.Departments
                .ToListAsync();

            // Departments ને 30 minutes માટે cache માં store કરો
            _cache.Set(
                "dept_list",
                departments,
                TimeSpan.FromMinutes(30));
        }

        // Cache માંથી અથવા DB માંથી મળેલી departments return કરો
        return Ok(departments);
    }


    // =========================================================
    // CREATE DEPARTMENT
    // Only Admin can create department
    // =========================================================

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateDepartment(
        [FromBody] CreateDepartmentRequest request)
    {
        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();


        // =====================================================
        // REMOVE OLD CACHE
        // =====================================================

        // New department create થયા પછી
        // જૂની cached department list remove કરો
        _cache.Remove("dept_list");


        // =====================================================
        // RESPONSE
        // =====================================================

        return Ok(new
        {
            Message = "Department created successfully!",
            Id = department.Id
        });
    }
}