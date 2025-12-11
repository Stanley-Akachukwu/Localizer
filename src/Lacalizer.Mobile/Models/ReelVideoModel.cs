using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lacalizer.Mobile.Services.Comments;
using Lacalizer.Mobile.Services.Videos;
using Lacalizer.Mobile.ViewModels;
using Lacalizer.Shared.Dtos;
using System.Collections.ObjectModel;

namespace Lacalizer.Mobile.Models;


public partial class ReelVideoModel : ObservableObject
{
    public IVideoService VideoService { get; set; }
    public ICommentService CommentService { get; set; }

    public ReelViewModel ParentViewModel { get; set; }
    public ReelVideoModel(string title, string topic, string videoUri, string videoTopicId)
    {
        Title = title;
        Topic = topic;
        VideoUri = videoUri;
        VideoTopicId = videoTopicId;
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

    [ObservableProperty]
    private string newComment;

    [ObservableProperty]
    private ObservableCollection<string> comments = new();
    
    [RelayCommand]
    private async Task OpenCommentsAsync()
    {
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
        var commentDto = new VideoCommentDto
        {
          Comment = NewComment,
          VideoTopicId = VideoTopicId
        };
        await CommentService.PostCommentAsync(commentDto);

        Comments.Add(NewComment);
        CommentCount++;

        NewComment = string.Empty;

        await ParentViewModel.CloseCommentsCommand.ExecuteAsync(null);
    }
    
}