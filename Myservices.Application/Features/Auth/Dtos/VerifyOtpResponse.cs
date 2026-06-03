namespace Myservices.Application.Features.Auth.Dtos;

public class VerifyOtpResponse
{
    public string Message { get; set; } = null!;
    public bool IsVerified { get; set; }
}