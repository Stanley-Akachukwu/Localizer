using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacalizer.WebAPI.Entites;

public abstract class BaseEntityConfiguration<TEntity, IdType> : IEntityTypeConfiguration<TEntity>
       where TEntity : BaseEntity<IdType>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> entity)
    {
        entity.UseTpcMappingStrategy();

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnType("text").HasMaxLength(40);

        entity.HasQueryFilter(t => t.IsDeleted == false);

        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("text")
            .HasMaxLength(255);
    }


}