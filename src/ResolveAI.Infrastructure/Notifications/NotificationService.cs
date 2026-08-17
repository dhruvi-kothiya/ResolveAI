using Microsoft.AspNetCore.SignalR;

using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Notifications.SignalR;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        ApplicationDbContext context,
        IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(
        Guid userId,
        string message,
        Guid? ticketId = null)
    {
        // 1. Notification database save
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TicketId = ticketId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();


        // 2. Real-time SignalR notification
        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            message
        );
    }
}