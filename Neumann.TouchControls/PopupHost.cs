using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    public class PopupHost : HeaderedContentControl
    {

        #region Private Fields

        private Popup _popup;
        private Border _border;
        private Path _path;

        #endregion

        #region Constructors

        public PopupHost()
        {
            this.DefaultStyleKey = typeof(PopupHost);
        }

        #endregion

        #region Properties
        
        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(PopupHost));
        public Brush HeaderBackground { get { return (Brush)GetValue(HeaderBackgroundProperty); } set { SetValue(HeaderBackgroundProperty, value); } }

        #endregion

        #region HeaderBorderBrush

        public static readonly DependencyProperty HeaderBorderBrushProperty =
            DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(PopupHost));
        public Brush HeaderBorderBrush { get { return (Brush)GetValue(HeaderBorderBrushProperty); } set { SetValue(HeaderBorderBrushProperty, value); } }

        #endregion

        #region HeaderBorderThickness

        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(PopupHost));
        public Thickness HeaderBorderThickness { get { return (Thickness)GetValue(HeaderBorderThicknessProperty); } set { SetValue(HeaderBorderThicknessProperty, value); } }

        #endregion

        #region HeaderForeground

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(PopupHost));
        public Brush HeaderForeground { get { return (Brush)GetValue(HeaderForegroundProperty); } set { SetValue(HeaderForegroundProperty, value); } }

        #endregion

        #region HeaderFontFamily

        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(PopupHost));
        public FontFamily HeaderFontFamily { get { return (FontFamily)GetValue(HeaderFontFamilyProperty); } set { SetValue(HeaderFontFamilyProperty, value); } }

        #endregion

        #region HeaderFontSize

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(PopupHost));
        public double HeaderFontSize { get { return (double)GetValue(HeaderFontSizeProperty); } set { SetValue(HeaderFontSizeProperty, value); } }

        #endregion

        #region HeaderFontWeight

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(PopupHost));
        public FontWeight HeaderFontWeight { get { return (FontWeight)GetValue(HeaderFontWeightProperty); } set { SetValue(HeaderFontWeightProperty, value); } }

        #endregion

        #region CloseCommand

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(PopupHost), new PropertyMetadata(null));
        public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }
        
        #endregion

        #region CloseButton

        public static readonly DependencyProperty CloseButtonProperty =
            DependencyProperty.Register("CloseButton", typeof(Button), typeof(PopupHost), new PropertyMetadata(null, OnCloseButtonPropertyChanged));
        public Button CloseButton { get { return (Button)GetValue(CloseButtonProperty); } set { SetValue(CloseButtonProperty, value); } }

        private static void OnCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PopupHost)d).OnCloseButtonChanged(d, e);
        }

        private void OnCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = e.NewValue as Button;
            if (button != null)
            {
                button.Click += this.OnCloseButtonClicked;
            }
            else
            {
                button = e.OldValue as Button;
                if (button != null)
                    button.Click -= this.OnCloseButtonClicked;
            }
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_popup != null && _border != null)
            {
                if (_popup.PopupAnimation == PopupAnimation.Slide)
                {
                    _border.RenderTransform = new TranslateTransform(0, 0);
                    var closeAnimation = new DoubleAnimation(this.RenderSize.Height * -1, new Duration(new System.TimeSpan(0, 0, 0, 0, 200)));
                    closeAnimation.SetValue(Storyboard.TargetProperty, _border);
                    closeAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.Y"));
                    var storyboard = new Storyboard();
                    storyboard.Children.Add(closeAnimation);
                    storyboard.Completed += this.OnCloseStoryboardCompleted;
                    storyboard.Begin();
                }
                else
                {
                    _popup.IsOpen = false;
                }
            }
        }

        private void OnCloseStoryboardCompleted(object sender, System.EventArgs e)
        {
            var storyboard = sender as ClockGroup;
            storyboard.Completed -= OnCloseStoryboardCompleted;
            _popup.IsOpen = false;
            _border.RenderTransform = new TranslateTransform(0, 0);
        }

        #endregion

        #region OpenButton

        public static readonly DependencyProperty OpenButtonProperty =
            DependencyProperty.Register("OpenButton", typeof(Button), typeof(PopupHost), new PropertyMetadata(null, OnOpenButtonPropertyChanged));
        public Button OpenButton { get { return (Button)GetValue(OpenButtonProperty); } set { SetValue(OpenButtonProperty, value); } }

        private static void OnOpenButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PopupHost)d).OnOpenButtonChanged(d, e);
        }

        private void OnOpenButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = e.NewValue as Button;
            if (button != null)
            {
                button.Click += this.OnOpenButtonClicked;
            }
            else
            {
                button = e.OldValue as Button;
                if (button != null)
                    button.Click -= this.OnOpenButtonClicked;
            }
        }

        private void OnOpenButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_popup == null)
                _popup = LogicalTreeHelper.GetParent(this) as Popup;
            if (_popup != null)
                _popup.IsOpen = true;
        }

        #endregion

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register("Alignment", typeof(PopupHostAlignment), typeof(PopupHost),
                new PropertyMetadata(PopupHostAlignment.Bottom));
        public PopupHostAlignment Alignment { get { return (PopupHostAlignment)GetValue(AlignmentProperty); } set { SetValue(AlignmentProperty, value); } }

        #endregion

        #endregion

        #region Overrrides

        public override void OnApplyTemplate()
        {
            _popup = LogicalTreeHelper.GetParent(this) as Popup;
            if (_popup != null)
            {
                _popup.Placement = PlacementMode.Relative;
                _popup.Opened += this.OnPopupOpened;
                _popup.Closed += this.OnPopupClosed;
            }
            _border = this.GetTemplateChild("border") as Border;
            _path = this.GetTemplateChild("path") as Path;
            this.AddHandler(PopupHost.SizeChangedEvent, new RoutedEventHandler(this.ArrangePopup));
        }

        #endregion

        #region Private Functions

        private void ArrangePopup(object sender, RoutedEventArgs e)
        {
            if (_path != null && _popup != null)
            {
                var height = this.RenderSize.Height;
                var width = this.RenderSize.Width;
                var targetWidth = _popup.PlacementTarget.RenderSize.Width;
                var targetHeight = _popup.PlacementTarget.RenderSize.Height;
                var offsetX = 0d;
                var offsetY = 0d;
                switch (this.Alignment)
                {
                    case PopupHostAlignment.Top:
                        _path.SetValue(Grid.RowProperty, 2);
                        _path.Margin = new Thickness(0, 0, 0, -16);
                        _border.Margin = new Thickness(0, 0, 0, 15);
                        _path.RenderTransformOrigin = new Point(0.5, 0.5);
                        _path.RenderTransform = new RotateTransform(180);
                        offsetX = (targetWidth / 2) - (width / 2);
                        offsetY = height * -1;
                        break;
                    case PopupHostAlignment.Bottom:
                        _path.SetValue(Grid.RowProperty, 0);
                        _path.Margin = new Thickness(0, -16, 0, 0);
                        _border.Margin = new Thickness(0, 15, 0, 0);
                        _path.RenderTransform = new RotateTransform();
                        offsetX = (targetWidth / 2) - (width / 2);
                        offsetY = targetHeight;
                        break;
                    case PopupHostAlignment.Right:
                        _path.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        _path.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        _path.SetValue(Grid.RowProperty, 0);
                        _path.SetValue(Grid.RowSpanProperty, 3);
                        _path.SetValue(Grid.ColumnProperty, 1);
                        _path.Margin = new Thickness(-25, 0, 0, 0);
                        _border.Margin = new Thickness(15, 0, 0, 0);
                        _path.RenderTransformOrigin = new Point(0.5, 0.5);
                        _path.RenderTransform = new RotateTransform(-90);
                        offsetX = targetWidth;
                        offsetY = (targetHeight - height) / 2;
                        break;
                    case PopupHostAlignment.Left:
                        _path.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        _path.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        _path.SetValue(Grid.RowProperty, 0);
                        _path.SetValue(Grid.RowSpanProperty, 3);
                        _path.SetValue(Grid.ColumnProperty, 1);
                        _path.Margin = new Thickness(0, 0, -25, 0);
                        _border.Margin = new Thickness(0, 0, 15, 0);
                        _path.RenderTransformOrigin = new Point(0.5, 0.5);
                        _path.RenderTransform = new RotateTransform(90);
                        offsetX = width * -1;
                        offsetY = (targetHeight - height) / 2;
                        break;
                }
                _popup.HorizontalOffset = offsetX;
                _popup.VerticalOffset = offsetY;
            }
        }

        #endregion

        #region Event Handling

        private void OnPopupOpened(object sender, System.EventArgs e)
        {
            this.Opacity = 1;
        }

        private void OnPopupClosed(object sender, System.EventArgs e)
        {
            if (this.CloseCommand != null)
                this.CloseCommand.Execute(null);
        }
        
        #endregion

    }

    public enum PopupHostAlignment
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
