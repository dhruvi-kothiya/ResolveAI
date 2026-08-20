using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET ALL DEPARTMENTS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();

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

        return Ok(new
        {
            Message = "Department created successfully!",
            Id = department.Id
        });
    }
}