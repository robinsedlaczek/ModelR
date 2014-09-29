using System.Windows;
using System.Windows.Controls;

namespace Neumann.TouchControls
{
    public class AnimatableScrollViewer : ScrollViewer
    {
        public double CurrentHorizontalOffset { get { return (double)this.GetValue(CurrentHorizontalOffsetProperty); } set { this.SetValue(CurrentHorizontalOffsetProperty, value); } }
        public static DependencyProperty CurrentHorizontalOffsetProperty =
            DependencyProperty.Register("CurrentHorizontalOffsetOffset", typeof(double), typeof(AnimatableScrollViewer),
            new PropertyMetadata(new PropertyChangedCallback(OnHorizontalChanged)));

        private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public double CurrentVerticalOffset { get { return (double)this.GetValue(CurrentVerticalOffsetProperty); } set { this.SetValue(CurrentVerticalOffsetProperty, value); } }
        public static DependencyProperty CurrentVerticalOffsetProperty =
            DependencyProperty.Register("CurrentVerticalOffset", typeof(double), typeof(AnimatableScrollViewer),
            new PropertyMetadata(new PropertyChangedCallback(OnVerticalChanged)));

        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatableScrollViewer viewer = d as AnimatableScrollViewer;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }
    }
}
