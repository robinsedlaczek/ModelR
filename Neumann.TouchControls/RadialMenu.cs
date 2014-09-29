using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Neumann.TouchControls
{
    [ContentProperty("Items")]
    public class RadialMenu : Control
    {

        #region Private Fields

        private Grid _itemsGrid;
        private Panel _expanderGrid;
        private RadialImageButton _centerButton;
        private RadialMenuItem _currentMenuItem;

        #endregion

        #region Constructors

        public RadialMenu()
        {
            this.Items = new RadialMenuItemCollection();
            this.DefaultStyleKey = typeof(RadialMenu);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Items

        public RadialMenuItemCollection Items { get { return (RadialMenuItemCollection)GetValue(ItemsProperty); } set { SetValue(ItemsProperty, value); } }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(RadialMenuItemCollection), typeof(RadialMenu),
            new PropertyMetadata(null));

        #endregion

        #region ImageSource

        public ImageSource ImageSource { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadialMenu),
            new PropertyMetadata(null));

        #endregion

        #region IsOpen

        public bool IsOpen { get { return (bool)GetValue(IsOpenProperty); } set { SetValue(IsOpenProperty, value); } }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadialMenu),
            new PropertyMetadata(false, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenu;
            var value = (bool)e.NewValue;
            if (value)
            {
                VisualStateManager.GoToState(element, "Opened", false);
                foreach (var item in element.Items)
                {
                    item.StartExpandingAnimation();
                }
                element.OnOpened();
            }
            else
            {
                if (element.IsExpanded)
                    element.IsExpanded = false;
                VisualStateManager.GoToState(element, "Closed", false);
                foreach (var item in element.Items)
                {
                    item.StartCollapsingAnimation();
                }
                element.OnClosed();
            }
        }

        #endregion

        #region IsExpanded

        public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } set { SetValue(IsExpandedProperty, value); } }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadialMenu),
            new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenu;
            var value = (bool)e.NewValue;
            if (value)
            {
                VisualStateManager.GoToState(element, "Expanded", false);
                if (element._currentMenuItem != null)
                    element.AddSubItems(element._currentMenuItem);
            }
            else
            {
                VisualStateManager.GoToState(element, "Collapsed", false);
                element.AddItems();
            }
            element.OnMenuItemExpanded();
        }

        #endregion

        #endregion

        #region Events

        #region MenuItemExpanded

        public event EventHandler MenuItemExpanded;
        protected virtual void OnMenuItemExpanded()
        {
            if (MenuItemExpanded != null)
                MenuItemExpanded(this, EventArgs.Empty);
        }

        #endregion

        #region Opened

        public event EventHandler Opened;
        protected virtual void OnOpened()
        {
            if (Opened != null)
                Opened(this, EventArgs.Empty);
        }

        #endregion

        #region Closed

        public event EventHandler Closed;
        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _itemsGrid = this.GetTemplateChild("PART_ItemsGrid") as Grid;
            _expanderGrid = this.GetTemplateChild("PART_ExpanderGrid") as Panel;
            _centerButton = this.GetTemplateChild("PART_CenterButton") as RadialImageButton;
            if (_centerButton != null)
            {
                _centerButton.Click += this.OnCenterButtonClick;
            }
            this.AddItems();
        }

        #endregion

        #region Private Methods

        private void AddItems()
        {
            if (_itemsGrid != null)
            {
                _itemsGrid.Children.Clear();
                var startPos = (8 - this.Items.Count)/2;
                if (this.Items.Count < 8)
                    startPos++;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    var item = this.Items[i];
                    item.RenderTransformOrigin = new Point(0.5, 0.5);
                    item.RenderTransform = new TranslateTransform();
                    item.Position = startPos + i;
                    item.Loaded += (s, e) => item.StartExpandingAnimation();
                    if (item.Parent != null)
                    {
                        var p = item.Parent as Panel;
                        p.Children.Remove(item);
                    }
                    _itemsGrid.Children.Add(item);

                    if (item.HasChildren && _expanderGrid != null)
                    {
                        var expanderButton = new RadialMenuExtensionButton()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Position = startPos + i,
                            MenuItem = item
                        };
                        expanderButton.Click += this.OnExpanderClick;
                        _expanderGrid.Children.Add(expanderButton);
                    }
                }
            }
        }

        private void AddSubItems(RadialMenuItem parent)
        {
            if (_itemsGrid != null && parent != null)
            {
                _itemsGrid.Children.Clear();
                for (int i = 0; i < parent.Items.Count; i++)
                {
                    var item = parent.Items[i];
                    item.RenderTransformOrigin = new Point(0.5, 0.5);
                    item.RenderTransform = new TranslateTransform();
                    item.Position = i;
                    item.Loaded += (s, e) => item.StartExpandingAnimation();
                    if (item.Parent != null)
                    {
                        var p = item.Parent as Panel;
                        p.Children.Remove(item);
                    }
                    _itemsGrid.Children.Add(item);
                }
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        
        private void OnExpanderClick(object sender, EventArgs e)
        {
            var expander = sender as RadialMenuExtensionButton;
            _currentMenuItem = expander.MenuItem;
            this.IsExpanded = !this.IsExpanded;
        }

        private void OnCenterButtonClick(object sender, EventArgs e)
        {
            if (this.IsExpanded)
            {
                this.IsExpanded = !this.IsExpanded;
            }   
            else
            {
                this.IsOpen = !this.IsOpen;
            }
        }
        
        #endregion

    }

    public class RadialMenuItemCollection : Collection<RadialMenuItem>
    {
    }
}
