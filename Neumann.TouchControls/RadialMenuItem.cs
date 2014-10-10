using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Neumann.TouchControls
{
    [ContentProperty("Items")]
    public class RadialMenuItem : Control
    {

        #region Private Fields

        private const double PART_LENGTH = 44d;
        private const double COMMAND_DISTANCE = 35d;
        private const double PART_SPACING = 1d;
        private Panel _commandPandel;

        #endregion

        #region Constructors

        public RadialMenuItem()
        {
            this.DefaultStyleKey = typeof(RadialMenuItem);
            this.Items = new RadialMenuItemCollection();
            var descriptor = DependencyPropertyDescriptor.FromProperty(RadialMenuItem.IsEnabledProperty, typeof(RadialMenuItem));
            if (descriptor != null)
            {
                descriptor.AddValueChanged(this, this.ToggleIsEnabled);
            }
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region ImageSource

        public ImageSource ImageSource { get { return (ImageSource)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadialMenuItem),
            new PropertyMetadata(null));

        #endregion

        #region Header

        public string Header { get { return (string)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RadialMenuItem),
            new PropertyMetadata(null));

        #endregion

        #region Items

        public RadialMenuItemCollection Items { get { return (RadialMenuItemCollection)GetValue(ItemsProperty); } set { SetValue(ItemsProperty, value); } }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(RadialMenuItemCollection), typeof(RadialMenuItem),
            new PropertyMetadata(null));

        #endregion

        #region HasChildren

        public bool HasChildren
        {
            get { return this.Items.Count > 0; }
        }

        #endregion

        #region Command

        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RadialMenuItem),
            new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenuItem;
            var command = e.NewValue as ICommand;
            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
                element.IsEnabled = command.CanExecute(null);
            }
            else
            {
                command = e.OldValue as ICommand;
                if (command != null)
                {
                    command.CanExecuteChanged -= element.OnCommandCanExecuteChanged;
                }
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            var command = sender as ICommand;
            this.IsEnabled = command.CanExecute(null);
        }

        #endregion

        #region Position

        public int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(RadialMenuItem),
            new PropertyMetadata(-1, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenuItem;
            var value = (int)e.NewValue;
            if (value == 0)
            {
                element.StartAngle = 338;
                element.EndAngle = 22;
            }
            else
            {
                var start = 23;
                element.StartAngle = ((value - 1) * (PART_LENGTH + PART_SPACING)) + start;
                element.EndAngle = element.StartAngle + PART_LENGTH;
            }
            element.PositionCommand();
        }

        #endregion

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialMenuItem),
            new PropertyMetadata(-1d));

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(RadialMenuItem),
            new PropertyMetadata(-1d));

        #endregion

        #region ShowFocusIndicator

        public bool ShowFocusIndicator { get { return (bool)GetValue(ShowFocusIndicatorProperty); } set { SetValue(ShowFocusIndicatorProperty, value); } }
        public static readonly DependencyProperty ShowFocusIndicatorProperty =
            DependencyProperty.Register("ShowFocusIndicator", typeof(bool), typeof(RadialMenuItem),
            new PropertyMetadata(false, OnShowFocusIndicatorPropertyChanged));

        private static void OnShowFocusIndicatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenuItem;
            var value = (bool)e.NewValue;
            if (value)
            {
                VisualStateManager.GoToState(element, "MouseEnter", false);
            }
            else
            {
                VisualStateManager.GoToState(element, "MouseLeave", false);
            }
        }

        #endregion

        #endregion

        #region Events

        #region Click

        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (this.Command != null)
            {
                this.Command.Execute(null);
            }

            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _commandPandel = this.GetTemplateChild("PART_CommandPanel") as Panel;
            if (_commandPandel != null)
            {
                _commandPandel.AddHandler(RadialMenuItem.MouseEnterEvent, new MouseEventHandler(this.OnMouseEnter));
                _commandPandel.AddHandler(RadialMenuItem.MouseLeaveEvent, new MouseEventHandler(this.OnMouseLeave));
                _commandPandel.AddHandler(RadialMenuItem.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp));
            }
        }

        #endregion

        #region Private Methods

        private void PositionCommand()
        {
            if (_commandPandel != null)
            {
                var translate = new TranslateTransform();
                var start = (this.RenderSize.Height / 2) - COMMAND_DISTANCE;
                var angle = (this.StartAngle + PART_LENGTH / 2);
                var radian = (angle * Math.PI) / 180;
                translate.X = (int)start * Math.Sin(radian);
                translate.Y = (int)((start * Math.Cos(radian)) * -1);
                _commandPandel.RenderTransform = translate;
            }
        }

        internal void StartExpandingAnimation()
        {
            if (!string.IsNullOrWhiteSpace(this.Header))
                VisualStateManager.GoToState(this, "Expanded", false);
        }

        internal void StartCollapsingAnimation()
        {
            if (!string.IsNullOrWhiteSpace(this.Header))
                VisualStateManager.GoToState(this, "Collapsed", false);
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.PositionCommand();
            this.ToggleIsEnabled(sender, e);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseEnter", false);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseLeave", false);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.OnClick();
        }

        private void ToggleIsEnabled(object sender, EventArgs e)
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", false);
            }
        }

        #endregion

    }
}
