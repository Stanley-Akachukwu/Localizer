using System.ComponentModel.DataAnnotations;

namespace Lacalizer.Mobile.Entites;
 
public abstract class BaseEntity<T>
{
    [Key]
    public T Id { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }
    [MaxLength(250)]
    public string? UID { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(128)]
    public string? CreatedByUserId { get; set; }

    public DateTimeOffset? DateCreated { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(128)]
    public string? UpdatedByUserId { get; set; }

    public DateTimeOffset? DateUpdated { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(128)]
    public string? DeletedByUserId { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTimeOffset? DateDeleted { get; set; }

    [MaxLength(150)]
    public string? Tags { get; set; }

}
