using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class Rating : BaseEntity
{
    public int RequestId { get; set; }
    public int Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime RatedAt { get; set; } = DateTime.UtcNow;

    public Request Request { get; set; } = null!;
}