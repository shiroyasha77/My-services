namespace Myservices.Application.Features.Customer.Dtos;

public class ProviderProfileResponse
{
    public int ProviderId { get; set; }
    public string FullName { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public bool IsAvailable { get; set; }

    public int ServiceId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string AreaName { get; set; } = "غير محدد";

    public int? YearsExperience { get; set; }
    public int CompletedWorksCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }

    public string? Bio { get; set; }

    public List<ProviderGalleryItemDto> Gallery { get; set; } = new();
    public List<ProviderReviewDto> Reviews { get; set; } = new();
}

public class ProviderGalleryItemDto
{
    public int Id { get; set; }
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
}

public class ProviderReviewDto
{
    public int RatingId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime RatedAt { get; set; }
}