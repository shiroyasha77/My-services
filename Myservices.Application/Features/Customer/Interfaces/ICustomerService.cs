using Myservices.Application.Features.Customer.Dtos;

namespace Myservices.Application.Features.Customer.Interfaces;

public interface ICustomerService
{
    Task<CustomerHomeResponse> GetHomeAsync(int userId);

    Task<GetProvidersByServiceResponse> GetProvidersByServiceAsync(
        int serviceId,
        string? search,
        bool? availableOnly,
        string? sortBy);

    Task<ProviderProfileResponse> GetProviderProfileAsync(
        int providerId,
        int serviceId);
    Task<ProviderReviewsResponse> GetProviderReviewsAsync(int providerId);

    Task<CreateServiceRequestResponse> CreateServiceRequestAsync(
    int customerId,
    CreateServiceRequestDto request);

    Task<MyRequestsResponse> GetMyRequestsAsync(int customerId);

    Task<RequestDetailsResponse> GetRequestDetailsAsync(int customerId, int requestId);

    Task<ChatListResponse> GetChatsAsync(int customerId);

    Task<ChatMessagesResponse> GetChatMessagesAsync(int customerId, int requestId);

    Task<SendMessageResponse> SendMessageAsync(
        int customerId,
        int requestId,
        SendMessageRequestDto request);


    Task<NotificationsResponse> GetNotificationsAsync(int customerId);

    Task<MarkNotificationAsReadResponse> MarkNotificationAsReadAsync(
        int customerId,
        int notificationId);

    Task<MarkNotificationAsReadResponse> MarkAllNotificationsAsReadAsync(
        int customerId);

    Task<CustomerAccountResponse> GetAccountAsync(int customerId);

    Task<ChangeCustomerAreaResponse> ChangeAreaAsync(
        int customerId,
        ChangeCustomerAreaRequest request);

    Task<CreateEmergencyRequestResponse> CreateEmergencyRequestAsync(
    int customerId,
    CreateEmergencyRequestDto request);

    Task<ChangePasswordResponse> ChangePasswordAsync(
    int customerId,
    ChangePasswordRequest request);

}

