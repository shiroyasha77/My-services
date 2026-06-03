namespace Myservices.Application.Features.Customer.Dtos;

public class CustomerAccountResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public AreaDto? CurrentArea { get; set; }
}

public class AreaDto
{
    public int AreaId { get; set; }
    public string Name { get; set; } = null!;
    public string Level { get; set; } = null!;
}

public class ChangeCustomerAreaRequest
{
    public int AreaId { get; set; }
}

public class ChangeCustomerAreaResponse
{
    public string Message { get; set; } = null!;
}