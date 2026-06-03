namespace Myservices.Application.Features.Customer.Dtos;

public class ProviderReviewsResponse
{
    public int ProviderId { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public List<ProviderReviewDto> Reviews { get; set; } = new();
}