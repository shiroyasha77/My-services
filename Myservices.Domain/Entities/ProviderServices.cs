namespace MyServices.Domain.Entities;

public class ProviderService
{
    public int ProviderId { get; set; }
    public int ServiceId { get; set; }

    public string? EducationLevel { get; set; }
    public string? State { get; set; }
    public int? YearsExperience { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Provider Provider { get; set; } = null!;
    public Service Service { get; set; } = null!;
}