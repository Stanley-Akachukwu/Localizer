using Lacalizer.Mobile.Models;
using System.Windows.Input;

namespace Lacalizer.Mobile.Views;


public partial class ThreadedCommentView : ContentView
{
    public ThreadedCommentView()
    {
        InitializeComponent();
    }

    // =========================
    // COMMANDS
    // =========================

    public ICommand ReplyCommand => new Command<VideoComment>(OnReply);

    public ICommand SendReplyCommand => new Command<VideoComment>(OnSendReply);

    // =========================
    // COMMAND HANDLERS
    // =========================

    private void OnReply(VideoComment comment)
    {
        if (comment == null)
            return;

        // Close any open reply editors in the entire tree
        var root = GetRoot(comment);
        CloseAllReplies(root);

        // Open reply editor for selected comment
        comment.IsReplying = true;
    }

    private void OnSendReply(VideoComment parent)
    {
        if (parent == null || string.IsNullOrWhiteSpace(parent.ReplyText))
            return;

        var reply = new VideoComment
        {
            Author = "You",
            ContentText = parent.ReplyText,
            CreatedAt = DateTime.UtcNow,
            Depth = parent.Depth + 1,
            Parent = parent
        };

        parent.Children.Add(reply);

        // Reset UI state
        parent.ReplyText = string.Empty;
        parent.IsReplying = false;
    }

    // =========================
    // TREE HELPERS
    // =========================

    /// <summary>
    /// Walks up the tree to find the root comment
    /// </summary>
    private VideoComment GetRoot(VideoComment comment)
    {
        while (comment.Parent != null)
            comment = comment.Parent;

        return comment;
    }

    /// <summary>
    /// Recursively closes all reply editors in the tree
    /// </summary>
    private void CloseAllReplies(VideoComment node)
    {
        node.IsReplying = false;

        foreach (var child in node.Children)
            CloseAllReplies(child);
    }
}