using Microsoft.EntityFrameworkCore;
using Myservices.Domain.Entities;
using MyServices.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MyServices.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ProviderArea> ProviderAreas => Set<ProviderArea>();
    public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
    public DbSet<ProviderServiceDay> ProviderServiceDays => Set<ProviderServiceDay>();
    public DbSet<ProviderServiceGallery> ProviderServiceGallery => Set<ProviderServiceGallery>();
    public DbSet<RequestMessage> RequestMessages => Set<RequestMessage>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<PasswordResetOtp> PasswordResetOtps => Set<PasswordResetOtp>();

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureProvider(modelBuilder);
        ConfigureArea(modelBuilder);
        ConfigureService(modelBuilder);
        ConfigureRequest(modelBuilder);
        ConfigureRating(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureProviderArea(modelBuilder);
        ConfigureProviderService(modelBuilder);
        ConfigureProviderServiceDay(modelBuilder);
        ConfigureProviderServiceGallery(modelBuilder);
        ConfigureRequestMessage(modelBuilder);
        ConfigureMessageAttachment(modelBuilder);
        ConfigurePasswordResetOtp(modelBuilder);

    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
            entity.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.PhoneNumber).IsUnique();
            entity.HasOne(x => x.Area)
                  .WithMany()
                  .HasForeignKey(x => x.AreaId)
                  .OnDelete(DeleteBehavior.SetNull);

        });
    }

    private static void ConfigureProvider(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.ToTable("Providers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.IdNumber).HasMaxLength(100);
            entity.Property(x => x.IdImageUrl).HasMaxLength(500);
            entity.Property(x => x.ProfileImageUrl).HasMaxLength(500);

            entity.HasOne(x => x.User)
                .WithOne(x => x.ProviderProfile)
                .HasForeignKey<Provider>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureArea(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Areas");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureService(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Services");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });
    }

    private static void ConfigureRequest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Requests");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.IsEmergency).HasDefaultValue(false);

            entity.Property(x => x.ScheduledAt)
                .IsRequired(false);

            entity.Property(x => x.Latitude)
                 .IsRequired(false);

            entity.Property(x => x.Longitude)
                .IsRequired(false);

            entity.Property(x => x.ImageUrlsJson)
                .HasMaxLength(2000);

            entity.HasOne(x => x.User)
                .WithMany(x => x.CustomerRequests)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.Requests)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Service)
                .WithMany(x => x.Requests)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Area)
                .WithMany(x => x.Requests)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            

        });
    }

    private static void ConfigureRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("Ratings");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Rate).IsRequired();
            entity.Property(x => x.Comment).HasMaxLength(1000);

            entity.HasOne(x => x.Request)
                .WithMany(x => x.Ratings)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Body).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.IsRead).HasDefaultValue(false);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProviderArea(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderArea>(entity =>
        {
            entity.ToTable("ProviderAreas");
            entity.HasKey(x => new { x.ProviderId, x.AreaId });

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.ProviderAreas)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Area)
                .WithMany(x => x.ProviderAreas)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProviderService(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderService>(entity =>
        {
            entity.ToTable("ProviderServices");
            entity.HasKey(x => new { x.ProviderId, x.ServiceId });

            entity.Property(x => x.EducationLevel).HasMaxLength(150);
            entity.Property(x => x.State).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.ProviderServices)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Service)
                .WithMany(x => x.ProviderServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProviderServiceDay(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderServiceDay>(entity =>
        {
            entity.ToTable("ProviderServiceDays");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DayOfWeek).IsRequired().HasMaxLength(20);

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.ProviderServiceDays)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Service)
                .WithMany(x => x.ProviderServiceDays)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProviderServiceGallery(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderServiceGallery>(entity =>
        {
            entity.ToTable("ProviderServiceGallery");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(x => x.FileType).IsRequired().HasMaxLength(100);

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.ProviderServiceGallery)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Service)
                .WithMany(x => x.ProviderServiceGallery)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRequestMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestMessage>(entity =>
        {
            entity.ToTable("RequestMessages");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MessageText)
                .HasMaxLength(2000);

            entity.Property(x => x.SenderType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.IsRead)
                .HasDefaultValue(false);

            entity.HasOne(x => x.Request)
                .WithMany(x => x.MessagesText)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureMessageAttachment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.ToTable("MessageAttachments");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(x => x.FileType).IsRequired().HasMaxLength(100);

            entity.HasOne(x => x.Message)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePasswordResetOtp(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordResetOtp>(entity =>
        {
            entity.ToTable("PasswordResetOtps");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.IsVerified)
                .HasDefaultValue(false);

            entity.Property(x => x.IsUsed)
                .HasDefaultValue(false);

            entity.HasOne(x => x.User)
                .WithMany(x => x.PasswordResetOtps)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


}