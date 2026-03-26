using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GenericNotification.Core.Domain.Models;

public partial class Ra02Context : DbContext
{

    public Ra02Context(DbContextOptions<Ra02Context> options, IConfiguration configuration)
         : base(options)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }
    public virtual DbSet<SubscriberFcmToken> SubscriberFcmTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.SubscriberId).HasName("subscribers_pkey");

            entity.ToTable("subscribers");

            entity.HasIndex(e => e.EmailId, "subscribers_email_id_key").IsUnique();

            entity.HasIndex(e => e.MobileNumber, "subscribers_mobile_number_key").IsUnique();

            entity.HasIndex(e => e.SubscriberUid, "subscribers_subscriber_uid_key").IsUnique();

            entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id");
            entity.Property(e => e.AppVersion)
                .HasMaxLength(100)
                .HasColumnName("app_version");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("created_date");
            entity.Property(e => e.DateOfBirth)
                .HasMaxLength(50)
                .HasColumnName("date_of_birth");
            entity.Property(e => e.DeviceInfo)
                .HasMaxLength(100)
                .HasColumnName("device_info");
            entity.Property(e => e.EmailId)
                .HasMaxLength(64)
                .HasColumnName("email_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(128)
                .HasColumnName("full_name");
            entity.Property(e => e.IdDocNumber)
                .HasMaxLength(16)
                .HasColumnName("id_doc_number");
            entity.Property(e => e.IdDocType)
                .HasMaxLength(16)
                .HasColumnName("id_doc_type");
            entity.Property(e => e.IsSmartphoneUser)
                .HasDefaultValue((short)1)
                .HasColumnName("is_smartphone_user");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(16)
                .HasColumnName("mobile_number");
            entity.Property(e => e.NationalId)
                .HasMaxLength(100)
                .HasColumnName("national_id");
            entity.Property(e => e.OsName)
                .HasMaxLength(36)
                .HasColumnName("os_name");
            entity.Property(e => e.OsVersion)
                .HasMaxLength(100)
                .HasColumnName("os_version");
            entity.Property(e => e.SubscriberUid)
                .HasMaxLength(36)
                .HasColumnName("subscriber_uid");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("updated_date");
        });


        modelBuilder.Entity<SubscriberFcmToken>(entity =>
        {
            entity.HasKey(e => e.SubscriberFcmTokenId).HasName("subscriber_fcm_token_pkey");

            entity.ToTable("subscriber_fcm_token");

            entity.HasIndex(e => e.SubscriberUid, "subscriber_fcm_token_un").IsUnique();

            entity.Property(e => e.SubscriberFcmTokenId).HasColumnName("subscriber_fcm_token_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("created_date");
            entity.Property(e => e.FcmToken)
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("fcm_token");
            entity.Property(e => e.SubscriberUid)
                .HasMaxLength(36)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("subscriber_uid");

            entity.HasOne(d => d.SubscriberU).WithOne(p => p.SubscriberFcmToken)
                .HasPrincipalKey<Subscriber>(p => p.SubscriberUid)
                .HasForeignKey<SubscriberFcmToken>(d => d.SubscriberUid)
                .HasConstraintName("subscriber_fcm_token_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
