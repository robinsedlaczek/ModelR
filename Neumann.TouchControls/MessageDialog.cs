using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    [DefaultProperty("Commands")]
    public class MessageDialog : HeaderedContentControl
    {

        #region Private Fields

        private Grid _grid;

        #endregion

        #region Constructors

        public MessageDialog()
        {
            this.DefaultStyleKey = typeof(MessageDialog);
            this.Commands = new CommandsCollection();
        }

        #endregion

        #region Events

        public event EventHandler<DefaultButtonClickedEventArgs> DefaultButtonClicked;
        protected virtual void OnDefaultButtonClicked(MessageDialogDefaultButton buttonType)
        {
            if (DefaultButtonClicked != null)
                DefaultButtonClicked(this, new DefaultButtonClickedEventArgs(buttonType));
        }

        #endregion

        #region Properties

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MessageDialog),
            new PropertyMetadata(false, OnIsOpenPropertyChanged));
        public bool IsOpen { get { return (bool)this.GetValue(IsOpenProperty); } set { this.SetValue(IsOpenProperty, value); } }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as MessageDialog;
            element.Focus();
            if (element.IsOpen && element.Commands.Count > 0)
                element.Commands[0].Focus();
            if (element._grid == null) return;
            var isOpen = (bool)e.NewValue;
            if (isOpen)
                VisualStateManager.GoToElementState(element._grid, "Open", false);
            else
                VisualStateManager.GoToElementState(element._grid, "Close", false);
        }

        #endregion

        #region Commands
        
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(CommandsCollection), typeof(MessageDialog), new PropertyMetadata());
        public CommandsCollection Commands { get { return (CommandsCollection)GetValue(CommandsProperty); } set { SetValue(CommandsProperty, value); } }
        
        #endregion

        #region DefaultButtons

        public static readonly DependencyProperty DefaultButtonsProperty =
            DependencyProperty.Register("DefaultButtons", typeof(MessageDialogDefaultButtons), typeof(MessageDialog),
            new PropertyMetadata(MessageDialogDefaultButtons.Ok, OnDefaultButtonsPropertyChanged));
        public MessageDialogDefaultButtons DefaultButtons { get { return (MessageDialogDefaultButtons)GetValue(DefaultButtonsProperty); } set { SetValue(DefaultButtonsProperty, value); } }

        private static void OnDefaultButtonsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MessageDialog;
            element.CreateDefaultButtons();
        }

        #endregion

        #region DefaultButtonStyle
        
        public static readonly DependencyProperty DefaultButtonStyleProperty =
            DependencyProperty.Register("DefaultButtonStyle", typeof(Style), typeof(MessageDialog), new PropertyMetadata());
        public Style DefaultButtonStyle { get { return (Style)GetValue(DefaultButtonStyleProperty); } set { SetValue(DefaultButtonStyleProperty, value); } }

        #endregion

        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(MessageDialog));
        public Brush HeaderBackground { get { return (Brush)GetValue(HeaderBackgroundProperty); } set { SetValue(HeaderBackgroundProperty, value); } }

        #endregion

        #region HeaderBorderBrush

        public static readonly DependencyProperty HeaderBorderBrushProperty =
            DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(MessageDialog));
        public Brush HeaderBorderBrush { get { return (Brush)GetValue(HeaderBorderBrushProperty); } set { SetValue(HeaderBorderBrushProperty, value); } }

        #endregion

        #region HeaderBorderThickness

        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(MessageDialog));
        public Thickness HeaderBorderThickness { get { return (Thickness)GetValue(HeaderBorderThicknessProperty); } set { SetValue(HeaderBorderThicknessProperty, value); } }

        #endregion

        #region HeaderForeground

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(MessageDialog));
        public Brush HeaderForeground { get { return (Brush)GetValue(HeaderForegroundProperty); } set { SetValue(HeaderForegroundProperty, value); } }

        #endregion

        #region HeaderFontFamily

        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(MessageDialog));
        public FontFamily HeaderFontFamily { get { return (FontFamily)GetValue(HeaderFontFamilyProperty); } set { SetValue(HeaderFontFamilyProperty, value); } }

        #endregion

        #region HeaderFontSize

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(MessageDialog));
        public double HeaderFontSize { get { return (double)GetValue(HeaderFontSizeProperty); } set { SetValue(HeaderFontSizeProperty, value); } }

        #endregion

        #region HeaderFontWeight

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(MessageDialog));
        public FontWeight HeaderFontWeight { get { return (FontWeight)GetValue(HeaderFontWeightProperty); } set { SetValue(HeaderFontWeightProperty, value); } }

        #endregion

        #region OkCommand

        public static readonly DependencyProperty OkCommandProperty =
            DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(null, OnOkCommandPropertyChanged));
        public ICommand OkCommand { get { return (ICommand)GetValue(OkCommandProperty); } set { SetValue(OkCommandProperty, value); } }

        private static void OnOkCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).RefreshButtonCommand(MessageDialogDefaultButton.Ok, e.NewValue as ICommand);
        }

        #endregion
        
        #region CancelCommand

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(null, OnCancelCommandPropertyChanged));
        public ICommand CancelCommand { get { return (ICommand)GetValue(CancelCommandProperty); } set { SetValue(CancelCommandProperty, value); } }

        private static void OnCancelCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).RefreshButtonCommand(MessageDialogDefaultButton.Cancel, e.NewValue as ICommand);
        }

        #endregion

        #region CloseCommand

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(null, OnCloseCommandPropertyChanged));
        public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }

        private static void OnCloseCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).RefreshButtonCommand(MessageDialogDefaultButton.Close, e.NewValue as ICommand);
        }

        #endregion

        #region YesCommand

        public static readonly DependencyProperty YesCommandProperty =
            DependencyProperty.Register("YesCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(null, OnYesCommandPropertyChanged));
        public ICommand YesCommand { get { return (ICommand)GetValue(YesCommandProperty); } set { SetValue(YesCommandProperty, value); } }

        private static void OnYesCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).RefreshButtonCommand(MessageDialogDefaultButton.Yes, e.NewValue as ICommand);
        }

        #endregion

        #region NoCommand

        public static readonly DependencyProperty NoCommandProperty =
            DependencyProperty.Register("NoCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(null, OnNoCommandPropertyChanged));
        public ICommand NoCommand { get { return (ICommand)GetValue(NoCommandProperty); } set { SetValue(NoCommandProperty, value); } }

        private static void OnNoCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).RefreshButtonCommand(MessageDialogDefaultButton.No, e.NewValue as ICommand);
        }

        #endregion

        #region DialogResult
        
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register("DialogResult", typeof(MessageDialogDefaultButton), typeof(MessageDialog), new PropertyMetadata(MessageDialogDefaultButton.Cancel));
        public MessageDialogDefaultButton DialogResult { get { return (MessageDialogDefaultButton)GetValue(DialogResultProperty); } private set { SetValue(DialogResultProperty, value); } }

        #endregion

        #endregion

        #region Public Functions

        public void Show(UIElement parent)
        {
            if (parent is ContentControl)
                parent = ((ContentControl)parent).Content as UIElement;
            if (parent is Panel)
            {
                this.SetValue(Grid.RowSpanProperty, 100);
                this.SetValue(Grid.ColumnSpanProperty, 100);
                ((Panel)parent).Children.Add(this);
                this.IsOpen = true;
            }
            else
            {
                throw new ArgumentException("Can't integrate dialog into visual tree. Set grid or window as parent.");
            }
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _grid = this.GetTemplateChild("grid") as Grid;
            if (this.DefaultButtonStyle == null)
            {
                var dictionary = Application.LoadComponent(new Uri("/Neumann.TouchControls;component/MessageDialog.xaml", UriKind.Relative)) as ResourceDictionary;
                if (dictionary != null && dictionary.Contains("DefaultButtonStyle"))
                {
                    this.DefaultButtonStyle = dictionary["DefaultButtonStyle"] as Style;
                }
            }
            this.CreateDefaultButtons();
        }
        
        #endregion

        #region Private Functions

        #region CreateDefaultButtons
        
        private void CreateDefaultButtons()
        {
            if (this.DefaultButtons != MessageDialogDefaultButtons.Custom)
            {
                this.Commands = new CommandsCollection();
                switch (this.DefaultButtons)
                {
                    case MessageDialogDefaultButtons.Ok:
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Ok, this.OkCommand));
                        break;
                    case MessageDialogDefaultButtons.OkCancel:
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Ok, this.OkCommand));
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Cancel, this.CancelCommand));
                        break;
                    case MessageDialogDefaultButtons.YesNo:
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Yes, this.YesCommand));
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.No, this.NoCommand));
                        break;
                    case MessageDialogDefaultButtons.YesNoCancel:
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Yes, this.YesCommand));
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.No, this.NoCommand));
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Cancel, this.CancelCommand));
                        break;
                    case MessageDialogDefaultButtons.Close:
                        this.Commands.Add(this.CreateButton(MessageDialogDefaultButton.Close, this.CloseCommand));
                        break;
                }
            }
            else
            {
                this.Commands.ForEach(button => button.Style = this.DefaultButtonStyle);
            }
        }

        #endregion

        #region CreateButton

        private Button CreateButton(MessageDialogDefaultButton buttonType, ICommand command)
        {
            var button = new Button
            {
                Content = this.GetDefaultButtonContent(buttonType),
                Command = command,
                Style = this.DefaultButtonStyle,
                Tag = buttonType,
                IsDefault = (buttonType == MessageDialogDefaultButton.Ok || buttonType == MessageDialogDefaultButton.Yes),
                IsCancel = (buttonType == MessageDialogDefaultButton.Cancel || buttonType == MessageDialogDefaultButton.Close || buttonType == MessageDialogDefaultButton.No),
            };
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnDefaultButtonClicked));
            return button;
        }

        #endregion

        #region GetDefaultButtonContent

        private string GetDefaultButtonContent(MessageDialogDefaultButton buttonType)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            var lang = culture.TwoLetterISOLanguageName;
            switch (buttonType)
            {
                case MessageDialogDefaultButton.Ok: if (lang == "de") return "OK"; else if (lang == "en") return "OK"; break;
                case MessageDialogDefaultButton.Cancel: if (lang == "de") return "Abbrechen"; else if (lang == "en") return "Cancel"; break;
                case MessageDialogDefaultButton.Yes: if (lang == "de") return "Ja"; else if (lang == "en") return "Yes"; break;
                case MessageDialogDefaultButton.No: if (lang == "de") return "Nein"; else if (lang == "en") return "No"; break;
                case MessageDialogDefaultButton.Close: if (lang == "de") return "Schließen"; else if (lang == "en") return "Close"; break;
            }
            return string.Empty;
        }

        #endregion

        #region RefreshButtonCommand

        private void RefreshButtonCommand(MessageDialogDefaultButton buttonType, ICommand command)
        {
            var button = (from b in this.Commands
                          where b.Tag is MessageDialogDefaultButton &&
                                ((MessageDialogDefaultButton)b.Tag).Equals(buttonType)
                          select b).FirstOrDefault();
            if (button != null)
                button.Command = command;
        }

        #endregion

        #endregion

        #region Event Handling

        private void OnDefaultButtonClicked(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            var button = sender as Button;
            var tag = button.Tag;
            if (tag != null && tag is MessageDialogDefaultButton)
            {
                var buttonType = (MessageDialogDefaultButton)tag;
                this.DialogResult = buttonType;
                this.OnDefaultButtonClicked(buttonType);
            }
        }

        #endregion

    }

    public class CommandsCollection : List<ButtonBase>
    {
    }

    public class DefaultButtonClickedEventArgs : EventArgs
    {
        public readonly MessageDialogDefaultButton ButtonType;
        public DefaultButtonClickedEventArgs(MessageDialogDefaultButton buttonType)
        {
            this.ButtonType = buttonType;
        }
    }

    public enum MessageDialogDefaultButtons
    {
        Custom,
        Ok,
        Close,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageDialogDefaultButton
    {
        Custom,
        Ok,
        Close,
        Cancel,
        Yes,
        No
    }
}
