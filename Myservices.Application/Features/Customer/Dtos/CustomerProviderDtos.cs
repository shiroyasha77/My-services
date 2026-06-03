namespace Myservices.Application.Features.Customer.Dtos;

public class GetProvidersByServiceResponse
{
    public int ServiceId { get; set; }
    public string ServiceType { get; set; } = null!;
    public List<ProviderListItemDto> Providers { get; set; } = new();
}

public class ProviderListItemDto
{
    public int ProviderId { get; set; }
    public string FullName { get; set; } = null!;
    public string ServiceType { get; set; } = null!;
    public string AreaName { get; set; } = "غير محدد";
    public int? YearsExperience { get; set; }
    public bool IsAvailable { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public string? ProfileImageUrl { get; set; }
}