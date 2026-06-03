namespace Myservices.Application.Features.Auth.Dtos;

public class VerifyOtpRequest
{
    public string Login { get; set; } = null!;
    public string Code { get; set; } = null!;
}