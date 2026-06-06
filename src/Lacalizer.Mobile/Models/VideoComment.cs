using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Models;

public partial class VideoComment : ObservableObject
{
    public string? VideoId { get; set; }
    public string? VideoContextId { get; set; }
    public int CommentCount { get; set; }
    public string? Id { get; set; }
    public string? ParentId { get; set; }
    public string? Author { get; set; }
    public string? ContentText { get; set; }
    public ObservableCollection<VideoComment> Children { get; set; } = new();
    public int Depth { get; set; }
    public VideoComment? Parent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Id1 { get; }
    public int? ParentId1 { get; }

    [ObservableProperty]
    private bool isReplying;

    [ObservableProperty]
    private string replyText;

    [ObservableProperty]
    private int commentLikesCount;

    [RelayCommand]
    private async Task IncreaseCommentLikesAsync()
    {
        CommentLikesCount++;
    }
}