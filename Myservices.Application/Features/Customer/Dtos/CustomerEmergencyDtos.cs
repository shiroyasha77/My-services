namespace Myservices.Application.Features.Customer.Dtos;

public class CreateEmergencyRequestDto
{
    public int ServiceId { get; set; }
    public int AreaId { get; set; }

    public string? Description { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}

public class CreateEmergencyRequestResponse
{
    public int RequestId { get; set; }
    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}