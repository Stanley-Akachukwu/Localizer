using System.Globalization;

namespace Lacalizer.Mobile.Converters;

public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Colors.LimeGreen;
    public Color FalseColor { get; set; } = Colors.White;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isPlaying)
            return isPlaying ? TrueColor : FalseColor;

        return FalseColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}