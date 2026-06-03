namespace MyServices.Domain.Entities;

public class ProviderArea
{
    public int ProviderId { get; set; }
    public int AreaId { get; set; }

    public Provider Provider { get; set; } = null!;
    public Area Area { get; set; } = null!;
}