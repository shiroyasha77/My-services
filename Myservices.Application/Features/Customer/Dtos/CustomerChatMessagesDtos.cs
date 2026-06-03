namespace Myservices.Application.Features.Customer.Dtos;

public class ChatMessagesResponse
{
    public int RequestId { get; set; }
    public string? ProviderName { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}

public class ChatMessageDto
{
    public int MessageId { get; set; }
    public string? MessageText { get; set; }
    public string SenderType { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = new();
}

public class MessageAttachmentDto
{
    public int Id { get; set; }
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
}

public class SendMessageRequestDto
{
    public string? MessageText { get; set; }
    public List<SendMessageAttachmentDto> Attachments { get; set; } = new();
}

public class SendMessageAttachmentDto
{
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public int? FileSizeBytes { get; set; }
}

public class SendMessageResponse
{
    public int MessageId { get; set; }
    public string Message { get; set; } = null!;
}