using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    public class FlyoutMessageBar : FlyoutBase
    {

        #region Private Fields
        
        private const string CLOSE_INACTIVE_IMAGE_URI = "pack://application:,,,/Neumann.TouchControls;component/Images/Close.png";
        private const string CLOSE_ACTIVE_IMAGE_URI = "pack://application:,,,/Neumann.TouchControls;component/Images/CloseActive.png";
        private Border _border;
        private DoubleAnimation _openAnimation;
        private DoubleAnimation _closeAnimation;

        #endregion

        #region Constructors
        
        public FlyoutMessageBar()
        {
            this.DefaultStyleKey = typeof(FlyoutMessageBar);
            if (!IsInDesignMode)
            {
                this.CloseImage = new BitmapImage(new Uri(CLOSE_INACTIVE_IMAGE_URI));
                this.SizeChanged += this.OnSizeChanged;
                this.LostFocus += this.OnLostFocus;
                var desc = DependencyPropertyDescriptor.FromProperty(FlyoutBase.IsOpenProperty, typeof(FlyoutBase));
                if (desc != null)
                {
                    desc.AddValueChanged(this, (s, e) => this.OnIsOpenChanged(s, e));
                }
            }
            else
            {
                this.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        #endregion

        #region Events

        public event EventHandler Click;
        public event EventHandler CloseButtonClick;

        #endregion

        #region Properties

        #region IsOpen

        private void OnIsOpenChanged(object sender, EventArgs e)
        {
            if (_border == null) return;
            if (this.IsOpen)
            {
                VisualStateManager.GoToElementState(_border, "Open", false);
            }
            else
            {
                VisualStateManager.GoToElementState(_border, "Closed", false);
            }
        }

        #endregion

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register("Alignment", typeof(FlyoutMessageAlignmentType), typeof(FlyoutMessageBar),
            new PropertyMetadata(FlyoutMessageAlignmentType.Top, OnAlignmentPropertyChanged));
        public FlyoutMessageAlignmentType Alignment { get { return (FlyoutMessageAlignmentType)GetValue(AlignmentProperty); } set { SetValue(AlignmentProperty, value); } }

        private static void OnAlignmentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FlyoutMessageBar)sender).OnAlignmentChanged(sender, e);
        }

        private void OnAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsInDesignMode) return;
            var alignment = (FlyoutMessageAlignmentType)e.NewValue;
            this.OnSizeChanged(null, null);
            this.SetTransformationDirection();
        }

        #endregion

        #region CloseImage

        public static readonly DependencyProperty CloseImageProperty =
            DependencyProperty.Register("CloseImage", typeof(ImageSource), typeof(FlyoutMessageBar), new PropertyMetadata(null));
        public ImageSource CloseImage { get { return (ImageSource)GetValue(CloseImageProperty); } set { SetValue(CloseImageProperty, value); } }

        #endregion

        #region ShowCloseButton

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(FlyoutMessageBar), new PropertyMetadata(true));
        public bool ShowCloseButton { get { return (bool)GetValue(ShowCloseButtonProperty); } set { SetValue(ShowCloseButtonProperty, value); } }

        #endregion

        #region CloseOnClick

        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.Register("CloseOnClick", typeof(bool), typeof(FlyoutMessageBar), new PropertyMetadata(false));
        public bool CloseOnClick { get { return (bool)GetValue(CloseOnClickProperty); } set { SetValue(CloseOnClickProperty, value); } }

        #endregion

        #region CloseButtonCommand

        public static readonly DependencyProperty CloseButtonCommandProperty =
            DependencyProperty.Register("CloseButtonCommand", typeof(ICommand), typeof(FlyoutMessageBar), new PropertyMetadata(null));
        public ICommand CloseButtonCommand { get { return (ICommand)GetValue(CloseButtonCommandProperty); } set { SetValue(CloseButtonCommandProperty, value); } }

        #endregion

        #region Command

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FlyoutMessageBar), new PropertyMetadata(null));
        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }

        #endregion

        #region MessageType

        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register("MessageType", typeof(MessageType), typeof(FlyoutMessageBar), new PropertyMetadata(MessageType.None, OnMessageTypePropertyChanged));
        public MessageType MessageType { get { return (MessageType)GetValue(MessageTypeProperty); } set { SetValue(MessageTypeProperty, value); } }

        private static void OnMessageTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(sender)) return;
            ((FlyoutMessageBar)sender).SetGeometry();
        }

        #endregion

        #region SymbolGeometry

        public static readonly DependencyProperty SymbolGeometryProperty =
            DependencyProperty.Register("SymbolGeometry", typeof(Geometry), typeof(FlyoutMessageBar));
        public Geometry SymbolGeometry { get { return (Geometry)GetValue(SymbolGeometryProperty); } set { SetValue(SymbolGeometryProperty, value); } }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            if (IsInDesignMode) return;
            if (this.Header == null)
            {
                var dictionary = Application.LoadComponent(new Uri("/Neumann.TouchControls;component/FlyoutMessageBar.xaml", UriKind.Relative)) as ResourceDictionary;
                if (dictionary != null && dictionary.Contains("FlyoutMessageBarDefaultHeader"))
                {
                    this.Header = dictionary["FlyoutMessageBarDefaultHeader"];
                }
            }
            var closeImage = this.GetTemplateChild("PART_CloseImage") as Image;
            if (closeImage != null)
            {
                closeImage.AddHandler(Image.MouseEnterEvent, new MouseEventHandler(this.OnCloseMouseEnter));
                closeImage.AddHandler(Image.MouseLeaveEvent, new MouseEventHandler(this.OnCloseMouseLeave));
                closeImage.AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnCloseMouseDown));
                closeImage.AddHandler(Image.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnCloseMouseUp));
            }
            _border = this.GetTemplateChild("border") as Border;
            if (_border != null)
            {
                _border.AddHandler(Border.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseUp));
            }
            _openAnimation = this.GetTemplateChild("openAnimation") as DoubleAnimation;
            _closeAnimation = this.GetTemplateChild("closeAnimation") as DoubleAnimation;
            this.SetTransformationDirection();
            this.SetGeometry();
        }

        #endregion
        
        #region Private Functions

        private TranslateTransform GetTransformation()
        {
            double yOpen = 0;
            double yClose = 0;
            this.CalculateTransformationY(ref yOpen, ref yClose);
            TranslateTransform transformation = null;
            transformation = new TranslateTransform(0, this.IsOpen ? yOpen : yClose);
            return transformation;
        }

        private void CalculateTransformationY(ref double yOpen, ref double yClosed)
        {
            switch (this.Alignment)
            {
                case FlyoutMessageAlignmentType.Top:
                    yOpen = 0;
                    yClosed = (this.RenderSize.Height + 15) * -1;
                    this.VerticalAlignment = VerticalAlignment.Top;
                    this.BorderThickness = new Thickness(0, 0, 0, 1);
                    break;
                case FlyoutMessageAlignmentType.Bottom:
                    yOpen = 0;
                    yClosed = this.RenderSize.Height + 15;
                    this.VerticalAlignment = VerticalAlignment.Bottom;
                    this.BorderThickness = new Thickness(0, 1, 0, 0);
                    break;
            }
        }

        private void SetTransformationDirection()
        {
            if (_openAnimation == null || _closeAnimation == null) return;
            _openAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.Y"));
            _closeAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("RenderTransform.Y"));
        }

        private void SetGeometry()
        {
            if (this.IsInDesignMode) return;
            if (this.MessageType == MessageType.Custom) return;
            var dictionary = Application.LoadComponent(new Uri("/Neumann.TouchControls;component/FlyoutMessageBar.xaml", UriKind.Relative)) as ResourceDictionary;
            if (dictionary != null && dictionary.Contains("FlyoutMessageBarDefaultHeader"))
            {
                var defaultHeader = dictionary["FlyoutMessageBarDefaultHeader"] as Grid;
                if (defaultHeader != null && this.Header is Grid && ((Grid)this.Header).Children.Count > 0)
                {
                    var symbolPath = ((Grid)this.Header).Children[0] as Path;
                    if (symbolPath != null)
                    {
                        var geometryName = "";
                        var brush = new SolidColorBrush(Colors.Transparent);
                        switch (this.MessageType)
                        {
                            case MessageType.Information: geometryName = "FlyoutMessageInformationGeometry"; brush = new SolidColorBrush(Color.FromRgb(32, 78, 126)); break;
                            case MessageType.Warning: geometryName = "FlyoutMessageWarningGeometry"; brush = new SolidColorBrush(Color.FromRgb(255, 137, 0)); break;
                            case MessageType.Error: geometryName = "FlyoutMessageErrorGeometry"; brush = new SolidColorBrush(Color.FromRgb(183, 16, 0)); break;
                            case MessageType.None: this.Header = null; return;
                            case MessageType.Custom: return;
                        }
                        dictionary = Application.LoadComponent(new Uri("/Neumann.TouchControls;component/FlyoutMessageBar.xaml", UriKind.Relative)) as ResourceDictionary;
                        if (dictionary != null && dictionary.Contains(geometryName))
                        {
                            symbolPath.Data = dictionary[geometryName] as Geometry;
                            symbolPath.Fill = brush;
                        }
                    }
                }
            }
        }

        private bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(this); }
        }

        #endregion

        #region Event Handling

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.StaysOpen)
                this.IsOpen = false;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_openAnimation != null && _closeAnimation != null)
            {
                _border.RenderTransform = this.GetTransformation();
                double yOpen = 0;
                double yClose = 0;
                this.CalculateTransformationY(ref yOpen, ref yClose);
                _openAnimation.To = yOpen;
                _closeAnimation.To = yClose;
            }
        }

        private void OnCloseMouseEnter(object sender, MouseEventArgs e)
        {
            this.CloseImage = new BitmapImage(new Uri(CLOSE_ACTIVE_IMAGE_URI));
        }

        private void OnCloseMouseLeave(object sender, MouseEventArgs e)
        {
            this.CloseImage = new BitmapImage(new Uri(CLOSE_INACTIVE_IMAGE_URI));
        }

        private void OnCloseMouseDown(object sender, MouseEventArgs e)
        {
            this.CloseImage = new BitmapImage(new Uri(CLOSE_INACTIVE_IMAGE_URI));
        }

        private void OnCloseMouseUp(object sender, MouseEventArgs e)
        {
            this.CloseImage = new BitmapImage(new Uri(CLOSE_ACTIVE_IMAGE_URI));
            this.IsOpen = false;
            e.Handled = true;
            if (CloseButtonClick != null)
                CloseButtonClick(this, EventArgs.Empty);
            if (this.CloseButtonCommand != null)
                this.CloseButtonCommand.Execute(null);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (this.CloseOnClick)
                this.IsOpen = false;
            else if (Click != null)
                Click(this, EventArgs.Empty);
            if (this.Command != null)
                this.Command.Execute(null);
        }

        #endregion
        
    }

    public enum FlyoutMessageAlignmentType
    {
        Top,
        Bottom
    }

    public enum MessageType
    {
        None,
        Information,
        Warning,
        Error,
        Custom
    }
}
