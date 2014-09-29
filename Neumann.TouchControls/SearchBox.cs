using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    public sealed class SearchBox : TextBox
    {

        #region Private Fields

        private Selector _suggenstionListBox;
        private Popup _popup;
        private Button _searchButton;

        #endregion

        #region Constructors

        public SearchBox()
        {
            this.DefaultStyleKey = typeof(SearchBox);
        }

        #endregion

        #region Properties

        #region SuggestionsList

        public List<string> SuggestionsList { get { return (List<string>)GetValue(SuggestionsListProperty); } set { SetValue(SuggestionsListProperty, value); } }
        public static readonly DependencyProperty SuggestionsListProperty =
            DependencyProperty.Register("SuggestionsList", typeof(List<string>), typeof(SearchBox),
            new PropertyMetadata(null));

        #endregion

        #region IsDropDownOpen

        public bool IsDropDownOpen { get { return (bool)GetValue(IsDropDownOpenProperty); } set { SetValue(IsDropDownOpenProperty, value); } }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SearchBox),
            new PropertyMetadata(false, OnIsDropDownOpenPropertyChanged));

        private static void OnIsDropDownOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as SearchBox;
            if (element._popup != null)
            {
                var value = (bool)e.NewValue;
                element._popup.IsOpen = value;
            }
        }

        #endregion

        #region SearchCommand

        public ICommand SearchCommand { get { return (ICommand)GetValue(SearchCommandProperty); } set { SetValue(SearchCommandProperty, value); } }
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchBox),
            new PropertyMetadata(null));

        #endregion

        #region MaxDropDownItems

        public int MaxDropDownItems { get { return (int)GetValue(MaxDropDownItemsProperty); } set { SetValue(MaxDropDownItemsProperty, value); } }
        public static readonly DependencyProperty MaxDropDownItemsProperty =
            DependencyProperty.Register("MaxDropDownItems", typeof(int), typeof(SearchBox),
            new PropertyMetadata(10));

        #endregion
        
        #endregion

        #region Events

        #region SuggestionsRequested

        public event EventHandler<SearchBoxSuggestionsRequestedEventArgs> SuggestionsRequested;
        private void OnSuggestionsRequested(SearchBoxSuggestionsRequestedEventArgs e)
        {
            if (this.SuggestionsList != null)
            {
                if (string.IsNullOrWhiteSpace(e.QueryText)) return;
                var queryText = e.QueryText.ToLower();
                foreach (var suggestion in this.SuggestionsList)
                {
                    if (suggestion.ToLower().StartsWith(queryText) &&
                        suggestion.ToLower() != queryText)
                    {
                        e.Request.SearchSuggestionCollection.AppendQuerySuggestion(suggestion);
                    }
                }
            }
            else if (SuggestionsRequested != null)
            {
                SuggestionsRequested(this, e);
            }
        }

        #endregion

        #region QuerySubmitted

        public event EventHandler<SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        private void OnQuerySubmitted(string queryText)
        {
            if (QuerySubmitted != null)
                QuerySubmitted(this, new SearchBoxQuerySubmittedEventArgs(queryText));
        }

        #endregion

        #endregion

        #region Methods

        #region OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _suggenstionListBox = this.GetTemplateChild("PART_SuggenstionListBox") as Selector;
            if (_suggenstionListBox != null)
            {
                _suggenstionListBox.AddHandler(ListBox.PreviewKeyDownEvent, new KeyEventHandler(this.OnSuggestionListBoxPreviewKeyDown));
                _suggenstionListBox.AddHandler(ListBox.MouseUpEvent, new MouseButtonEventHandler(this.OnSuggestionListBoxMouseUp));
                _suggenstionListBox.AddHandler(ListBox.LostFocusEvent, new RoutedEventHandler(this.OnSuggestionListBoxLostFocus));
            }
            _popup = this.GetTemplateChild("PART_Popup") as Popup;
            _searchButton = this.GetTemplateChild("PART_SearchButton") as Button;
            if (_searchButton != null)
            {
                _searchButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnSearchButtonClick));
                this.InvalidateMeasure();
                this.Measure(new Size(100, 100));
                _searchButton.Height = this.DesiredSize.Height;
            }
            this.AddHandler(SearchBox.SizeChangedEvent, new SizeChangedEventHandler(this.OnSizeChanged));
        }

        #endregion

        #region OnSizeChanged

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var relativePoint = this.TransformToAncestor(this).Transform(new Point(0, 0));
            _popup.PlacementRectangle = new Rect(0, e.NewSize.Height-1, 0, 0);
        }

        #endregion

        #region OnPreviewKeyUp

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            if (_suggenstionListBox == null || Keyboard.FocusedElement != this) return;

            if (e.Key == Key.Down)
            {
                if (_suggenstionListBox.Items.Count > 0)
                {
                    _suggenstionListBox.SelectedIndex = 0;
                    var item = _suggenstionListBox.Items[0] as ListBoxItem;
                    item.IsSelected = true;
                    _popup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                return;
            }
            else if (e.Key == Key.Tab && _suggenstionListBox.SelectedIndex > -1)
            {
                this.Text = ((ListBoxItem)_suggenstionListBox.SelectedItem).Content.ToString();
                _suggenstionListBox.Visibility = Visibility.Collapsed;
                this.SelectionStart = this.Text.Length;
                return;
            }

            _suggenstionListBox.Items.Clear();
            var queryText = this.Text;
            if (this.SelectedText.Length > 0 && this.Text.Length > 0)
                queryText = this.Text.Replace(this.SelectedText, "");

            var request = new SearchBoxSuggestionsRequestedEventArgs(queryText);
            this.OnSuggestionsRequested(request);
            int i = 0;
            foreach (var suggestion in request.Request.SearchSuggestionCollection)
            {
                _suggenstionListBox.Items.Add(new ListBoxItem() { Content = suggestion });
                i++;
                if (i == this.MaxDropDownItems)
                    break;
            }
            this.IsDropDownOpen = (_suggenstionListBox.Items.Count > 0);
            _suggenstionListBox.Visibility = (_suggenstionListBox.Items.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            _suggenstionListBox.SelectedIndex = (_suggenstionListBox.Items.Count > 0) ? 0 : -1;
        }

        #endregion

        #region OnPreviewKeyDown

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (_suggenstionListBox == null || Keyboard.FocusedElement != this) return;
            var item = _suggenstionListBox.SelectedItem as ListBoxItem;
            if ((e.Key == Key.Tab || e.Key == Key.Enter && this.IsDropDownOpen) &&
                _suggenstionListBox.SelectedIndex > -1 &&
                item != null &&
                item.Content != null)
            {
                this.Text = item.Content.ToString();
                _suggenstionListBox.Visibility = Visibility.Collapsed;
                this.SelectionStart = this.Text.Length;
                return;
            }
            else if (e.Key == Key.Enter && this.IsDropDownOpen == false)
            {
                _searchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        #endregion

        #region OnSuggestionListBoxPreviewKeyDown

        private void OnSuggestionListBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_suggenstionListBox == null) return;
            if ((e.Key == Key.Tab || e.Key == Key.Enter) &&
                _suggenstionListBox.SelectedIndex > -1)
            {
                this.Text = ((ListBoxItem)_suggenstionListBox.SelectedItem).Content.ToString();
                _suggenstionListBox.Visibility = Visibility.Collapsed;
                this.SelectionStart = this.Text.Length;
                this.Focus();
                return;
            }
        }

        #endregion

        #region OnSuggestionListBoxMouseUp

        private void OnSuggestionListBoxMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_suggenstionListBox != null && _suggenstionListBox.SelectedIndex > -1)
            {
                this.Text = ((ListBoxItem)_suggenstionListBox.SelectedItem).Content.ToString();
                _suggenstionListBox.Visibility = Visibility.Collapsed;
                this.SelectionStart = this.Text.Length;
                this.Focus();
                return;
            }
        }

        #endregion

        #region OnLostFocus

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (!(Keyboard.FocusedElement is ListBoxItem))
                this.IsDropDownOpen = false;
        }

        #endregion

        #region OnSuggestionListBoxLostFocus

        private void OnSuggestionListBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!(Keyboard.FocusedElement is ListBoxItem))
                this.IsDropDownOpen = false;
        }

        #endregion

        #region OnSearchButtonClick

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnQuerySubmitted(this.Text);
        }

        #endregion

        #endregion

    }

    #region SearchBoxSuggestionsRequestedEventArgs

    public class SearchBoxSuggestionsRequestedEventArgs : EventArgs
    {
        public SearchBoxSuggestionsRequestedEventArgs(string queryText)
        {
            this.QueryText = queryText;
            this.Request = new SearchSuggestionsRequest();
        }
        public SearchSuggestionsRequest Request { get; private set; }
        public string QueryText { get; private set; }
    }

    #endregion

    #region SearchSuggestionsRequest

    public sealed class SearchSuggestionsRequest
    {
        public SearchSuggestionsRequest()
        {
            SearchSuggestionCollection = new SearchSuggestionCollection();
        }
        public SearchSuggestionCollection SearchSuggestionCollection { get; private set; }
    }

    #endregion

    #region SearchSuggestionCollection

    public sealed class SearchSuggestionCollection : List<string>
    {
        public void AppendQuerySuggestion(string text)
        {
            base.Add(text);
        }

        public void AppendQuerySuggestions(IEnumerable<string> suggestions)
        {
            foreach (var suggestion in suggestions)
            {
                base.Add(suggestion);
            }
        }
    }

    #endregion

    #region SearchBoxQuerySubmittedEventArgs

    public sealed class SearchBoxQuerySubmittedEventArgs
    {
        public SearchBoxQuerySubmittedEventArgs(string queryText)
        {
            this.QueryText = queryText;
        }

        public string QueryText { get; private set; }
    }

    #endregion

}
