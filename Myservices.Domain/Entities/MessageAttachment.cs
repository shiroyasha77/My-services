using Myservices.Domain.Entities;
using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class MessageAttachment : BaseEntity
{
    public int MessageId { get; set; }
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public int? FileSizeBytes { get; set; }

    public RequestMessage Message { get; set; } = null!;
}