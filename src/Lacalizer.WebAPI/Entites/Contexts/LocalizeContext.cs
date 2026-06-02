namespace Lacalizer.WebAPI.Entites.Contexts;

public class LocalizeContext : BaseEntity<string>
{
    public LocalizeContext()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }
    public string ContextText { get; set; } = string.Empty;
}
