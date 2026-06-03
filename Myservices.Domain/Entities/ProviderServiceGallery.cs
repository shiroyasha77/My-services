using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class ProviderServiceGallery : BaseEntity
{
    public int ProviderId { get; set; }
    public int ServiceId { get; set; }
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public int? FileSizeBytes { get; set; }

    public Provider Provider { get; set; } = null!;
    public Service Service { get; set; } = null!;
}