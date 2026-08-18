namespace ResolveAI.Application.DTOs;

public class DashboardStatsResponse
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int HighPriorityTickets { get; set; }
    public int TotalDepartments { get; set; }
}