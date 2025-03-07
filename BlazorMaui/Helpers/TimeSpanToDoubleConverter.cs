using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BlazorMaui.Helpers;

public class TimeSpanToDoubleConverter : IValueConverter
{
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //if (DragingState.GetIsDraging()) return false; // Prevent UI updates when dragging

        if (value is TimeSpan timeSpan )
        {
            return timeSpan.TotalSeconds; // Convert TimeSpan to total seconds as a double
        }
        return 0; // Default to 0 if value is null or not a TimeSpan
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //if (DragingState.GetIsDraging()) return false; // Prevent UI updates when dragging
        if (value is double seconds)
        {
            return TimeSpan.FromSeconds(seconds); // Convert double back to TimeSpan
        }
        return TimeSpan.Zero; // Default to zero if value is null or not a double
    }
}
