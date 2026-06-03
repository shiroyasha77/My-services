using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class ProviderServiceDay : BaseEntity
{
    public int ProviderId { get; set; }
    public int ServiceId { get; set; }
    public string DayOfWeek { get; set; } = null!;
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }

    public Provider Provider { get; set; } = null!;
    public Service Service { get; set; } = null!;
}