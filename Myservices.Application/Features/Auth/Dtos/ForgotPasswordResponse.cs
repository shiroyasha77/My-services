namespace Myservices.Application.Features.Auth.Dtos;

public class ForgotPasswordResponse
{
    public string Message { get; set; } = null!;
    public string Code { get; set; } = null!;
}