using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Models;


public partial class ReelVideoModel : ObservableObject
{
    public IVideoService VideoService { get; set; }
    public ICommentService CommentService { get; set; }

    public ReelViewModel ParentViewModel { get; set; }
    public ReelVideoModel(string title, string topic, string videoUri, string videoTopicId,
        IVideoService videoService,
        ICommentService commentService,
        ReelViewModel parentViewModel)
    {
        Title = title;
        Topic = topic;
        VideoUri = videoUri;
        VideoTopicId = videoTopicId;

        VideoService = videoService;
        CommentService = commentService;
        ParentViewModel = parentViewModel;

        CommentsPanelTranslationY = 500; // start hidden
    }

    public string Title { get; set; }
    public string Topic { get; set; }
    public string VideoUri { get; set; }
    public string VideoTopicId { get; set; }

    [ObservableProperty]
    private bool isPlaying;


    [ObservableProperty] private int participantsCount;
    [ObservableProperty] private int likesCount;
    [ObservableProperty] private int shareCount;
    [ObservableProperty] private int commentCount;
    
    [ObservableProperty] private bool isCommentsVisible;
    [ObservableProperty] private double commentsPanelTranslationY;

    [ObservableProperty] private string newComment;
    [ObservableProperty] private VideoComment? replyingTo;
    [ObservableProperty]
    private ObservableCollection<VideoComment> comments = new();

    [RelayCommand]
    private async Task IncreaseLikesAsync()
    {
        LikesCount++;
    }

    [RelayCommand]
    private async Task IncreaseParticipantsAsync()
    {
        ParticipantsCount++;
    }

    [RelayCommand]
    private async Task IncreaseShareAsync()
    {
        ShareCount++;
    }

    
    
    [RelayCommand]
    private async Task OpenCommentsAsync()
    {
        await LoadCommentsAsync();
        await ParentViewModel.OpenCommentsCommand.ExecuteAsync(this);
    }

    [RelayCommand]
    private async Task CloseCommentsAsync()
    {
        await ParentViewModel.CloseCommentsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task SendCommentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewComment))
            return;

        var newDto = new VideoComment
        {
            Content = NewComment,
            ParentId = ReplyingTo?.Id.ToString(),
            VideoTopicId = VideoTopicId,
            Depth = ReplyingTo == null ? 0 : ReplyingTo.Depth + 1,
            Children = new ObservableCollection<VideoComment>()
        };

        var created = await CommentService.PostCommentAsync(newDto);

        // Add to UI tree (ObservableCollection ensures refresh)
        if (ReplyingTo == null)
        {
            Comments.Add(created);
        }
        else
        {
            // Must be ObservableCollection, otherwise UI won't update
            ReplyingTo.Children.Add(created);
        }

        CommentCount++;
        NewComment = string.Empty;
        ReplyingTo = null;
    }
    [RelayCommand]
    private void StartReply(VideoComment parent)
    {
        ReplyingTo = parent;
        NewComment = $"@{parent.Author} ";
    }

    private void BuildHierarchy(List<VideoComment> source,string? parentId,ObservableCollection<VideoComment> dest,int depth)
    {
        foreach (var c in source.Where(x => x.ParentId == parentId))
        {
            if (c.Children == null)
                c.Children = new ObservableCollection<VideoComment>();

            c.Depth = depth;
            dest.Add(c);

            // Recursively build children
            BuildHierarchy(source, c.Id, c.Children, depth + 1);
        }
    }
    private async Task LoadCommentsAsync()
    {
        // Sample flat data (from a database or API)
       // var flatComments = await CommentService.GetVideoCommentsAsync(VideoTopicId);
        var flatComments = new List<VideoComment>
        {
            new VideoComment { Id = "1", Author = "Parent 1", Content = "This is a top-level comment.", ParentId = null },
            new VideoComment { Id = "2", Author = "Child 1", Content = "Reply to Parent 1.", ParentId = "1" },
            new VideoComment { Id = "3", Author = "Child 2", Content = "Another reply to Parent 1.", ParentId = "1" },
            new VideoComment { Id = "4", Author = "Parent 2", Content = "This is another top-level comment.", ParentId = null },
            new VideoComment { Id = "5", Author = "Child 3", Content = "Reply to Parent 2.", ParentId = "4"},
            new VideoComment { Id = "6", Author = "Grandchild 1", Content = "Reply to Child 3.", ParentId = "5" }
        };

        // Function to recursively build the hierarchy
        Comments.Clear();
        CommentCount = flatComments.Count;
        BuildHierarchy(flatComments, null, Comments, 0);

        await Task.CompletedTask;
    }
}