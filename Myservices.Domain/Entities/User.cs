
using Myservices.Domain.Entities;
using MyServices.Domain.Common;
using MyServices.Domain.Enums;

namespace MyServices.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public int? AreaId { get; set; }
    public Area? Area { get; set; }

    public Provider? ProviderProfile { get; set; }

    public ICollection<Request> CustomerRequests { get; set; } = new List<Request>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();

}