using System.Globalization;

namespace Booker.Converters;

/// <summary>
/// Flips a bool: true → false, false → true.
/// Used in XAML to show/hide elements that are the inverse of e.g. IsBusy.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}
