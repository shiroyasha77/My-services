using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Myservices.Application.Features.Auth.Dtos;
using Myservices.Application.Features.Auth.Interfaces;
using Myservices.Domain.Entities;
using MyServices.Domain.Entities;
using MyServices.Domain.Enums;
using MyServices.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace Myservices.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;

    public AuthService(
       ApplicationDbContext context,
       IEmailService emailService,
       IJwtService jwtService)
    {
        _context = context;
        _emailService = emailService;
        _jwtService = jwtService;
        _passwordHasher = new PasswordHasher<User>();
    }


    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (request == null)
            throw new Exception("البيانات المرسلة غير صحيحة.");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new Exception("الاسم الكامل مطلوب.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new Exception("الإيميل مطلوب.");

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new Exception("رقم الجوال مطلوب.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new Exception("كلمة المرور مطلوبة.");

        if (string.IsNullOrWhiteSpace(request.Role))
            throw new Exception("نوع الحساب مطلوب.");

        var fullName = request.FullName.Trim();
        var email = request.Email.Trim().ToLower();
        var phone = request.PhoneNumber.Trim();
        var password = request.Password.Trim();
        var roleValue = request.Role.Trim();

        var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
        if (!emailValidator.IsValid(email))
            throw new Exception("صيغة الإيميل غير صحيحة.");

        if (!Enum.TryParse<UserRole>(roleValue, true, out var role))
            throw new Exception("نوع الحساب غير صحيح.");

        if (role == UserRole.Admin)
            throw new Exception("لا يمكن إنشاء حساب أدمن من هذه الواجهة.");

        if (await _context.Users.AnyAsync(x => x.Email == email))
            throw new Exception("البريد مستخدم من قبل.");

        if (await _context.Users.AnyAsync(x => x.PhoneNumber == phone))
            throw new Exception("رقم الجوال مستخدم من قبل.");

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phone,
            Role = role,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (role == UserRole.Provider)
        {
            _context.Providers.Add(new Provider
            {
                UserId = user.Id,
                IsAvailable = true
            });

            await _context.SaveChangesAsync();
        }

        return new RegisterResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            Message = "تم إنشاء الحساب بنجاح"
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (request == null)
            throw new Exception("البيانات المرسلة غير صحيحة.");

        if (string.IsNullOrWhiteSpace(request.Login))
            throw new Exception("البريد الإلكتروني أو رقم الجوال مطلوب.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new Exception("كلمة المرور مطلوبة.");

        if (string.IsNullOrWhiteSpace(request.Role))
            throw new Exception("نوع الحساب مطلوب.");

        var login = request.Login.Trim();
        var password = request.Password.Trim();
        var roleValue = request.Role.Trim();

        if (login.Contains("@"))
        {
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailValidator.IsValid(login))
                throw new Exception("صيغة الإيميل غير صحيحة.");
        }

        if (!Enum.TryParse<UserRole>(roleValue, true, out var parsedRole))
            throw new Exception("نوع الحساب غير صحيح.");

        var loginValue = login.ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == loginValue ||
                x.PhoneNumber == login);

        if (user == null)
            throw new Exception("بيانات الدخول غير صحيحة.");

        if (!user.IsActive)
            throw new Exception("الحساب غير مفعل.");

        if (user.Role != parsedRole)
            throw new Exception("نوع الحساب المختار لا يطابق هذا المستخدم.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new Exception("بيانات الدخول غير صحيحة.");

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            Message = "تم تسجيل الدخول بنجاح",
            Token = token
        };
    }



    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var login = request.Login.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == login ||
                x.PhoneNumber == request.Login.Trim());

        if (user == null)
            throw new Exception("المستخدم غير موجود");

        // 🔥 إلغاء أي OTP قديم
        var oldOtps = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .ToListAsync();

        foreach (var old in oldOtps)
        {
            old.IsUsed = true;
        }

        // 🔥 إنشاء OTP جديد
        var code = new Random().Next(1000, 9999).ToString();

        var otp = new PasswordResetOtp
        {
            UserId = user.Id,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            IsVerified = false,
            IsUsed = false
        };

        _context.PasswordResetOtps.Add(otp);
        await _context.SaveChangesAsync();

        // 📧 إرسال إيميل
        var subject = "رمز إعادة تعيين كلمة المرور";
        var body = $@"
            <div style='direction:rtl; text-align:right;'>
                <h2>رمز التحقق</h2>
                <h1>{code}</h1>
                <p>صالح لمدة دقيقة واحدة</p>
            </div>";

        await _emailService.SendEmailAsync(user.Email, subject, body);

        return new ForgotPasswordResponse
        {
            Message = "تم إرسال رمز التحقق إلى الإيميل",
            Code = code // فقط للتجربة
        };
    }


    public async Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request)
    {
        var login = request.Login.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == login ||
                x.PhoneNumber == request.Login.Trim());

        if (user == null)
            throw new Exception("المستخدم غير موجود");

        var lastOtp = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastOtp != null && lastOtp.ExpiresAt > DateTime.UtcNow)
            throw new Exception("انتظر انتهاء الوقت قبل إعادة الإرسال");

        // 🔥 إلغاء OTP القديم
        var oldOtps = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .ToListAsync();

        foreach (var old in oldOtps)
        {
            old.IsUsed = true;
        }

        // 🔥 إنشاء OTP جديد
        var newCode = new Random().Next(1000, 9999).ToString();

        var otp = new PasswordResetOtp
        {
            UserId = user.Id,
            Code = newCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            IsVerified = false,
            IsUsed = false
        };

        _context.PasswordResetOtps.Add(otp);
        await _context.SaveChangesAsync();

        // 📧 إرسال الإيميل
        await _emailService.SendEmailAsync(
            user.Email,
            "رمز جديد",
            $"رمزك الجديد هو: {newCode}");

        return new ResendOtpResponse
        {
            Message = "تم إعادة إرسال الرمز"
        };
    }



    public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            throw new Exception("البريد الإلكتروني أو رقم الجوال مطلوب.");

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new Exception("رمز التحقق مطلوب.");

        var loginValue = request.Login.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == loginValue ||
                x.PhoneNumber == request.Login.Trim());

        if (user is null)
            throw new Exception("المستخدم غير موجود.");

        var otp = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null)
            throw new Exception("لا يوجد رمز تحقق صالح.");

        if (otp.ExpiresAt < DateTime.UtcNow)
            throw new Exception("انتهت صلاحية رمز التحقق.");

        if (otp.Code != request.Code.Trim())
            throw new Exception("رمز التحقق غير صحيح.");

        otp.IsVerified = true;

        await _context.SaveChangesAsync();

        return new VerifyOtpResponse
        {
            Message = "تم التحقق من الرمز بنجاح.",
            IsVerified = true
        };
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            throw new Exception("البريد الإلكتروني أو رقم الجوال مطلوب.");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new Exception("كلمة المرور الجديدة مطلوبة.");

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
            throw new Exception("تأكيد كلمة المرور مطلوب.");

        if (request.NewPassword != request.ConfirmPassword)
            throw new Exception("كلمة المرور وتأكيدها غير متطابقين.");

        var loginValue = request.Login.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == loginValue ||
                x.PhoneNumber == request.Login.Trim());

        if (user is null)
            throw new Exception("المستخدم غير موجود.");

        var otp = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && x.IsVerified && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null)
            throw new Exception("يجب التحقق من رمز الاستعادة أولاً.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        otp.IsUsed = true;

        await _context.SaveChangesAsync();

        return new ResetPasswordResponse
        {
            Message = "تم تغيير كلمة المرور بنجاح."
        };
    }
}