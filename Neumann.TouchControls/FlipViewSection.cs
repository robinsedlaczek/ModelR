using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    public class FlipViewSection : HeaderedContentControl
    {

        #region Private Fields

        private Button _navigationButton;
        private FlipView _flipView;

        #endregion

        #region Constructors

        public FlipViewSection()
        {
            this.DefaultStyleKey = typeof(FlipViewSection);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region ShowNavigationButton

        public bool ShowNavigationButton { get { return (bool)GetValue(ShowNavigationButtonProperty); } set { SetValue(ShowNavigationButtonProperty, value); } }
        public static readonly DependencyProperty ShowNavigationButtonProperty =
            DependencyProperty.Register("ShowNavigationButton", typeof(bool), typeof(FlipViewSection),
            new PropertyMetadata(true, OnShowNavigationButtonPropertyChanged));

        private static void OnShowNavigationButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FlipViewSection;
            element.ShowNavigationButtonCore = (bool)e.NewValue;
        }

        #endregion

        #region ShowNavigationButtonCore

        internal bool ShowNavigationButtonCore { get { return (bool)GetValue(ShowNavigationButtonCoreProperty); } set { SetValue(ShowNavigationButtonCoreProperty, value); } }
        public static readonly DependencyProperty ShowNavigationButtonCoreProperty =
            DependencyProperty.Register("ShowNavigationButtonCore", typeof(bool), typeof(FlipViewSection),
            new PropertyMetadata(true));

        #endregion

        #region ShowHeader

        public bool ShowHeader { get { return (bool)GetValue(ShowHeaderProperty); } set { SetValue(ShowHeaderProperty, value); } }
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(FlipViewSection),
            new PropertyMetadata(true));

        #endregion

        #region IsSelected

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } protected set { SetValue(IsSelectedPropertyKey, value); } }
        private static readonly DependencyPropertyKey IsSelectedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(FlipViewSection),
            new PropertyMetadata(false, OnIsSelectedChanged));
        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #endregion

        #region Events

        public event EventHandler NavigatedBack;
        private void OnNavigatedBack()
        {
            if (NavigatedBack != null)
                NavigatedBack(this, EventArgs.Empty);
        }

        #region IsSelectedChanged

        public event EventHandler IsSelectedChanged;
        internal void OnIsSelectedChanged()
        {
            if (IsSelectedChanged != null)
                IsSelectedChanged(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _navigationButton = this.GetTemplateChild("PART_NavigationButton") as Button;
            if (_navigationButton != null)
            {
                _navigationButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnNavigationButtonClick));
            }
        }

        #endregion

        #region Private Methods

        private void DetectFlipView()
        {
            DependencyObject element = this;
            while (true)
            {
                element = VisualTreeHelper.GetParent(element);
                if (element == null || element is FlipView)
                    break;
            }
            if (element != null)
            {
                _flipView = element as FlipView;
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DetectFlipView();
            if (_flipView != null)
            {
                this.ShowNavigationButtonCore = (_flipView.Items.IndexOf(this) > 0 & this.ShowNavigationButton);
            }
            if (_navigationButton == null)
            {
                _navigationButton = ElementHelpers.GetVisualChild<NavigationButton>(this, "PART_NavigationButton");
                if (_navigationButton != null)
                {
                    _navigationButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnNavigationButtonClick));
                }
            }
        }

        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            if (_flipView != null)
            {
                _flipView.GoBack();
            }
            this.OnNavigatedBack();
        }

        #endregion

    }
}
