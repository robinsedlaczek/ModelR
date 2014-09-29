using System;
using System.Globalization;
using System.Windows.Data;

namespace Neumann.TouchControls
{
    public class FlyoutAlignmentToNavigationDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                FlyoutAlignmentType alignment;
                if (Enum.TryParse<FlyoutAlignmentType>(value.ToString(), out alignment))
                {
                    switch (alignment)
                    {
                        case FlyoutAlignmentType.Top: return NavigationButtonDirection.Up;
                        case FlyoutAlignmentType.Bottom: return NavigationButtonDirection.Down;
                        case FlyoutAlignmentType.Left: return NavigationButtonDirection.Left;
                        case FlyoutAlignmentType.Right: return NavigationButtonDirection.Right;
                    }
                }
            }
            return NavigationButtonDirection.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                NavigationButtonDirection direction;
                if (Enum.TryParse<NavigationButtonDirection>(value.ToString(), out direction))
                {
                    switch (direction)
                    {
                        case NavigationButtonDirection.Up: return FlyoutAlignmentType.Top;
                        case NavigationButtonDirection.Down: return FlyoutAlignmentType.Bottom;
                        case NavigationButtonDirection.Left: return FlyoutAlignmentType.Left;
                        case NavigationButtonDirection.Right: return FlyoutAlignmentType.Right;
                    }
                }
            }
            return FlyoutAlignmentType.Left;
        }
    }
}
