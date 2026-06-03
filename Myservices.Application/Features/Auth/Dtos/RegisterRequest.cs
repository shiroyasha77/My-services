using System.ComponentModel.DataAnnotations;

namespace Myservices.Application.Features.Auth.Dtos;

public class RegisterRequest
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "الإيميل مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة الإيميل غير صحيحة")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "رقم الجوال مطلوب")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "نوع الحساب مطلوب")]
    public string Role { get; set; } = null!;
}