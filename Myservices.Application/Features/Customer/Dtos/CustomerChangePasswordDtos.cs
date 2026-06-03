namespace Myservices.Application.Features.Customer.Dtos;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangePasswordResponse
{
    public string Message { get; set; } = null!;
}