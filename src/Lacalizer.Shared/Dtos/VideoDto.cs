namespace Lacalizer.Shared.Dtos;
public class VideoDto
{
    public string? Id { get; set; }
    public string? Language { get; set; }
    public string? ContextText { get; set; }
    public string? VideoUri { get; set; }
    public string? VideoContextId { get; set; }
    public int SavedLikes { get; set; }
    public int SavedComments { get; set; }
    public int SavedShares { get; set; }
    public int SavedParticipants { get; set; }
}

 