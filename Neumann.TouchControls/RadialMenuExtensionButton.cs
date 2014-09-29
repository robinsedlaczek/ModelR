using Microsoft.Expression.Shapes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Neumann.TouchControls
{
    public class RadialMenuExtensionButton : Control
    {

        #region Private Fields
        
        private const double PART_LENGTH = 44d;
        private const double PART_SPACING = 1d;
        private Arc _expansionButton;

        #endregion

        #region Constructors

        public RadialMenuExtensionButton()
        {
            this.DefaultStyleKey = typeof(RadialMenuExtensionButton);
            var descriptor = DependencyPropertyDescriptor.FromProperty(RadialMenuExtensionButton.IsEnabledProperty, typeof(RadialMenuExtensionButton));
            if (descriptor != null)
            {
                descriptor.AddValueChanged(this, this.ToggleIsEnabled);
            }
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Position

        public int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(RadialMenuExtensionButton),
            new PropertyMetadata(-1, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialMenuExtensionButton;
            var value = (int)e.NewValue;
            if (value == 0)
            {
                element.StartAngle = 338;
                element.EndAngle = 22;
                element.MarkerAngle = -1;
            }
            else
            {
                var start = 23;
                element.StartAngle = ((value-1) * (PART_LENGTH + PART_SPACING)) + start;
                element.EndAngle = element.StartAngle + PART_LENGTH;
                element.MarkerAngle = element.EndAngle - (PART_LENGTH / 2);
            }
        }

        #endregion
        
        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialMenuExtensionButton),
            new PropertyMetadata(-1d));

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(RadialMenuExtensionButton),
            new PropertyMetadata(-1d));

        #endregion

        #region MarkerAngle

        public double MarkerAngle { get { return (double)GetValue(MarkerAngleProperty); } set { SetValue(MarkerAngleProperty, value); } }
        public static readonly DependencyProperty MarkerAngleProperty =
            DependencyProperty.Register("MarkerAngle", typeof(double), typeof(RadialMenuExtensionButton),
            new PropertyMetadata(0d));

        #endregion

        #region MenuItem

        public RadialMenuItem MenuItem { get { return (RadialMenuItem)GetValue(MenuItemProperty); } set { SetValue(MenuItemProperty, value); } }
        public static readonly DependencyProperty MenuItemProperty =
            DependencyProperty.Register("MenuItem", typeof(RadialMenuItem), typeof(RadialMenuExtensionButton),
            new PropertyMetadata(null));

        #endregion
        
        #endregion

        #region Events

        #region Click

        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _expansionButton = this.GetTemplateChild("PART_ExpansionButton") as Arc;
            if (_expansionButton != null)
            {
                _expansionButton.AddHandler(RadialMenuExtensionButton.MouseEnterEvent, new MouseEventHandler(this.OnMouseEnter));
                _expansionButton.AddHandler(RadialMenuExtensionButton.MouseLeaveEvent, new MouseEventHandler(this.OnMouseLeave));
                _expansionButton.AddHandler(RadialMenuExtensionButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp));
            }
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
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
