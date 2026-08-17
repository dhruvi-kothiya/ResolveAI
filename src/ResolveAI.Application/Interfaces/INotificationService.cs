namespace ResolveAI.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string message, Guid? ticketId = null);
}