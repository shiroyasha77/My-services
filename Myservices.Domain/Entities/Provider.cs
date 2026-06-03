using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class Provider : BaseEntity
{
    public int UserId { get; set; }
    public string? IdNumber { get; set; }
    public string? IdImageUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string? ExperienceCertificateUrl { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<ProviderArea> ProviderAreas { get; set; } = new List<ProviderArea>();
    public ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();
    public ICollection<ProviderServiceDay> ProviderServiceDays { get; set; } = new List<ProviderServiceDay>();
    public ICollection<ProviderServiceGallery> ProviderServiceGallery { get; set; } = new List<ProviderServiceGallery>();
}