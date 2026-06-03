using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myservices.Application.Features.Customer.Dtos;
using Myservices.Application.Features.Customer.Interfaces;
using System.Security.Claims;

namespace Myservices.Api.Controllers;

[ApiController]
[Route("api/v1/customer")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("home")]
    public async Task<IActionResult> GetHome()
    {
        var userId = GetCurrentUserId();
        var result = await _customerService.GetHomeAsync(userId);
        return Ok(result);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new Exception("المستخدم غير مصرح.");

        return int.Parse(userIdClaim);
    }

    [HttpGet("providers/by-service/{serviceId:int}")]
    public async Task<IActionResult> GetProvidersByService(
    int serviceId,
    [FromQuery] string? search,
    [FromQuery] bool? availableOnly,
    [FromQuery] string? sortBy)
    {
        var result = await _customerService.GetProvidersByServiceAsync(
            serviceId,
            search,
            availableOnly,
            sortBy);

        return Ok(result);
    }

    [HttpGet("providers/{providerId:int}/profile")]
    public async Task<IActionResult> GetProviderProfile(
    int providerId,
    [FromQuery] int serviceId)
    {
        var result = await _customerService.GetProviderProfileAsync(providerId, serviceId);
        return Ok(result);
    }

    [HttpGet("providers/{providerId:int}/reviews")]
    public async Task<IActionResult> GetProviderReviews(int providerId)
    {
        var result = await _customerService.GetProviderReviewsAsync(providerId);
        return Ok(result);
    }

    [HttpPost("requests")]
    public async Task<IActionResult> CreateServiceRequest(
    [FromBody] CreateServiceRequestDto request)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.CreateServiceRequestAsync(
            customerId,
            request);

        return Ok(result);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetMyRequestsAsync(customerId);

        return Ok(result);
    }



    [HttpGet("requests/{requestId:int}")]
    public async Task<IActionResult> GetRequestDetails(int requestId)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetRequestDetailsAsync(
            customerId,
            requestId);

        return Ok(result);
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetChats()
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetChatsAsync(customerId);

        return Ok(result);
    }

    [HttpGet("chats/{requestId:int}")]
    public async Task<IActionResult> GetChatMessages(int requestId)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetChatMessagesAsync(
            customerId,
            requestId);

        return Ok(result);
    }

    [HttpPost("chats/{requestId:int}/messages")]
    public async Task<IActionResult> SendMessage(
    int requestId,
    [FromBody] SendMessageRequestDto request)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.SendMessageAsync(
            customerId,
            requestId,
            request);

        return Ok(result);
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetNotificationsAsync(customerId);

        return Ok(result);
    }

    [HttpPut("notifications/{notificationId:int}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.MarkNotificationAsReadAsync(
            customerId,
            notificationId);

        return Ok(result);
    }

    [HttpPut("notifications/read-all")]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.MarkAllNotificationsAsReadAsync(customerId);

        return Ok(result);
    }

    [HttpGet("account")]
    public async Task<IActionResult> GetAccount()
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.GetAccountAsync(customerId);

        return Ok(result);
    }
    [HttpPut("account/area")]
    public async Task<IActionResult> ChangeArea(
    [FromBody] ChangeCustomerAreaRequest request)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.ChangeAreaAsync(
            customerId,
            request);

        return Ok(result);
    }

    [HttpPost("emergency-requests")]
    public async Task<IActionResult> CreateEmergencyRequest(
    [FromBody] CreateEmergencyRequestDto request)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.CreateEmergencyRequestAsync(
            customerId,
            request);

        return Ok(result);
    }

    [HttpPut("account/change-password")]
    public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest request)
    {
        var customerId = GetCurrentUserId();

        var result = await _customerService.ChangePasswordAsync(
            customerId,
            request);

        return Ok(result);
    }
}