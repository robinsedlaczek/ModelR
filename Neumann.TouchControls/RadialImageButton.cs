using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    public class RadialImageButton : Control
    {

        #region Private Fields

        #endregion

        #region Constructors

        public RadialImageButton()
        {
            this.DefaultStyleKey = typeof(RadialImageButton);
            var descriptor = DependencyPropertyDescriptor.FromProperty(RadialImageButton.IsEnabledProperty, typeof(RadialImageButton));
            if (descriptor != null)
            {
                descriptor.AddValueChanged(this, this.ToggleIsEnabled);
            }
            this.Loaded += this.OnLoaded;
            this.AddHandler(RadialImageButton.MouseEnterEvent, new MouseEventHandler(this.OnMouseEnter));
            this.AddHandler(RadialImageButton.MouseLeaveEvent, new MouseEventHandler(this.OnMouseLeave));
            this.AddHandler(RadialImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp));
        }

        #endregion

        #region Properties

        #region ImageSource

        public ImageSource ImageSource { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadialImageButton),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        #region Click

        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ToggleIsEnabled(sender, e);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseEnter", false);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseLeave", false);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseLeave", false);
            this.OnClick();
        }

        private void ToggleIsEnabled(object sender, EventArgs e)
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", false);
            }
        }

        #endregion

    }
}
