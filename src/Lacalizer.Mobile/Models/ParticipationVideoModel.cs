using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Navigation;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.ViewModels;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Models;


public partial class ParticipationVideoModel : ObservableObject
{
    public IVideoService VideoService { get; set; }
    public ICommentService CommentService { get; set; }
    public INavigationService NavigationService { get; set; }
    public ParticipationViewModel ParentViewModel { get; set; }   
    
    public ParticipationVideoModel(string contextText, string videoUri, string videoContextId,
       int savedLikes, int savedComments, int savedShares, int savedParticipants, string videoItemId,
        IVideoService videoService,
       ICommentService commentService,
       INavigationService navigationService,
       ParticipationViewModel parentViewModel)
    {
        ContextText = contextText;
        VideoUri = videoUri;
        VideoContextId = videoContextId;

        VideoService = videoService;
        CommentService = commentService;
        NavigationService = navigationService;
        ParentViewModel = parentViewModel;

        SavedLikes = savedLikes;
        SavedComments = savedComments;
        SavedShares = savedShares;
        SavedParticipants = savedParticipants;
        VideoItemId = videoItemId;
        CommentsPanelTranslationY = 500;
    }
    public string Title { get; set; }
    public string ContextText { get; set; }
    public string VideoUri { get; set; }
    public string VideoContextId { get; set; }
    public string VideoItemId { get; set; }
    public int SavedLikes { get; set; }
    public int SavedComments { get; set; }
    public int SavedShares { get; set; }
    public int SavedParticipants { get; set; }

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
        var addCountResult = await VideoService.SaveLikeAsync(VideoItemId);
        if (addCountResult != null)
        {
            LikesCount = addCountResult.Value;
        }
    }

    [RelayCommand]
    private async Task IncreaseShareAsync()
    {
        ShareCount++;
    }

    [RelayCommand]
    private async Task NavigateToTopicAsync()
    {
        await NavigationService.GoToAsync(Routes.ReelPage);
    }


    [RelayCommand]
    private async Task OpenCommentsAsync()
    {
        await LoadCommentsAsync(VideoItemId);
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
            ContentText = NewComment,
            ParentId = ReplyingTo?.Id.ToString(),
            VideoContextId = VideoContextId,
            Depth = ReplyingTo == null ? 0 : ReplyingTo.Depth + 1,
            VideoId = VideoItemId,
            Author = "CurrentUser", // Replace with actual current user
            Children = new ObservableCollection<VideoComment>()
        };

        var created = await CommentService.PostCommentAsync(newDto);

        if (ReplyingTo == null || string.IsNullOrEmpty(created.Id))
        {
            Comments.Add(created);
        }
        else
        {
            CommentCount++;
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

    private void BuildHierarchy(List<VideoComment> source, string? parentId, ObservableCollection<VideoComment> dest, int depth)
    {
        foreach (var c in source.Where(x => x.ParentId == parentId))
        {
            if (c.Children == null)
                c.Children = new ObservableCollection<VideoComment>();

            c.Depth = depth;
            dest.Add(c);

            BuildHierarchy(source, c.Id, c.Children, depth + 1);
        }
    }
    private async Task LoadCommentsAsync(string videoItemId)
    {
        var flatComments = await CommentService.GetVideoCommentsAsync(1, 100, videoItemId);
        Comments.Clear();
        CommentCount = flatComments.Count;
        BuildHierarchy(flatComments, null, Comments, 0);

        await Task.CompletedTask;
    }
}