using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    public class ToggleSwitch : Control
    {

        #region Private Fields

        private Border _border;
        private Thumb _thumb;
        private TranslateTransform _curtainTransform;
        private TranslateTransform _knobTransform;
        private double DRAG_DELTA = 2;

        #endregion

        #region Constructors

        public ToggleSwitch()
        {
            this.DefaultStyleKey = typeof(ToggleSwitch);
            this.GotFocus += (s, e) => this.ChangeState("Focused");
            this.LostFocus += (s, e) => this.ChangeState("Unfocused");
            this.MouseLeftButtonDown += (s, e) => this.ChangeState("Pressed");
            this.MouseEnter += (s, e) => this.ChangeState("PointerOver");
            this.MouseLeave += (s, e) => this.ChangeState("Normal");
            var desc = DependencyPropertyDescriptor.FromProperty(ToggleSwitch.IsEnabledProperty, typeof(ToggleSwitch));
            if (desc != null)
            {
                desc.AddValueChanged(this, (s, e) => this.ChangeState(this.IsEnabled ? "Normal" : "Disabled"));
            }
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ChangeState(this.IsOn ? "OnContent" : "OffContent");
            this.ChangeState(this.IsOn ? "On" : "Off");
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _curtainTransform = this.GetTemplateChild("CurtainTranslateTransform") as TranslateTransform;
            _knobTransform = this.GetTemplateChild("KnobTranslateTransform") as TranslateTransform;
            _border = this.GetTemplateChild("PART_Border") as Border;
            _thumb = this.GetTemplateChild("SwitchThumb") as Thumb;
            if (_thumb != null)
            {
                _thumb.DragCompleted += OnThumbDragCompleted;
                _thumb.DragDelta += OnThumbDragDelta;
            }
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_curtainTransform == null) return;
            if (e.HorizontalChange != 0 && (_knobTransform.X <= 36 || _knobTransform.X == 38) && _knobTransform.X >= 0)
            {
                var ani = new DoubleAnimation();
                ani.By = e.HorizontalChange > 0 ? DRAG_DELTA : DRAG_DELTA * -1;
                ani.Duration = new TimeSpan();
                _curtainTransform.BeginAnimation(TranslateTransform.XProperty, ani);

                var ani2 = new DoubleAnimation();
                ani2.By = e.HorizontalChange > 0 ? DRAG_DELTA : DRAG_DELTA * -1;
                ani2.Duration = new TimeSpan();
                _knobTransform.BeginAnimation(TranslateTransform.XProperty, ani2);
            }
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.HorizontalChange == 0)
                this.IsOn = !this.IsOn;
            else
                this.IsOn = e.HorizontalChange > 0;
        }

        #endregion

        #region Private Methods

        private void ChangeState(string stateName)
        {
            if (_border == null) return;
            VisualStateManager.GoToElementState(_border, stateName, false);
        }

        #endregion

        #region Properties

        #region OnContent

        public object OnContent { get { return (object)GetValue(OnContentProperty); } set { SetValue(OnContentProperty, value); } }
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register("OnContent", typeof(object), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #region OffContent

        public object OffContent { get { return (object)GetValue(OffContentProperty); } set { SetValue(OffContentProperty, value); } }
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register("OffContent", typeof(object), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #region IsOn

        public bool IsOn { get { return (bool)GetValue(IsOnProperty); } set { SetValue(IsOnProperty, value); } }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleSwitch),
            new PropertyMetadata(false, OnIsOnPropertyChanged));

        private static void OnIsOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ToggleSwitch;
            var value = (bool)e.NewValue;
            element.ChangeState(value ? "OnContent" : "OffContent");
            element.ChangeState(value ? "On" : "Off");
            if (element.IsLoaded)
                element.OnToggled();
        }

        #endregion

        #region OnContentTemplate

        public DataTemplate OnContentTemplate { get { return (DataTemplate)GetValue(OnContentTemplateProperty); } set { SetValue(OnContentTemplateProperty, value); } }
        public static readonly DependencyProperty OnContentTemplateProperty =
            DependencyProperty.Register("OnContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #region OffContentTemplate

        public DataTemplate OffContentTemplate { get { return (DataTemplate)GetValue(OffContentTemplateProperty); } set { SetValue(OffContentTemplateProperty, value); } }
        public static readonly DependencyProperty OffContentTemplateProperty =
            DependencyProperty.Register("OffContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #region Header

        public object Header { get { return (object)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate { get { return (DataTemplate)GetValue(HeaderTemplateProperty); } set { SetValue(HeaderTemplateProperty, value); } }
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        #region Toggled

        public event EventHandler Toggled;
        protected virtual void OnToggled()
        {
            if (Toggled != null)
                Toggled(this, EventArgs.Empty);
        }

        #endregion

        #endregion

    }
}
