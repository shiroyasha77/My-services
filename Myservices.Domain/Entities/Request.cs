using Myservices.Domain.Entities;
using MyServices.Domain.Common;
using MyServices.Domain.Enums;

namespace MyServices.Domain.Entities;

public class Request : BaseEntity
{
    public int UserId { get; set; }
    public int? ProviderId { get; set; }
    public int ServiceId { get; set; }
    public int AreaId { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public string? ImageUrlsJson { get; set; } = null;
    public string? Description { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public bool IsEmergency { get; set; } = false;
    public int? EmergencyExpiresMins { get; set; }

    public User User { get; set; } = null!;
    public Provider? Provider { get; set; }
    public Service Service { get; set; } = null!;
    public Area Area { get; set; } = null!;

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<RequestMessage> MessagesText { get; set; } = new List<RequestMessage>();
    
}