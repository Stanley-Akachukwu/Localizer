using Lacalizer.WebAPI.Entites;
using Lacalizer.WebAPI.Entites.Helpers.Converters;
using Lacalizer.WebAPI.Entites.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace Lacalizer.WebAPI.Infrastructure;

public class LocalizeContext(DbContextOptions<LocalizeContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<VideoItem> VideoItems { get; set; }
    public DbSet<VideoTopic> VideoTopics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Comment>().ToTable("Comments", DbSchemaConstants.COMMENT);
        modelBuilder.Entity<VideoItem>().ToTable("VideoItems", DbSchemaConstants.VIDEO);
        modelBuilder.Entity<VideoTopic>().ToTable("VideoTopics", DbSchemaConstants.VIDEO);

        //modelBuilder.Entity<Domain.UpdashOrders.Order>()
        //    .HasOne(c => c.Amount)
        //    .WithOne(a => a.Order)
        //    .HasForeignKey<Domain.UpdashOrders.Order>(c => c.AmountId);


        //  modelBuilder.ApplyConfiguration(new UpDashUserConfiguration());



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
