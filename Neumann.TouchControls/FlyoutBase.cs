using System.Windows;
using System.Windows.Controls;

namespace Neumann.TouchControls
{
    public abstract class FlyoutBase : HeaderedContentControl
    {

        #region IsOpen
        
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutPanel),
            new PropertyMetadata(false));
        public bool IsOpen { get { return (bool)this.GetValue(IsOpenProperty); } set { this.SetValue(IsOpenProperty, value); } }

        #endregion

        #region StaysOpen

        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register("StaysOpen", typeof(bool), typeof(FlyoutBase), new PropertyMetadata(true));
        public bool StaysOpen { get { return (bool)GetValue(StaysOpenProperty); } set { SetValue(StaysOpenProperty, value); } }

        #endregion
        
    }
}
