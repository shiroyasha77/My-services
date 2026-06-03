using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool IsRead { get; set; } = false;

    public User User { get; set; } = null!;
}