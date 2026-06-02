using Lacalizer.WebAPI.Entites;
using Lacalizer.WebAPI.Entites.Contexts;
using Lacalizer.WebAPI.Entites.Helpers.Converters;
using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Entites.Videos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace Lacalizer.WebAPI.Infrastructure;

public class LocalizeDbContext(DbContextOptions<LocalizeDbContext> options): IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<VideoItem> VideoItems { get; set; }
    public DbSet<VideoTopic> VideoTopics { get; set; }
    public DbSet<LocalizeContext> LocalizeContexts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Comment>().ToTable("Comments", DbSchemaConstants.COMMENT);
        modelBuilder.Entity<VideoItem>().ToTable("VideoItems", DbSchemaConstants.VIDEO);
        modelBuilder.Entity<VideoTopic>().ToTable("VideoTopics", DbSchemaConstants.VIDEO);
        modelBuilder.Entity<LocalizeContext>().ToTable("LocalizeContexts", DbSchemaConstants.VIDEO);

        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", DbSchemaConstants.AUTH);
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", DbSchemaConstants.AUTH);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(new ValueConverter<DateTimeOffset, DateTimeOffset>(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v.UtcDateTime, DateTimeKind.Utc)));
                }

                if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(new ValueConverter<DateTimeOffset?, DateTimeOffset?>(
                        v => v.HasValue ? v.Value.ToUniversalTime() : null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value.UtcDateTime, DateTimeKind.Utc) : null));
                }
            }
        }
    }


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
    .Properties<Ulid>()
    .HaveConversion<UlidToStringConverter>();

        configurationBuilder
            .Properties<Ulid?>()
            .HaveConversion<NullableUlidToStringConverter>();
    }
}
