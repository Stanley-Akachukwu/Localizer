namespace Lacalizer.Mobile.Models;

public class ReelVideoGroupModel
{
    public ReelVideoModel? TopLeft { get; set; }
    public ReelVideoModel? TopRight { get; set; }
    public ReelVideoModel? BottomLeft { get; set; }
    public ReelVideoModel? BottomRight { get; set; }
    public string? CenterText { get; set; }
    public bool IsPlayingSequence { get; set; }
}
