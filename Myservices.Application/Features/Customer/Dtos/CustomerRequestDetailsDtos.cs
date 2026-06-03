namespace Myservices.Application.Features.Customer.Dtos;

public class RequestDetailsResponse
{
    public int RequestId { get; set; }
    public string RequestNumber { get; set; } = null!;
    public string Status { get; set; } = null!;

    public string ServiceType { get; set; } = null!;
    public string? ProviderName { get; set; }
    public string? ProviderPhone { get; set; }

    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = new();

    public string AreaName { get; set; } = null!;
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public List<RequestTrackingStepDto> TrackingSteps { get; set; } = new();
}

public class RequestTrackingStepDto
{
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}