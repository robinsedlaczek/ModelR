using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    internal static class ElementHelpers
    {
        public static T GetVisualChild<T>(Visual referenceVisual) where T : Visual
        {
            Visual child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;
                if (child != null && (child.GetType() == typeof(T)))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetVisualChild<T>(child);
                    if (child != null && (child.GetType() == typeof(T)))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

        public static T GetVisualChild<T>(Visual referenceVisual, string name) where T : Visual
        {
            Visual child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;
                if (child != null && child.GetType() == typeof(T) && child is Control && ((Control)child).Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetVisualChild<T>(child, name);
                    if (child != null && child.GetType() == typeof(T) && child is Control && ((Control)child).Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

        public static T GetParent<T>(DependencyObject element) where T : DependencyObject
        {
            if (element != null)
            {
                if (typeof(T).IsAssignableFrom(element.GetType()))
                {
                    return (T)element;
                }
                else
                {
                    DependencyObject parent = LogicalTreeHelper.GetParent(element);
                    DependencyObject result = GetParent<T>(parent);
                    if (result != null)
                        return (T)result;
                }
            }
            return default(T);
        }
    }
}
