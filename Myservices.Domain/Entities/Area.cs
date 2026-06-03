using MyServices.Domain.Common;

namespace MyServices.Domain.Entities;

public class Area : BaseEntity
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }

    public string Level { get; set; } = null!;

    public Area? Parent { get; set; }
    public ICollection<Area> Children { get; set; } = new List<Area>();

    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<ProviderArea> ProviderAreas { get; set; } = new List<ProviderArea>();
}