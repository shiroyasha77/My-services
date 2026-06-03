namespace Myservices.Application.Features.Auth.Dtos;

public class ResetPasswordRequest
{
    public string Login { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}