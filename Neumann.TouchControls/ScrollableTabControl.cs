using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Neumann.TouchControls
{
    public class ScrollableTabControl : TabControl
    {

        #region Private Fields

        private ScrollViewer _scrollViewer;
        private ContentPresenter _selectedContentHost;
        private StackPanel _headerPanel;
        private int _scrollIndex = 0;

        #endregion

        #region Constructors

        public ScrollableTabControl()
        {
            this.DefaultStyleKey = typeof(ScrollableTabControl);
        }

        #endregion

        #region Properties

        #region ShowHeaderLine

        public bool ShowHeaderLine { get { return (bool)GetValue(ShowHeaderLineProperty); } set { SetValue(ShowHeaderLineProperty, value); } }
        public static readonly DependencyProperty ShowHeaderLineProperty =
            DependencyProperty.Register("ShowHeaderLine", typeof(bool), typeof(ScrollableTabControl),
            new PropertyMetadata(true));

        #endregion

        #region TransitionAnimation

        public bool TransitionAnimation { get { return (bool)GetValue(TransitionAnimationProperty); } set { SetValue(TransitionAnimationProperty, value); } }
        public static readonly DependencyProperty TransitionAnimationProperty =
            DependencyProperty.Register("TransitionAnimation", typeof(bool), typeof(ScrollableTabControl),
            new PropertyMetadata(true));

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _headerPanel = this.GetTemplateChild("headerPanel") as StackPanel;
            _scrollViewer = this.GetTemplateChild("scrollViewer") as ScrollViewer;
            _selectedContentHost = this.GetTemplateChild("PART_SelectedContentHost") as ContentPresenter;
            var leftButton = this.GetTemplateChild("leftButton") as RepeatButton;
            var rightButton = this.GetTemplateChild("rightButton") as RepeatButton;
            if (leftButton != null && rightButton != null)
            {
                leftButton.Click += this.OnLeftButtonClick;
                rightButton.Click += this.OnRightButtonClick;
            }
        }

        #endregion

        #region Private Methods

        private double GetOffset(int index)
        {
            double offset = 0;
            if (index > 0 && index < this.Items.Count - 1)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    offset += ((UIElement)this.Items[i]).RenderSize.Width;
                    if (i == index || index == 1)
                        break;
                }
            }
            return offset;
        }

        #endregion

        #region Event Handling

        private void OnLeftButtonClick(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer != null && this.Items.Count > 0 && _scrollIndex > 0)
            {
                _scrollIndex--;
                _scrollViewer.ScrollToHorizontalOffset(this.GetOffset(_scrollIndex));
            }
        }

        private void OnRightButtonClick(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer == null || _headerPanel == null) return;
            var actualOffset = _scrollViewer.HorizontalOffset + _scrollViewer.RenderSize.Width;
            if (this.Items.Count > 0 && _scrollIndex < this.Items.Count - 1 &&
                actualOffset < _headerPanel.RenderSize.Width)
            {
                _scrollIndex++;
                _scrollViewer.ScrollToHorizontalOffset(this.GetOffset(_scrollIndex));
            }
        }

        #endregion

    }
}
