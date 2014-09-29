using System.Windows;
using System.Windows.Controls;

namespace Neumann.TouchControls
{
    public class ColumnViewSection : HeaderedContentControl
    {
        public ColumnViewSection()
        {
            this.DefaultStyleKey = typeof(ColumnViewSection);
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        
        #region IsExpanded

        public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } set { SetValue(IsExpandedProperty, value); } }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ColumnViewSection),
            new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ColumnViewSection;
        }

        #endregion
        
    }
}
