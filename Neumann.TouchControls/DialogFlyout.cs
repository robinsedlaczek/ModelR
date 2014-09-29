using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    [ContentProperty("Content")]
    public class DialogFlyout : Popup
    {

        #region Private Fields

        private Panel _panel;

        #endregion

        #region Constructors

        public DialogFlyout()
        {
            this.Placement = PlacementMode.Bottom;
            this.PopupAnimation = PopupAnimation.Slide;
            this.AllowsTransparency = true;
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Background

        public Brush Background { get { return (Brush)GetValue(BackgroundProperty); } set { SetValue(BackgroundProperty, value); } }
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region Foreground

        public Brush Foreground { get { return (Brush)GetValue(ForegroundProperty); } set { SetValue(ForegroundProperty, value); } }
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region FontSize

        public double FontSize { get { return (double)GetValue(FontSizeProperty); } set { SetValue(FontSizeProperty, value); } }
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(DialogFlyout),
            new PropertyMetadata(16d));

        #endregion

        #region FontFamily

        public FontFamily FontFamily { get { return (FontFamily)GetValue(FontFamilyProperty); } set { SetValue(FontFamilyProperty, value); } }
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(DialogFlyout),
            new PropertyMetadata(new FontFamily()));

        #endregion

        #region BorderBrush

        public Brush BorderBrush { get { return (Brush)GetValue(BorderBrushProperty); } set { SetValue(BorderBrushProperty, value); } }
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region BorderThickness

        public Thickness BorderThickness { get { return (Thickness)GetValue(BorderThicknessProperty); } set { SetValue(BorderThicknessProperty, value); } }
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DialogFlyout),
            new PropertyMetadata(new Thickness(1)));

        #endregion

        #region Padding

        public Thickness Padding { get { return (Thickness)GetValue(PaddingProperty); } set { SetValue(PaddingProperty, value); } }
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(DialogFlyout),
            new PropertyMetadata(new Thickness(10)));

        #endregion

        #region ContentBackground

        private Brush ContentBackground { get { return (Brush)GetValue(ContentBackgroundProperty); } set { SetValue(ContentBackgroundProperty, value); } }
        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region Header

        public object Header { get { return (object)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate { get { return (DataTemplate)GetValue(HeaderTemplateProperty); } set { SetValue(HeaderTemplateProperty, value); } }
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region Content

        public object Content { get { return (object)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region ContentTemplate

        public DataTemplate ContentTemplate { get { return (DataTemplate)GetValue(ContentTemplateProperty); } set { SetValue(ContentTemplateProperty, value); } }
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DialogFlyout),
            new PropertyMetadata(null));

        #endregion

        #region OpenButton

        public Button OpenButton { get { return (Button)GetValue(OpenButtonProperty); } set { SetValue(OpenButtonProperty, value); } }
        public static readonly DependencyProperty OpenButtonProperty =
            DependencyProperty.Register("OpenButton", typeof(Button), typeof(DialogFlyout),
            new PropertyMetadata(null, OnOpenButtonPropertyChanged));

        private static void OnOpenButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as DialogFlyout;
            var button = e.NewValue as Button;
            if (button != null)
            {
                button.AddHandler(Button.ClickEvent, new RoutedEventHandler(element.OnOpenButtonClicked));
                element.PlacementTarget = button;
            }
            button = e.OldValue as Button;
            if (button != null)
                button.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(element.OnOpenButtonClicked));
        }

        private void OnOpenButtonClicked(object sender, RoutedEventArgs e)
        {
            this.IsOpen = true;
        }

        #endregion

        #region AcceptButton

        public Button AcceptButton { get { return (Button)GetValue(AcceptButtonProperty); } set { SetValue(AcceptButtonProperty, value); } }
        public static readonly DependencyProperty AcceptButtonProperty =
            DependencyProperty.Register("AcceptButton", typeof(Button), typeof(DialogFlyout),
            new PropertyMetadata(null, OnAcceptButtonPropertyChanged));

        private static void OnAcceptButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as DialogFlyout;
            var button = e.NewValue as Button;
            if (button != null)
            {
                button.AddHandler(Button.ClickEvent, new RoutedEventHandler(element.OnAcceptButtonClicked));
            }
            button = e.OldValue as Button;
            if (button != null)
                button.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(element.OnAcceptButtonClicked));
        }

        private void OnAcceptButtonClicked(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        #endregion

        #region CancelButton

        public Button CancelButton { get { return (Button)GetValue(CancelButtonProperty); } set { SetValue(CancelButtonProperty, value); } }
        public static readonly DependencyProperty CancelButtonProperty =
            DependencyProperty.Register("CancelButton", typeof(Button), typeof(DialogFlyout),
            new PropertyMetadata(null, OnAcceptButtonPropertyChanged));

        #endregion

        #endregion

        #region Private Methods

        private void CreateHeaderTemplate()
        {
            var rootFactory = new FrameworkElementFactory(typeof(Grid));
            rootFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            rootFactory.SetValue(Border.MarginProperty, new Thickness(0, 16, 0, 0));

            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            factory.SetValue(Border.MarginProperty, new Thickness(0, 0, 0, 0));
            factory.SetValue(Border.PaddingProperty, new Thickness(8));
            factory.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            factory.SetValue(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            factory.SetBinding(Border.BackgroundProperty, new Binding("Background") { Source = this });
            factory.SetBinding(TextElement.ForegroundProperty, new Binding("Foreground") { Source = this });
            factory.SetBinding(TextElement.FontSizeProperty, new Binding("FontSize") { Source = this });
            factory.SetBinding(TextElement.FontFamilyProperty, new Binding("FontFamily") { Source = this });

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            var binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("Header")
            };
            contentFactory.SetBinding(ContentPresenter.ContentProperty, binding);
            factory.AppendChild(contentFactory);

            var pathFactory = new FrameworkElementFactory(typeof(Path));
            pathFactory.SetValue(Path.SnapsToDevicePixelsProperty, true);
            pathFactory.SetValue(Path.MarginProperty, new Thickness(0, -16, 0, 0));
            pathFactory.SetValue(Path.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            pathFactory.SetValue(Path.VerticalAlignmentProperty, VerticalAlignment.Top);
            pathFactory.SetBinding(Path.FillProperty, new Binding("Background") { Source = this });
            pathFactory.SetValue(Path.WidthProperty, 32d);
            pathFactory.SetValue(Path.HeightProperty, 16d);
            pathFactory.SetValue(Path.DataProperty, new GeometryConverter().ConvertFrom("M0,16 L16,0 L32,16"));

            rootFactory.AppendChild(factory);
            rootFactory.AppendChild(pathFactory);

            var template = new DataTemplate();
            template.VisualTree = rootFactory;
            this.HeaderTemplate = template;
        }

        private void CreateContentTemplate()
        {
            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            factory.SetValue(Border.MarginProperty, new Thickness(0, 0, 0, 0));
            factory.SetBinding(Border.PaddingProperty, new Binding("Padding") { Source = this });
            factory.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            factory.SetValue(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            factory.SetBinding(Border.BorderBrushProperty, new Binding("BorderBrush") { Source = this });
            factory.SetBinding(Border.BackgroundProperty, new Binding("ContentBackground") { Source = this });
            factory.SetBinding(Border.BorderThicknessProperty, new Binding("BorderThickness") { Source = this });

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            var binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("Content")
            };
            contentFactory.SetBinding(ContentPresenter.ContentProperty, binding);
            factory.AppendChild(contentFactory);

            var template = new DataTemplate();
            template.VisualTree = factory;
            this.ContentTemplate = template;
        }

        private void ArrangePopup()
        {
            if (this.Child != null)
            {
                this.Child.Measure(new Size(1000, 1000));
                var width = this.Child.DesiredSize.Width;
                var targetWidth = this.PlacementTarget.RenderSize.Width;
                this.HorizontalOffset = (targetWidth / 2) - (width / 2);
                this.VerticalOffset = 0d;
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Background == null)
                this.Background = new SolidColorBrush(Color.FromRgb(32, 78, 126));
            if (this.BorderBrush == null)
                this.BorderBrush = new SolidColorBrush(Color.FromRgb(32, 78, 126));
            if (this.Foreground == null)
                this.Foreground = Brushes.White;
            if (this.HeaderTemplate == null)
                this.CreateHeaderTemplate();
            if (this.ContentTemplate == null)
                this.CreateContentTemplate();

            _panel = new DockPanel();
            var headerPresenter = new ContentPresenter()
            {
                Content = this.Header,
                ContentTemplate = this.HeaderTemplate
            };
            headerPresenter.SetValue(DockPanel.DockProperty, Dock.Top);
            _panel.Children.Add(headerPresenter);

            var contentPresenter = new ContentPresenter()
            {
                Content = this.Content,
                ContentTemplate = this.ContentTemplate
            };
            this.ContentBackground = (this.Content is Control) ? ((Control)this.Content).Background : (this.Content is Panel) ? ((Panel)this.Content).Background : null;
            _panel.Children.Add(contentPresenter);
            this.Child = _panel;
            this.ArrangePopup();
        }

        #endregion

    }
}
