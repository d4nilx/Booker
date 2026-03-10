using System.Globalization;

namespace Booker.Converters;

/// <summary>
/// Returns true when a string is non-null and non-empty.
/// Used in XAML to show labels/elements only when a field has a value (e.g. Category).
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
