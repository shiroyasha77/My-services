namespace Myservices.Application.Features.Customer.Dtos;

public class CustomerHomeResponse
{
    public string UserName { get; set; } = null!;
    public int UnreadMessagesCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public ActiveRequestDto? ActiveRequest { get; set; }
    public List<HomeServiceDto> Services { get; set; } = new();
}

public class ActiveRequestDto
{
    public int RequestId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ProviderName { get; set; }
    public string ButtonText { get; set; } = "تابع الطلب";
}

public class HomeServiceDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
}