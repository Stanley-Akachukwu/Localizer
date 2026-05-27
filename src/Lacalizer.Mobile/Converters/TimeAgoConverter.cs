using System.Globalization;

namespace Lacalizer.Mobile.Converters;

public class TimeAgoConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not DateTime time)
            return string.Empty;

        var span = DateTime.UtcNow - time;

        if (span.TotalSeconds < 60)
            return $"{(int)span.TotalSeconds}s";
        if (span.TotalMinutes < 60)
            return $"{(int)span.TotalMinutes}m";
        if (span.TotalHours < 24)
            return $"{(int)span.TotalHours}h";
        if (span.TotalDays < 7)
            return $"{(int)span.TotalDays}d";

        return time.ToString("MMM dd", culture);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        // One-way binding only
        throw new NotSupportedException();
    }
}