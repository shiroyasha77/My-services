namespace Myservices.Application.Features.Customer.Dtos;

public class MyRequestsResponse
{
    public List<MyRequestItemDto> Current { get; set; } = new();
    public List<MyRequestItemDto> Completed { get; set; } = new();
    public List<MyRequestItemDto> Rejected { get; set; } = new();
}

public class MyRequestItemDto
{
    public int RequestId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string? ProviderName { get; set; }
    public string Status { get; set; } = null!;
    public bool IsEmergency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
}