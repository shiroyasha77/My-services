using MyServices.Domain.Common;
using MyServices.Domain.Entities;

namespace Myservices.Domain.Entities;

public class RequestMessage : BaseEntity
{
    public int RequestId { get; set; }

    public string? MessageText { get; set; } = null;

    // Customer أو Provider
    public string SenderType { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public Request Request { get; set; } = null!;

    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}