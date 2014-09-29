using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Neumann.TouchControls
{
    [ContentProperty("Items")]
    public class FlipView : Control
    {

        #region Private Fields

        private Panel _panel;
        private AnimatableScrollViewer _scrollViewer;
        private NavigationButton _leftButton;
        private NavigationButton _rightButton;
        private BreadcrumbBulletBar _breadcrumbBar;
        private StylusPointCollection _downPoints;
        private double _elementWidth;
        private bool _isUpdating;

        #endregion

        #region Constructors

        public FlipView()
        {
            this.Items = new FlipViewItemCollection();
            this.DefaultStyleKey = typeof(FlipView);
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties

        #region Items

        public FlipViewItemCollection Items { get { return (FlipViewItemCollection)GetValue(ItemsProperty); } set { SetValue(ItemsProperty, value); } }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(FlipViewItemCollection), typeof(FlipView),
            new PropertyMetadata(null));

        #endregion

        #region SelectedIndex

        public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FlipView),
            new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipView;
            if (element._isUpdating) return;
            var index = (int)e.NewValue;
            var oldIndex = (int)e.OldValue;
            if (index >= element.Items.Count) return;
            element._isUpdating = true;
            element.SelectedItem = index >= 0 ? element.Items[index] : null;
            element.CanGoBack = index > 0;
            element.CanGoForeward = (element.Items.Count > 0) ? index < element.Items.Count - 1 : false;
            if (element._breadcrumbBar != null && element.ShowBreadcrumbBar)
            {
                element._breadcrumbBar._isUpdating = true;
                element._breadcrumbBar.SelectedIndex = index;
                element._breadcrumbBar._isUpdating = false;
            }
            
            double x;
            if (oldIndex < index)
            {
                x = element._scrollViewer.ContentHorizontalOffset + ((index-oldIndex) * element._elementWidth);
            }
            else
            {
                x = element._scrollViewer.ContentHorizontalOffset - ((oldIndex-index) * element._elementWidth);
            }
            element.AnimateSelectionChanged(x);
            element._isUpdating = false;
        }

        #endregion

        #region SelectedItem

        public object SelectedItem { get { return (object)GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(FlipView),
            new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipView;
            var item = e.NewValue as UIElement;
            if (item != null)
            {
                if (!element._isUpdating)
                    element.SelectedIndex = element.Items.IndexOf(item);
                ((FlipView)d).OnSelectedItemChanged();
            }
        }

        #endregion

        #region CanGoBack

        public bool CanGoBack { get { return (bool)GetValue(CanGoBackProperty); } set { SetValue(CanGoBackProperty, value); } }
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof(bool), typeof(FlipView),
            new PropertyMetadata(false));

        #endregion

        #region CanGoForeward

        public bool CanGoForeward { get { return (bool)GetValue(CanGoForewardProperty); } set { SetValue(CanGoForewardProperty, value); } }
        public static readonly DependencyProperty CanGoForewardProperty =
            DependencyProperty.Register("CanGoForeward", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true));

        #endregion

        #region ShowNavigationButtons

        public bool ShowNavigationButtons { get { return (bool)GetValue(ShowNavigationButtonsProperty); } set { SetValue(ShowNavigationButtonsProperty, value); } }
        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register("ShowNavigationButtons", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true, OnShowNavigationButtonsPropertyChanged));

        private static void OnShowNavigationButtonsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipView;
            var value = (bool)e.NewValue;
            element.ToggleNavigationButtons(value);
        }

        #endregion

        #region ShowBackButton

        public bool ShowBackButton { get { return (bool)GetValue(ShowBackButtonProperty); } set { SetValue(ShowBackButtonProperty, value); } }
        public static readonly DependencyProperty ShowBackButtonProperty =
            DependencyProperty.Register("ShowBackButton", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true, OnShowBackButtonPropertyChanged));

        private static void OnShowBackButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipView;
            if (element.IsLoaded)
            {
                element.ToggleBackButtons();
            }
        }

        #endregion

        #region ShowHeader

        public bool ShowHeader { get { return (bool)GetValue(ShowHeaderProperty); } set { SetValue(ShowHeaderProperty, value); } }
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true, OnShowHeaderPropertyChanged));

        private static void OnShowHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipView;
            if (element.IsLoaded)
            {
                element.ToggleHeaders();
            }
        }

        #endregion

        #region ShowBreadcrumbBar

        public bool ShowBreadcrumbBar { get { return (bool)GetValue(ShowBreadcrumbBarProperty); } set { SetValue(ShowBreadcrumbBarProperty, value); } }
        public static readonly DependencyProperty ShowBreadcrumbBarProperty =
            DependencyProperty.Register("ShowBreadcrumbBar", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true));

        #endregion

        #region IsTouchNavigationEnabled

        public bool IsTouchNavigationEnabled { get { return (bool)GetValue(IsTouchNavigationEnabledProperty); } set { SetValue(IsTouchNavigationEnabledProperty, value); } }
        public static readonly DependencyProperty IsTouchNavigationEnabledProperty =
            DependencyProperty.Register("IsTouchNavigationEnabled", typeof(bool), typeof(FlipView),
            new PropertyMetadata(true));

        #endregion

        #region InlineNavigationButtonStyle

        public Style InlineNavigationButtonStyle { get { return (Style)GetValue(InlineNavigationButtonStyleProperty); } set { SetValue(InlineNavigationButtonStyleProperty, value); } }
        public static readonly DependencyProperty InlineNavigationButtonStyleProperty =
            DependencyProperty.Register("InlineNavigationButtonStyle", typeof(Style), typeof(FlipView),
            new PropertyMetadata(null));

        #endregion
        
        #endregion

        #region Events

        #region SelectedItemChanged

        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedIndexChanged

        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _panel = this.GetTemplateChild("PART_Panel") as Panel;
            _scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as AnimatableScrollViewer;
            _leftButton = this.GetTemplateChild("PART_LeftButton") as NavigationButton;
            if (_leftButton != null)
            {
                _leftButton.AddHandler(NavigationButton.ClickEvent, new RoutedEventHandler(this.OnLeftButtonClicked));
            }
            _rightButton = this.GetTemplateChild("PART_RightButton") as NavigationButton;
            if (_rightButton != null)
            {
                _rightButton.AddHandler(NavigationButton.ClickEvent, new RoutedEventHandler(this.OnRightButtonClicked));
            }
            _breadcrumbBar = this.GetTemplateChild("PART_BreadcrumbBar") as BreadcrumbBulletBar;
            if (_breadcrumbBar != null)
            {
                _breadcrumbBar.SelectedIndexChanged += this.OnBreadcrumbBarSelectedIndexChanged;
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (!DesignerProperties.GetIsInDesignMode(this))
                this.InvalidateVisual();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (this.ShowNavigationButtons)
                this.ToggleNavigationButtons(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.ShowNavigationButtons)
                this.ToggleNavigationButtons(false);
        }

        #endregion

        #region Public Methods

        public void GoBack()
        {
            if (this.SelectedIndex > 0)
                this.SelectedIndex--;
        }

        public void GoForeward()
        {
            if (this.SelectedIndex < this.Items.Count - 1)
                this.SelectedIndex++;
        }

        #endregion

        #region Private Methods

        private void AnimateSelectionChanged(double offset)
        {
            var storyboard = new Storyboard();
            var animation = new DoubleAnimation();
            animation.From = _scrollViewer.ContentHorizontalOffset;
            animation.To = offset;
            animation.FillBehavior = FillBehavior.Stop;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(animation, _scrollViewer);
            Storyboard.SetTargetProperty(animation, new PropertyPath(AnimatableScrollViewer.CurrentHorizontalOffsetProperty));
            storyboard.Children.Add(animation);
            this.SelectedIndex = (int)Math.Round(offset / _elementWidth);
            storyboard.Completed += (s, ee) =>
            {
                _scrollViewer.CurrentHorizontalOffset = offset;
                _leftButton.IsEnabled = true;
                _rightButton.IsEnabled = true;
            };
            storyboard.Begin();
        }
        
        private void ToggleNavigationButtons(bool visible)
        {
            if (_leftButton != null && _rightButton != null)
            {
                _leftButton.Opacity = visible ? 0.7 : 0;
                _leftButton.IsEnabled = visible;
                _rightButton.Opacity = visible ? 0.7 : 0;
                _rightButton.IsEnabled = visible;
            }
        }

        private void ToggleBackButtons()
        {
            foreach (var item in this.Items)
            {
                var section = item as FlipViewSection;
                if (section != null)
                {
                    section.ShowNavigationButton = this.ShowBackButton;
                }
            }
        }

        private void ToggleHeaders()
        {
            foreach (var item in this.Items)
            {
                var section = item as FlipViewSection;
                if (section != null)
                {
                    section.ShowHeader = this.ShowHeader;
                }
            }
        }

        #endregion

        #region Event Handling

        #region FlipView Events

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_panel != null && _panel.Children.Count > 0) return;
            if (_panel != null)
            {
                _panel.StylusDown += this.OnStylusDown;
                _panel.StylusMove += this.OnStylusMove;
                _panel.StylusUp += this.OnStylusUp;
                _panel.StylusLeave += this.OnStylusLeave;

                _elementWidth = _scrollViewer.RenderSize.Width;
                foreach (var item in this.Items)
                {
                    var presenter = new ContentPresenter() { Content = item };
                    presenter.Width = _elementWidth;
                    _panel.Children.Add(presenter);
                }
            }
            this.CanGoBack = this.SelectedIndex > 0;
            this.CanGoForeward = (this.Items.Count > 0) ? this.SelectedIndex < this.Items.Count - 1 : false;
            this.ToggleBackButtons();
            this.ToggleHeaders();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                _elementWidth = this.RenderSize.Width;
            else
                _elementWidth = _scrollViewer.RenderSize.Width;
            if (_panel != null)
            {
                foreach (var item in _panel.Children)
                {
                    ((FrameworkElement)item).Width = _elementWidth;
                }
                if (!DesignerProperties.GetIsInDesignMode(this))
                    _scrollViewer.CurrentHorizontalOffset = this.SelectedIndex * _elementWidth;
            }
        }

        #endregion

        #region BreadcrumbBar Events

        private void OnBreadcrumbBarSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            this.SelectedIndex = ((BreadcrumbBulletBar)sender).SelectedIndex;
        }

        #endregion
        
        #region Stylus Events

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            if (!this.IsTouchNavigationEnabled) return;
            _downPoints = e.GetStylusPoints(sender as IInputElement);
            this.ShowNavigationButtons = false;
        }

        private void OnStylusMove(object sender, StylusEventArgs e)
        {
            if (!this.IsTouchNavigationEnabled) return;
            var x = _scrollViewer.ContentHorizontalOffset +
                _downPoints[0].X - e.GetStylusPoints(sender as IInputElement)[0].X;
            _scrollViewer.ScrollToHorizontalOffset(x);
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            if (!this.IsTouchNavigationEnabled) return;
            var pos = _scrollViewer.ContentHorizontalOffset;
            var x = Math.Round((Math.Round(pos / _elementWidth)) * _elementWidth);
            this.AnimateSelectionChanged(x);
        }

        private void OnStylusLeave(object sender, StylusEventArgs e)
        {
            if (!this.IsTouchNavigationEnabled) return;
            this.OnStylusUp(sender, e);
        }

        #endregion

        #region NavigationButton Events

        private void OnLeftButtonClicked(object sender, RoutedEventArgs e)
        {
            _leftButton.IsEnabled = false;
            this.SelectedIndex--;
        }

        private void OnRightButtonClicked(object sender, RoutedEventArgs e)
        {
            _rightButton.IsEnabled = false;
            this.SelectedIndex++;
        }

        #endregion

        #endregion

    }

    #region FlipViewItemCollection

    public class FlipViewItemCollection : Collection<UIElement>
    {
    }

    #endregion

}
