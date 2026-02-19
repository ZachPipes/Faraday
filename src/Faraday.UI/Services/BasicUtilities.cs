using System.Data;
using System.IO;

namespace Faraday.UI.Services;

public static class BasicUtilities {
    public static string GetDownloadsFolder() {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    }

    public static T GetNonNullValue<T>(string rawValue) {
        // Remove quotes and trim whitespace
        string cleanedValue = rawValue.Trim('"').Trim();

        if (string.IsNullOrEmpty(cleanedValue)) {
            if (typeof(T) == typeof(int)) {
                return (T)(object)(0);
            }

            if (typeof(T) == typeof(decimal)) {
                return (T)(object)(decimal.Zero);
            }
        }

        // Try to parse the cleaned string into the target type (T)
        if (typeof(T) == typeof(int) && int.TryParse(cleanedValue, out int intResult)) {
            return (T)(object)intResult;
        }
        
        if (typeof(T) == typeof(decimal) && decimal.TryParse(cleanedValue, out decimal decimalResult)) {
            return (T)(object)decimalResult;
        }

        // If parsing fails, return null
        throw new EvaluateException("Cannot convert " + cleanedValue + " to " + typeof(T));
    }
}