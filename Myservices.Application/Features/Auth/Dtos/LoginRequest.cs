using System.ComponentModel.DataAnnotations;

namespace Myservices.Application.Features.Auth.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage = "البريد الإلكتروني أو رقم الجوال مطلوب")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "نوع الحساب مطلوب")]
    public string Role { get; set; } = null!;
}