namespace ResolveAI.Application.DTOs;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; } // department problem
    public Guid CreatedById { get; set; } // who ticket creat
}