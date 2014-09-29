using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace Neumann.TouchControls
{
    public class FlyoutPanel : FlyoutBase
    {

        #region Private Fields
        
        private Border _border;
        private Button _closeButton;
        private DoubleAnimation _openAnimation;
        private DoubleAnimation _closeAnimation;

        #endregion

        #region Constructors
        
        public FlyoutPanel()
        {
            this.DefaultStyleKey = typeof(FlyoutPanel);
            this.SizeChanged += this.OnSizeChanged;
            this.LostFocus += this.OnLostFocus;
            var desc = DependencyPropertyDescriptor.FromProperty(FlyoutBase.IsOpenProperty, typeof(FlyoutBase));
            if (desc != null)
            {
                desc.AddValueChanged(this, (s, e) => this.OnIsOpenChanged(s,e));
            }
        }

        #endregion

        #region Properties

        #region IsOpen

        private void OnIsOpenChanged(object sender, EventArgs e)
        {
            if (_border == null) return;
            if (this.IsOpen)
            {
                VisualStateManager.GoToElementState(_border, "Open", false);
                this.Focus();
            }
            else
            {
                VisualStateManager.GoToElementState(_border, "Closed", false);
                if (this.Closed != null)
                    this.Closed(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register("Alignment", typeof(FlyoutAlignmentType), typeof(FlyoutPanel),
            new PropertyMetadata(FlyoutAlignmentType.Left, OnAlignmentPropertyChanged));
        public FlyoutAlignmentType Alignment { get { return (FlyoutAlignmentType)GetValue(AlignmentProperty); } set { SetValue(AlignmentProperty, value); } }

        private static void OnAlignmentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FlyoutPanel)sender).OnAlignmentChanged(sender, e);
        }

        private void OnAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var alignment = (FlyoutAlignmentType)e.NewValue;
            this.OnSizeChanged(null, null);
            this.SetTransformationDirection();
        }

        #endregion

        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(FlyoutPanel));
        public Brush HeaderBackground { get { return (Brush)GetValue(HeaderBackgroundProperty); } set { SetValue(HeaderBackgroundProperty, value); } }

        #endregion

        #region HeaderForeground

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(FlyoutPanel));
        public Brush HeaderForeground { get { return (Brush)GetValue(HeaderForegroundProperty); } set { SetValue(HeaderForegroundProperty, value); } }

        #endregion

        #region HeaderFontFamily

        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(FlyoutPanel));
        public FontFamily HeaderFontFamily { get { return (FontFamily)GetValue(HeaderFontFamilyProperty); } set { SetValue(HeaderFontFamilyProperty, value); } }

        #endregion

        #region HeaderFontSize

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(FlyoutPanel));
        public double HeaderFontSize { get { return (double)GetValue(HeaderFontSizeProperty); } set { SetValue(HeaderFontSizeProperty, value); } }

        #endregion

        #region HeaderFontWeight

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(FlyoutPanel));
        public FontWeight HeaderFontWeight { get { return (FontWeight)GetValue(HeaderFontWeightProperty); } set { SetValue(HeaderFontWeightProperty, value); } }

        #endregion

        #region CloseButtonGeometry

        public static readonly DependencyProperty CloseButtonGeometryProperty =
            DependencyProperty.Register("CloseButtonGeometry", typeof(Geometry), typeof(FlyoutPanel));
        public Geometry CloseButtonGeometry { get { return (Geometry)GetValue(CloseButtonGeometryProperty); } set { SetValue(CloseButtonGeometryProperty, value); } }

        #endregion

        #region ShowCloseButton

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(FlyoutPanel), new PropertyMetadata(true));
        public bool ShowCloseButton { get { return (bool)GetValue(ShowCloseButtonProperty); } set { SetValue(ShowCloseButtonProperty, value); } }

        #endregion

        #region CloseButtonCommand

        public static readonly DependencyProperty CloseButtonCommandProperty =
            DependencyProperty.Register("CloseButtonCommand", typeof(ICommand), typeof(FlyoutPanel), new PropertyMetadata(null));
        public ICommand CloseButtonCommand { get { return (ICommand)GetValue(CloseButtonCommandProperty); } set { SetValue(CloseButtonCommandProperty, value); } }

        #endregion

        #endregion

        #region Events

        public event EventHandler CloseButtonClick;
        public event EventHandler Closed;

        #endregion

        #region Overrides
        
        public override void OnApplyTemplate()
        {
            _border = this.GetTemplateChild("border") as Border;
            _openAnimation = this.GetTemplateChild("openAnimation") as DoubleAnimation;
            _closeAnimation = this.GetTemplateChild("closeAnimation") as DoubleAnimation;
            _closeButton = this.GetTemplateChild("closeButton") as Button;
            if (_closeButton != null)
                _closeButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnCloseButtonClick));
            this.SetTransformationDirection();
        }

        #endregion

        #region Private Functions
        
        private TranslateTransform GetTransformation()
        {
            double xOpen = 0;
            double xClose = 0;
            double yOpen = 0;
            double yClose = 0;
            this.CalculateTransformationX(ref xOpen, ref xClose);
            this.CalculateTransformationY(ref yOpen, ref yClose);

            TranslateTransform transformation = null;
            if (this.Alignment == FlyoutAlignmentType.Left || this.Alignment == FlyoutAlignmentType.Right)
            {
                transformation = new TranslateTransform(this.IsOpen ? xOpen : xClose, 0);
            }
            else
            {
                transformation = new TranslateTransform(0, this.IsOpen ? yOpen : yClose);
            }
            return transformation;
        }

        private void CalculateTransformationX(ref double xOpen, ref double xClosed)
        {
            switch (this.Alignment)
            {
                case FlyoutAlignmentType.Left:
                    xOpen = 0;
                    xClosed = (this.RenderSize.Width + 15) *  -1;
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case FlyoutAlignmentType.Right:
                    xOpen = 0;
                    xClosed = this.RenderSize.Width + 15;
                    this.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
            }
        }

        private void CalculateTransformationY(ref double yOpen, ref double yClosed)
        {
            switch (this.Alignment)
            {
                case FlyoutAlignmentType.Top:
                    yOpen = 0;
                    yClosed = (this.RenderSize.Height + 15) * -1;
                    this.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case FlyoutAlignmentType.Bottom:
                    yOpen = 0;
                    yClosed = this.RenderSize.Height + 15;
                    this.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }
        }

        private void SetTransformationDirection()
        {
            if (_openAnimation == null || _closeAnimation == null) return;
            if (this.Alignment == FlyoutAlignmentType.Left || this.Alignment == FlyoutAlignmentType.Right)
            {
                _openAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.X"));
                _closeAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.X"));
            }
            else
            {
                _openAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.Y"));
                _closeAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.Y"));
            }
        }
        
        private bool IsOver<T>(DependencyObject element)
        {
            if (element == null)
                return false;
            if (element.GetType().Equals(typeof(T)))
                return true;
            var parent = VisualTreeHelper.GetParent(element);
            if (parent != null)
            {
                if (parent.GetType().Equals(typeof(T)))
                    return true;
                else
                    return IsOver<T>(parent);
            }
            return false;
        }

        #endregion

        #region Event Handling

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.StaysOpen)
            {
                if (!this.IsOver<FlyoutPanel>((DependencyObject)Mouse.DirectlyOver))
                    this.IsOpen = false;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_openAnimation != null && _closeAnimation != null)
            {
                _border.RenderTransform = this.GetTransformation();

                double xOpen = 0;
                double xClose = 0;
                double yOpen = 0;
                double yClose = 0;
                this.CalculateTransformationX(ref xOpen, ref xClose);
                this.CalculateTransformationY(ref yOpen, ref yClose);

                if (this.Alignment == FlyoutAlignmentType.Left || this.Alignment == FlyoutAlignmentType.Right)
                {
                    _openAnimation.To = xOpen;
                    _closeAnimation.To = xClose;
                }
                else
                {
                    _openAnimation.To = yOpen;
                    _closeAnimation.To = yClose;
                }
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.IsOpen = !this.IsOpen;
            if (CloseButtonClick != null)
                CloseButtonClick(this, EventArgs.Empty);
        }

        #endregion

    }

    public enum FlyoutAlignmentType
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
