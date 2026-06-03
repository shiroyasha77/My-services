using MyServices.Domain.Common;
using MyServices.Domain.Entities;

namespace Myservices.Domain.Entities;

public class PasswordResetOtp : BaseEntity
{
    public int UserId { get; set; }
    public string Code { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsVerified { get; set; } = false;
    public bool IsUsed { get; set; } = false;

    public User User { get; set; } = null!;
}