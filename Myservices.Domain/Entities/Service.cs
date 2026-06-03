using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class Service : BaseEntity
{
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();
    public ICollection<ProviderServiceDay> ProviderServiceDays { get; set; } = new List<ProviderServiceDay>();
    public ICollection<ProviderServiceGallery> ProviderServiceGallery { get; set; } = new List<ProviderServiceGallery>();
}