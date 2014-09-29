using Microsoft.Expression.Shapes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    public class RadialPicker : Control
    {

        #region Private Fields

        private const double OUTER_DISTANCE = -1d;
        private const double PART_LENGTH = 44d;
        private Panel _pointsPanel;
        private Panel _labelsPanel;
        private Arc _indicator;
        private Line _valueLine;
        private Line _indicatorLine;
        private Ellipse _innerCircle;
        private Ellipse _outerCircle;
        private Label _toolTip;
        private RadialImageButton _centerButton;

        #endregion

        #region Constructors

        public RadialPicker()
        {
            this.DefaultStyleKey = typeof(RadialPicker);
            var descriptor = DependencyPropertyDescriptor.FromProperty(RadialPicker.IsEnabledProperty, typeof(RadialPicker));
            if (descriptor != null)
            {
                descriptor.AddValueChanged(this, this.ToggleIsEnabled);
            }
            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;
            this.AddHandler(RadialPicker.MouseEnterEvent, new MouseEventHandler(this.OnMouseEnter));
            this.AddHandler(RadialPicker.MouseLeaveEvent, new MouseEventHandler(this.OnMouseLeave));
            this.AddHandler(RadialPicker.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp));
        }

        #endregion

        #region Properties

        #region Minimum

        public int Minimum { get { return (int)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(-1, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region Maximum

        public int Maximum { get { return (int)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(-1, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region Value

        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(-1, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.PositionValue();
            element.OnValueChanged();
        }

        #endregion

        #region Distance

        public int Distance { get { return (int)GetValue(DistanceProperty); } set { SetValue(DistanceProperty, value); } }
        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(int), typeof(RadialPicker),
            new PropertyMetadata(0, OnDistancePropertyChanged));

        private static void OnDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RadialPicker;
            element.CreatePoints();
        }

        #endregion

        #region StartAngle

        public double StartAngle { get { return (double)GetValue(StartAngleProperty); } set { SetValue(StartAngleProperty, value); } }
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialPicker),
            new PropertyMetadata(-1d));

        #endregion

        #region EndAngle

        public double EndAngle { get { return (double)GetValue(EndAngleProperty); } set { SetValue(EndAngleProperty, value); } }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(RadialPicker),
            new PropertyMetadata(-1d));

        #endregion

        #region AcceptOnlyStepValues

        public bool AcceptOnlyStepValues { get { return (bool)GetValue(AcceptOnlyStepValuesProperty); } set { SetValue(AcceptOnlyStepValuesProperty, value); } }
        public static readonly DependencyProperty AcceptOnlyStepValuesProperty =
            DependencyProperty.Register("AcceptOnlyStepValues", typeof(bool), typeof(RadialPicker),
            new PropertyMetadata(false));

        #endregion

        #endregion

        #region Events

        #region ValueChanged

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Click

        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
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
            _toolTip = this.GetTemplateChild("PART_ToolTip") as Label;
            _pointsPanel = this.GetTemplateChild("PART_PointsPanel") as Panel;
            _labelsPanel = this.GetTemplateChild("PART_LabelsPanel") as Panel;
            _indicator = this.GetTemplateChild("PART_Indicator") as Arc;
            _valueLine = this.GetTemplateChild("PART_ValueLine") as Line;
            _indicatorLine = this.GetTemplateChild("PART_IndicatorLine") as Line;
            _innerCircle = this.GetTemplateChild("PART_InnerCircle") as Ellipse;
            _outerCircle = this.GetTemplateChild("PART_OuterCircle") as Ellipse;
            _centerButton = this.GetTemplateChild("PART_CenterButton") as RadialImageButton;
            if (_innerCircle != null)
            {
                _innerCircle.MouseMove += this.OnMouseMove;
            }
            if (_outerCircle != null)
            {
                _outerCircle.MouseMove += this.OnMouseMove;
            }
            if (_centerButton != null)
            {
                _centerButton.Click += this.OnCenterButtonClick;
            }
        }

        #endregion

        #region Private Methods

        private void CreatePoints()
        {
            if (_pointsPanel == null) return;
            _pointsPanel.Children.Clear();
            _labelsPanel.Children.Clear();
            var count = (this.Maximum - this.Minimum) / this.Distance;
            var value = this.Minimum;
            for (int i = 0; i < count; i++)
            {
                var point = new Line()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = new SolidColorBrush(Color.FromRgb(128, 57, 123)),
                    StrokeThickness = 2.5,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    SnapsToDevicePixels = true,
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 3,
                };
                _pointsPanel.Children.Add(point);
                var label = new TextBlock()
                {
                    Text = value.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    SnapsToDevicePixels = true,
                };
                _labelsPanel.Children.Add(label);
                value += Distance;
            }
            this.PositionPoints();
        }

        private void PositionPoints()
        {
            if (_pointsPanel != null && _pointsPanel.Children.Count > 0)
            {
                var startAngle = 0;
                var count = (this.Maximum - this.Minimum) / this.Distance;
                var angleDistance = 360 / count;
                for (int i = 0; i < count; i++)
                {
                    var point = _pointsPanel.Children[i] as Line;
                    var translate = new TranslateTransform();

                    var radius = (this.RenderSize.Height / 2) - OUTER_DISTANCE;
                    var angle = startAngle;
                    var radian = (angle * Math.PI) / 180;

                    translate.X = (int)radius * Math.Sin(radian);
                    translate.Y = (int)((radius * Math.Cos(radian)) * -1);

                    var group = new TransformGroup();
                    group.Children.Add(new RotateTransform(angle));
                    group.Children.Add(translate);
                    point.RenderTransform = group;

                    if (_labelsPanel != null && _labelsPanel.Children.Count > 0)
                    {
                        var label = _labelsPanel.Children[i] as TextBlock;
                        var labelTranslate = new TranslateTransform();
                        var labelRadius = (this.RenderSize.Height / 2) - -20d;
                        var labelAngle = startAngle;
                        var labelRadian = (labelAngle * Math.PI) / 180;
                        labelTranslate.X = (int)labelRadius * Math.Sin(labelRadian);
                        labelTranslate.Y = (int)((labelRadius * Math.Cos(labelRadian)) * -1);
                        label.RenderTransform = labelTranslate;
                    }
                    startAngle += angleDistance;
                }
            }
            this.PositionValue();
        }

        private void PositionValue()
        {
            if (_valueLine == null) return;
            var startAngle = 0;
            var count = this.AcceptOnlyStepValues ? (this.Maximum - this.Minimum) / this.Distance : this.Maximum - this.Minimum;
            var angleDistance = 360 / count;
            this.StartAngle = startAngle;
            this.EndAngle = this.AcceptOnlyStepValues ? (this.Value / this.Distance) * angleDistance : ((this.Value * angleDistance) * 1.19);
            _valueLine.RenderTransform = new RotateTransform(this.EndAngle);
        }

        private int CalculateValue(Point pos)
        {
            var origin = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
            var angle = MathHelpers.AngleFromPoint(origin, pos);
            var count = this.AcceptOnlyStepValues ? (this.Maximum - this.Minimum) / this.Distance : this.Maximum - this.Minimum;
            var angleDistance = 360 / count;
            var value = 0;
            if (this.AcceptOnlyStepValues)
            {
                value = (int)Math.Round(((angle / angleDistance) * this.Distance) + (this.Distance / 2));
                var v = Math.Round((double)(value / this.Distance)) * this.Distance;
                value = (int)v;
            }
            else
            {
                value = (int)((angle / angleDistance) / 1.19);
            }
            if (value > this.Maximum)
                value = this.Maximum;
            return value;
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CreatePoints();
            this.ToggleIsEnabled(sender, e);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_valueLine != null)
                _valueLine.Y1 = (this.RenderSize.Height / 2) * -1;
            if (_indicatorLine != null)
                _indicatorLine.Y1 = (this.RenderSize.Height / 2) * -1; 
            this.PositionPoints();
            if (_toolTip != null)
            {
                _toolTip.Margin = new Thickness(0, 0, (this.RenderSize.Width * 2.1) * -1, 0);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_indicatorLine != null)
            {
                var origin = new Point(this.RenderSize.Width / 2, this.RenderSize.Height / 2);
                var pos = e.GetPosition(this);
                var angle = MathHelpers.AngleFromPoint(origin, pos);
                _indicatorLine.RenderTransform = new RotateTransform(angle);
                var value = this.CalculateValue(pos);
                if (_toolTip != null)
                {
                    _toolTip.Content = value;
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image || e.OriginalSource is RadialImageButton)
                return;
            var pos = e.GetPosition(this);
            this.Value = this.CalculateValue(pos);
            VisualStateManager.GoToState(this, "MouseLeave", false);
            this.OnClick();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_indicatorLine != null)
                _indicatorLine.Visibility = Visibility.Visible;
            if (_toolTip != null)
                _toolTip.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(this, "MouseEnter", false);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_indicatorLine != null)
                _indicatorLine.Visibility = Visibility.Collapsed;
            if (_toolTip != null)
                _toolTip.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "MouseLeave", false);
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

        private void OnCenterButtonClick(object sender, EventArgs e)
        {
            this.OnClosed();
        }

        #endregion

    }
}
