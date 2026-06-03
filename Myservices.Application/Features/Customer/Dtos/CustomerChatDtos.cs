namespace Myservices.Application.Features.Customer.Dtos;

public class ChatListResponse
{
    public List<ChatItemDto> Chats { get; set; } = new();
}

public class ChatItemDto
{
    public int RequestId { get; set; }

    public string ServiceType { get; set; } = null!;

    public string? ProviderName { get; set; }

    public string LastMessage { get; set; } = null!;

    public DateTime LastMessageAt { get; set; }

    public int UnreadCount { get; set; }
}