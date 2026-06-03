namespace Myservices.Application.Features.Auth.Dtos;

public class RegisterResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Message { get; set; } = null!;
}