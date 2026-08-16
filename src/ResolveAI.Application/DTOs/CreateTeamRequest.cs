namespace ResolveAI.Application.DTOs;

public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; } // 
}