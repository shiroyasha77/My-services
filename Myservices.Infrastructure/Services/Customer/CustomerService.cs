using Microsoft.EntityFrameworkCore;
using Myservices.Application.Features.Customer.Dtos;
using Myservices.Application.Features.Customer.Interfaces;
using Myservices.Domain.Entities;
using MyServices.Domain.Entities;
using MyServices.Domain.Enums;
using MyServices.Infrastructure.Persistence;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Myservices.Infrastructure.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }
    public async Task<CustomerHomeResponse> GetHomeAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new Exception("المستخدم غير موجود.");

        var activeRequest = await _context.Requests
            .Include(x => x.Service)
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .Where(x =>
                x.UserId == userId &&
                (
                    x.Status == RequestStatus.Pending ||
                    x.Status == RequestStatus.WaitingForStart ||
                    x.Status == RequestStatus.InProgress
                ))
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        var services = await _context.Services
            .Where(x => x.IsActive)
            .OrderBy(x => x.Type)
            .Take(9)
            .Select(x => new HomeServiceDto
            {
                Id = x.Id,
                Type = x.Type
            })
            .ToListAsync();

        var unreadNotificationsCount = await _context.Notifications
            .CountAsync(x => x.UserId == userId && !x.IsRead);

        return new CustomerHomeResponse
        {
            UserName = user.FullName,
            UnreadMessagesCount = 0,
            UnreadNotificationsCount = unreadNotificationsCount,

            ActiveRequest = activeRequest == null
                ? null
                : new ActiveRequestDto
                {
                    RequestId = activeRequest.Id,
                    ServiceType = activeRequest.Service.Type,
                    Status = activeRequest.Status.ToString(),
                    ProviderName = activeRequest.Provider != null
                        ? activeRequest.Provider.User.FullName
                        : null
                },

            Services = services
        };
    }

    public async Task<GetProvidersByServiceResponse> GetProvidersByServiceAsync(
    int serviceId,
    string? search,
    bool? availableOnly,
    string? sortBy)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == serviceId && x.IsActive);

        if (service == null)
            throw new Exception("الخدمة غير موجودة.");

        var query = _context.ProviderServices
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .Include(x => x.Provider)
                .ThenInclude(x => x.ProviderAreas)
                    .ThenInclude(x => x.Area)
            .Include(x => x.Service)
            .Where(x => x.ServiceId == serviceId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchValue = search.Trim().ToLower();

            query = query.Where(x =>
                x.Provider.User.FullName.ToLower().Contains(searchValue) ||
                (x.Description != null && x.Description.ToLower().Contains(searchValue)));
        }

        if (availableOnly == true)
        {
            query = query.Where(x => x.Provider.IsAvailable);
        }

        var providers = await query
            .Select(x => new ProviderListItemDto
            {
                ProviderId = x.ProviderId,
                FullName = x.Provider.User.FullName,
                ServiceType = x.Service.Type,
                YearsExperience = x.YearsExperience,
                IsAvailable = x.Provider.IsAvailable,
                ProfileImageUrl = x.Provider.ProfileImageUrl,

                AreaName = x.Provider.ProviderAreas
                    .Select(pa => pa.Area.Name)
                    .FirstOrDefault() ?? "غير محدد",

                RatingsCount = _context.Ratings
                    .Count(r => r.Request.ProviderId == x.ProviderId),

                AverageRating = _context.Ratings
                    .Where(r => r.Request.ProviderId == x.ProviderId)
                    .Select(r => (double?)r.Rate)
                    .Average() ?? 0
            })
            .ToListAsync();

        providers = sortBy?.ToLower() switch
        {
            "rating" => providers
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.RatingsCount)
                .ToList(),

            "available" => providers
                .OrderByDescending(x => x.IsAvailable)
                .ThenByDescending(x => x.AverageRating)
                .ToList(),

            "nearest" => providers
                .OrderBy(x => x.AreaName)
                .ToList(),

            _ => providers
                .OrderByDescending(x => x.IsAvailable)
                .ThenByDescending(x => x.AverageRating)
                .ToList()
        };

        return new GetProvidersByServiceResponse
        {
            ServiceId = service.Id,
            ServiceType = service.Type,
            Providers = providers
        };
    }

    public async Task<ProviderProfileResponse> GetProviderProfileAsync(
    int providerId,
    int serviceId)
    {
        var providerService = await _context.ProviderServices
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x =>
                x.ProviderId == providerId &&
                x.ServiceId == serviceId);

        if (providerService == null)
            throw new Exception("مقدم الخدمة غير موجود أو لا يقدم هذه الخدمة.");

        var areaName = await _context.ProviderAreas
            .Where(x => x.ProviderId == providerId)
            .Select(x => x.Area.Name)
            .FirstOrDefaultAsync();

        var completedWorksCount = await _context.Requests
            .CountAsync(x =>
                x.ProviderId == providerId &&
                x.Status == RequestStatus.Completed);

        var ratingsQuery = _context.Ratings
            .Where(x => x.Request.ProviderId == providerId);

        var ratingsCount = await ratingsQuery.CountAsync();

        var averageRating = await ratingsQuery
            .Select(x => (double?)x.Rate)
            .AverageAsync() ?? 0;

        var gallery = await _context.ProviderServiceGallery
            .Where(x =>
                x.ProviderId == providerId &&
                x.ServiceId == serviceId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(6)
            .Select(x => new ProviderGalleryItemDto
            {
                Id = x.Id,
                FileUrl = x.FileUrl,
                FileType = x.FileType
            })
            .ToListAsync();

        var reviews = await _context.Ratings
            .Include(x => x.Request)
                .ThenInclude(x => x.User)
            .Where(x => x.Request.ProviderId == providerId)
            .OrderByDescending(x => x.RatedAt)
            .Take(5)
            .Select(x => new ProviderReviewDto
            {
                RatingId = x.Id,
                CustomerName = x.Request.User.FullName,
                Rate = x.Rate,
                Comment = x.Comment,
                RatedAt = x.RatedAt
            })
            .ToListAsync();

        return new ProviderProfileResponse
        {
            ProviderId = providerService.ProviderId,
            FullName = providerService.Provider.User.FullName,
            ProfileImageUrl = providerService.Provider.ProfileImageUrl,
            PhoneNumber = providerService.Provider.User.PhoneNumber,
            IsAvailable = providerService.Provider.IsAvailable,

            ServiceId = providerService.ServiceId,
            ServiceType = providerService.Service.Type,
            AreaName = areaName ?? "غير محدد",

            YearsExperience = providerService.YearsExperience,
            CompletedWorksCount = completedWorksCount,
            AverageRating = Math.Round(averageRating, 1),
            RatingsCount = ratingsCount,

            Bio = providerService.Description,

            Gallery = gallery,
            Reviews = reviews
        };
    }

    public async Task<ProviderReviewsResponse> GetProviderReviewsAsync(int providerId)
    {
        var providerExists = await _context.Providers
            .AnyAsync(x => x.Id == providerId);

        if (!providerExists)
            throw new Exception("مقدم الخدمة غير موجود.");

        var ratingsQuery = _context.Ratings
            .Include(x => x.Request)
                .ThenInclude(x => x.User)
            .Where(x => x.Request.ProviderId == providerId);

        var ratingsCount = await ratingsQuery.CountAsync();

        var averageRating = await ratingsQuery
            .Select(x => (double?)x.Rate)
            .AverageAsync() ?? 0;

        var reviews = await ratingsQuery
            .OrderByDescending(x => x.RatedAt)
            .Select(x => new ProviderReviewDto
            {
                RatingId = x.Id,
                CustomerName = x.Request.User.FullName,
                Rate = x.Rate,
                Comment = x.Comment,
                RatedAt = x.RatedAt
            })
            .ToListAsync();

        return new ProviderReviewsResponse
        {
            ProviderId = providerId,
            AverageRating = Math.Round(averageRating, 1),
            RatingsCount = ratingsCount,
            Reviews = reviews
        };
    }

    public async Task<CreateServiceRequestResponse> CreateServiceRequestAsync(
    int customerId,
    CreateServiceRequestDto request)
    {
        var customerExists = await _context.Users
            .AnyAsync(x =>
                x.Id == customerId &&
                x.Role == UserRole.Customer &&
                x.IsActive);

        if (!customerExists)
            throw new Exception("العميل غير موجود أو غير مفعل.");

        var providerServiceExists = await _context.ProviderServices
            .AnyAsync(x =>
                x.ProviderId == request.ProviderId &&
                x.ServiceId == request.ServiceId);

        if (!providerServiceExists)
            throw new Exception("مقدم الخدمة لا يقدم هذه الخدمة.");

        var areaExists = await _context.Areas
            .AnyAsync(x => x.Id == request.AreaId);

        if (!areaExists)
            throw new Exception("المنطقة غير موجودة.");

        if (request.ImageUrls.Count > 4)
            throw new Exception("لا يمكن إرفاق أكثر من 4 صور.");

        var newRequest = new Request
        {
            UserId = customerId,
            ProviderId = request.ProviderId,
            ServiceId = request.ServiceId,
            AreaId = request.AreaId,
            Description = request.Description,
            ScheduledAt = request.ScheduledAt,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ImageUrlsJson = request.ImageUrls.Any()
                ? JsonSerializer.Serialize(request.ImageUrls)
                : null,
            Status = RequestStatus.Pending,
            IsEmergency = false
        };

        _context.Requests.Add(newRequest);
        await _context.SaveChangesAsync();

        return new CreateServiceRequestResponse
        {
            RequestId = newRequest.Id,
            Status = newRequest.Status.ToString(),
            Message = "تم إرسال الطلب بنجاح وهو بانتظار قبول مقدم الخدمة."
        };
    }

    public async Task<MyRequestsResponse> GetMyRequestsAsync(int customerId)
    {
        var requests = await _context.Requests
            .Include(x => x.Service)
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .Where(x => x.UserId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new MyRequestItemDto
            {
                RequestId = x.Id,
                ServiceType = x.Service.Type,
                ProviderName = x.Provider != null ? x.Provider.User.FullName : null,
                Status = x.Status.ToString(),
                IsEmergency = x.IsEmergency,
                CreatedAt = x.CreatedAt,
                ScheduledAt = x.ScheduledAt
            })
            .ToListAsync();

        return new MyRequestsResponse
        {
            Current = requests
                .Where(x =>
                    x.Status == RequestStatus.Pending.ToString() ||
                    x.Status == RequestStatus.WaitingForStart.ToString() ||
                    x.Status == RequestStatus.InProgress.ToString())
                .ToList(),

            Completed = requests
                .Where(x => x.Status == RequestStatus.Completed.ToString())
                .ToList(),

            Rejected = requests
                .Where(x => x.Status == RequestStatus.Rejected.ToString())
                .ToList()
        };
    }

    public async Task<RequestDetailsResponse> GetRequestDetailsAsync(
    int customerId,
    int requestId)
    {
        var request = await _context.Requests
            .Include(x => x.Service)
            .Include(x => x.Area)
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Id == requestId &&
                x.UserId == customerId);

        if (request == null)
            throw new Exception("الطلب غير موجود.");

        var imageUrls = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.ImageUrlsJson))
        {
            imageUrls = JsonSerializer.Deserialize<List<string>>(request.ImageUrlsJson)
                ?? new List<string>();
        }

        return new RequestDetailsResponse
        {
            RequestId = request.Id,
            RequestNumber = $"#{request.Id}",
            Status = request.Status.ToString(),

            ServiceType = request.Service.Type,
            ProviderName = request.Provider?.User.FullName,
            ProviderPhone = request.Provider?.User.PhoneNumber,

            Description = request.Description,
            ImageUrls = imageUrls,

            AreaName = request.Area.Name,
            ScheduledAt = request.ScheduledAt,
            CreatedAt = request.CreatedAt,

            Latitude = request.Latitude,
            Longitude = request.Longitude,

            TrackingSteps = BuildTrackingSteps(request.Status, request.CreatedAt)
        };
    }

    private static List<RequestTrackingStepDto> BuildTrackingSteps(
        RequestStatus status,
        DateTime createdAt)
    {
        return new List<RequestTrackingStepDto>
    {
        new RequestTrackingStepDto
        {
            Title = "تم إرسال الطلب",
            IsCompleted = true,
            CompletedAt = createdAt
        },
        new RequestTrackingStepDto
        {
            Title = "بانتظار قبول مقدم الخدمة",
            IsCompleted = status == RequestStatus.WaitingForStart ||
                          status == RequestStatus.InProgress ||
                          status == RequestStatus.Completed
        },
        new RequestTrackingStepDto
        {
            Title = "بانتظار بدء العمل",
            IsCompleted = status == RequestStatus.InProgress ||
                          status == RequestStatus.Completed
        },
        new RequestTrackingStepDto
        {
            Title = "جاري تنفيذ الخدمة",
            IsCompleted = status == RequestStatus.InProgress ||
                          status == RequestStatus.Completed
        },
        new RequestTrackingStepDto
        {
            Title = "اكتملت الخدمة",
            IsCompleted = status == RequestStatus.Completed
        }
    };
    }

    public async Task<ChatListResponse> GetChatsAsync(int customerId)
    {
        var chats = await _context.Requests
            .Include(x => x.Service)
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .Where(x => x.UserId == customerId)
            .Select(x => new
            {
                Request = x,

                LastMessage = _context.RequestMessages
                    .Where(m => m.RequestId == x.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault(),

                UnreadCount = _context.RequestMessages
                    .Count(m =>
                        m.RequestId == x.Id &&
                        m.SenderType == "Provider" &&
                        !m.IsRead)
            })
            .Where(x => x.LastMessage != null)
            .OrderByDescending(x => x.LastMessage!.CreatedAt)
            .ToListAsync();

        return new ChatListResponse
        {
            Chats = chats.Select(x => new ChatItemDto
            {
                RequestId = x.Request.Id,
                ServiceType = x.Request.Service.Type,
                ProviderName = x.Request.Provider?.User.FullName,
                LastMessage = x.LastMessage!.MessageText ?? "",
                LastMessageAt = x.LastMessage.CreatedAt,
                UnreadCount = x.UnreadCount
            }).ToList()
        };
    }

    public async Task<ChatMessagesResponse> GetChatMessagesAsync(
    int customerId,
    int requestId)
    {
        var request = await _context.Requests
            .Include(x => x.Provider)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Id == requestId &&
                x.UserId == customerId);

        if (request == null)
            throw new Exception("الطلب غير موجود.");

        var providerMessages = await _context.RequestMessages
            .Where(x =>
                x.RequestId == requestId &&
                x.SenderType == "Provider" &&
                !x.IsRead)
            .ToListAsync();

        foreach (var message in providerMessages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();

        var messages = await _context.RequestMessages
            .Include(x => x.Attachments)
            .Where(x => x.RequestId == requestId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new ChatMessageDto
            {
                MessageId = x.Id,
                MessageText = x.MessageText,
                SenderType = x.SenderType,
                CreatedAt = x.CreatedAt,
                Attachments = x.Attachments.Select(a => new MessageAttachmentDto
                {
                    Id = a.Id,
                    FileUrl = a.FileUrl,
                    FileType = a.FileType
                }).ToList()
            })
            .ToListAsync();

        return new ChatMessagesResponse
        {
            RequestId = requestId,
            ProviderName = request.Provider?.User.FullName,
            Messages = messages
        };
    }

    public async Task<SendMessageResponse> SendMessageAsync(
    int customerId,
    int requestId,
    SendMessageRequestDto request)
    {
        var requestExists = await _context.Requests
            .AnyAsync(x =>
                x.Id == requestId &&
                x.UserId == customerId);

        if (!requestExists)
            throw new Exception("الطلب غير موجود.");

        if (string.IsNullOrWhiteSpace(request.MessageText) &&
            !request.Attachments.Any())
            throw new Exception("لا يمكن إرسال رسالة فارغة.");

        var message = new RequestMessage
        {
            RequestId = requestId,
            MessageText = request.MessageText,
            SenderType = "Customer",
            IsRead = false
        };

        _context.RequestMessages.Add(message);
        await _context.SaveChangesAsync();

        if (request.Attachments.Any())
        {
            var attachments = request.Attachments.Select(x => new MessageAttachment
            {
                MessageId = message.Id,
                FileUrl = x.FileUrl,
                FileType = x.FileType,
                FileSizeBytes = x.FileSizeBytes
            }).ToList();

            _context.MessageAttachments.AddRange(attachments);
            await _context.SaveChangesAsync();
        }

        return new SendMessageResponse
        {
            MessageId = message.Id,
            Message = "تم إرسال الرسالة بنجاح."
        };
    }

    public async Task<NotificationsResponse> GetNotificationsAsync(int customerId)
    {
        var notifications = await _context.Notifications
            .Where(x => x.UserId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NotificationItemDto
            {
                NotificationId = x.Id,
                Title = x.Title,
                Body = x.Body,
                IsRead = x.IsRead,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new NotificationsResponse
        {
            UnreadCount = notifications.Count(x => !x.IsRead),
            Notifications = notifications
        };
    }

    public async Task<MarkNotificationAsReadResponse> MarkNotificationAsReadAsync(
        int customerId,
        int notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.UserId == customerId);

        if (notification == null)
            throw new Exception("الإشعار غير موجود.");

        notification.IsRead = true;

        await _context.SaveChangesAsync();

        return new MarkNotificationAsReadResponse
        {
            Message = "تم تعليم الإشعار كمقروء."
        };
    }

    public async Task<MarkNotificationAsReadResponse> MarkAllNotificationsAsReadAsync(
        int customerId)
    {
        var notifications = await _context.Notifications
            .Where(x => x.UserId == customerId && !x.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return new MarkNotificationAsReadResponse
        {
            Message = "تم تعليم جميع الإشعارات كمقروءة."
        };
    }

    public async Task<CustomerAccountResponse> GetAccountAsync(int customerId)
    {
        var user = await _context.Users
            .Include(x => x.Area)
            .FirstOrDefaultAsync(x => x.Id == customerId);

        if (user == null)
            throw new Exception("المستخدم غير موجود.");

        return new CustomerAccountResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CurrentArea = user.Area == null
                ? null
                : new AreaDto
                {
                    AreaId = user.Area.Id,
                    Name = user.Area.Name,
                    Level = user.Area.Level
                }
        };
    }
    public async Task<ChangeCustomerAreaResponse> ChangeAreaAsync(
    int customerId,
    ChangeCustomerAreaRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == customerId);

        if (user == null)
            throw new Exception("المستخدم غير موجود.");

        var areaExists = await _context.Areas
            .AnyAsync(x => x.Id == request.AreaId);

        if (!areaExists)
            throw new Exception("المنطقة غير موجودة.");

        user.AreaId = request.AreaId;

        await _context.SaveChangesAsync();

        return new ChangeCustomerAreaResponse
        {
            Message = "تم تغيير المنطقة بنجاح."
        };
    }

    public async Task<CreateEmergencyRequestResponse> CreateEmergencyRequestAsync(
    int customerId,
    CreateEmergencyRequestDto request)
    {
        var customerExists = await _context.Users
            .AnyAsync(x =>
                x.Id == customerId &&
                x.Role == UserRole.Customer &&
                x.IsActive);

        if (!customerExists)
            throw new Exception("العميل غير موجود أو غير مفعل.");

        var serviceExists = await _context.Services
            .AnyAsync(x => x.Id == request.ServiceId && x.IsActive);

        if (!serviceExists)
            throw new Exception("الخدمة غير موجودة.");

        var areaExists = await _context.Areas
            .AnyAsync(x => x.Id == request.AreaId);

        if (!areaExists)
            throw new Exception("المنطقة غير موجودة.");

        if (request.ImageUrls.Count > 4)
            throw new Exception("لا يمكن إرفاق أكثر من 4 صور.");

        var availableProvider = await _context.ProviderServices
            .Include(x => x.Provider)
            .Where(x =>
                x.ServiceId == request.ServiceId &&
                x.Provider.IsAvailable)
            .Select(x => x.Provider)
            .FirstOrDefaultAsync();

        var emergencyRequest = new Request
        {
            UserId = customerId,
            ProviderId = availableProvider?.Id,
            ServiceId = request.ServiceId,
            AreaId = request.AreaId,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ImageUrlsJson = request.ImageUrls.Any()
                ? JsonSerializer.Serialize(request.ImageUrls)
                : null,
            Status = availableProvider == null
                ? RequestStatus.Pending
                : RequestStatus.WaitingForStart,
            IsEmergency = true,
            EmergencyExpiresMins = 15,
            ScheduledAt = DateTime.UtcNow
        };

        _context.Requests.Add(emergencyRequest);
        await _context.SaveChangesAsync();

        return new CreateEmergencyRequestResponse
        {
            RequestId = emergencyRequest.Id,
            Status = emergencyRequest.Status.ToString(),
            Message = availableProvider == null
                ? "تم إرسال طلب الطوارئ، وبانتظار توفر مقدم خدمة."
                : "تم إرسال طلب الطوارئ وتوجيهه لأقرب مقدم خدمة متاح."
        };
    }

    
    public async Task<ChangePasswordResponse> ChangePasswordAsync(
    int customerId,
    ChangePasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == customerId);

        if (user == null)
            throw new Exception("المستخدم غير موجود.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword);

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new Exception("كلمة المرور الحالية غير صحيحة.");

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.NewPassword);

        await _context.SaveChangesAsync();

        return new ChangePasswordResponse
        {
            Message = "تم تغيير كلمة المرور بنجاح."
        };
    }
}