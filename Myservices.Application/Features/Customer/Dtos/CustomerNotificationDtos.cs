namespace Myservices.Application.Features.Customer.Dtos;

public class NotificationsResponse
{
    public int UnreadCount { get; set; }
    public List<NotificationItemDto> Notifications { get; set; } = new();
}

public class NotificationItemDto
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MarkNotificationAsReadResponse
{
    public string Message { get; set; } = null!;
}