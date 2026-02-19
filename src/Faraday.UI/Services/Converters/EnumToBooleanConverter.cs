using System.Globalization;
using System.Windows.Data;

namespace Faraday.UI.Services.Converters;

/// <summary>
/// Converts between an Enum value and a Boolean for use in bindings,
/// typically for RadioButtons where each button represents a single enum value.
/// </summary>
public class EnumToBooleanConverter : IValueConverter {
    /// <summary>
    /// Converts an enum value to a boolean. 
    /// Returns true if the enum's string value matches the provided parameter.
    /// </summary>
    /// <param name="value">The current enum value bound from the ViewModel.</param>
    /// <param name="targetType">The target binding type (usually bool).</param>
    /// <param name="parameter">The enum value to compare against (provided in XAML).</param>
    /// <param name="culture">Culture information (not used).</param>
    /// <returns>True if the enum matches the parameter; otherwise false.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value == null || parameter == null)
            return false;

        return value.ToString() == parameter.ToString();
    }
    
    /// <summary>
    /// Converts back from a boolean to an enum value.
    /// Returns the corresponding enum value if the boolean is true,
    /// otherwise returns Binding.DoNothing to prevent unwanted updates.
    /// </summary>
    /// <param name="value">The boolean value (from the control, e.g., RadioButton IsChecked).</param>
    /// <param name="targetType">The type of the enum to convert to.</param>
    /// <param name="parameter">The enum name to convert back to.</param>
    /// <param name="culture">Culture information (not used).</param>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (parameter != null)
            return value is bool
                ? Enum.Parse(targetType, parameter.ToString()!) : Binding.DoNothing;
        return null;
    }
}