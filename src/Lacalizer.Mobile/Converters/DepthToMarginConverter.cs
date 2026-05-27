using System.Globalization;

namespace Lacalizer.Mobile.Converters;

public class DepthToMarginConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int depth = (int)value;
        return depth * 20; // indent 20px per level
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

