using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Neumann.TouchControls
{
    public class RangeSlider : Control
    {

        #region Private Fields

        private Thumb _thumb1;
        private Thumb _thumb2;
        private TranslateTransform _transform1;
        private TranslateTransform _transform2;
        private TransformGroup _trackerTransform;
        private TranslateTransform _rangeTransform;
        private Point _positionOnThumb1;
        private Point _positionOnThumb2;
        private Rectangle _trackBar;
        private bool _updating;
        private TextBlock _rangeLabel;

        #endregion

        #region Constructors

        public RangeSlider()
        {
            this.DefaultStyleKey = typeof(RangeSlider);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region Minimum

        public int Minimum { get { return (int)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(RangeSlider),
            new PropertyMetadata(0, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RangeSlider;
            var value = (int)e.NewValue;
            if (value > element.Maximum)
                element.Minimum = element.Maximum;
            element.UpdateThumbPosition();
            element.UpdateRange();
        }

        #endregion

        #region Maximum

        public int Maximum { get { return (int)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(RangeSlider),
            new PropertyMetadata(100, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RangeSlider;
            var value = (int)e.NewValue;
            if (value < element.Minimum)
                element.Maximum = element.Minimum;
            element.UpdateThumbPosition();
            element.UpdateRange();
        }

        #endregion

        #region Value1

        public int Value1 { get { return (int)GetValue(Value1Property); } set { SetValue(Value1Property, value); } }
        public static readonly DependencyProperty Value1Property =
            DependencyProperty.Register("Value1", typeof(int), typeof(RangeSlider),
            new PropertyMetadata(0, OnValue1PropertyChanged));

        private static void OnValue1PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RangeSlider;
            if (!element.IsLoaded) return;
            var value = (int)e.NewValue;
            if (value > element.Value2)
                element.Value1 = element.Value2;
            if (value < element.Minimum)
                element.Value1 = element.Minimum;
            else if (value > element.Maximum)
                element.Value1 = element.Maximum;
            element.UpdateThumbPosition();
            element.UpdateRange();
        }

        #endregion

        #region Value2

        public int Value2 { get { return (int)GetValue(Value2Property); } set { SetValue(Value2Property, value); } }
        public static readonly DependencyProperty Value2Property =
            DependencyProperty.Register("Value2", typeof(int), typeof(RangeSlider),
            new PropertyMetadata(0, OnValue2PropertyChanged));

        private static void OnValue2PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as RangeSlider;
            if (!element.IsLoaded) return;
            var value = (int)e.NewValue;
            if (value < element.Value1)
                element.Value2 = element.Value1;
            if (value < element.Minimum)
                element.Value2 = element.Minimum;
            else if (value > element.Maximum)
                element.Value2 = element.Maximum;
            element.UpdateThumbPosition();
            element.UpdateRange();
            element.OnValue2Changed();
        }

        #endregion

        #region Range

        public int Range { get { return (int)GetValue(RangeProperty); } private set { SetValue(RangeProperty, value); } }
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(int), typeof(RangeSlider),
            new PropertyMetadata(0, OnRangePropertyChanged));

        private static void OnRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeSlider)d).OnRangeChanged();
        }

        #endregion

        #endregion

        #region Events
        
        #region Value1Changed
        
        public event EventHandler Value1Changed;
        protected virtual void OnValue1Changed()
        {
            if (Value1Changed != null)
                Value1Changed(this, EventArgs.Empty);
        }

        #endregion

        #region Value2Changed
        
        public event EventHandler Value2Changed;
        protected virtual void OnValue2Changed()
        {
            if (Value2Changed != null)
                Value2Changed(this, EventArgs.Empty);
        }

        #endregion

        #region RangeChanged
        
        public event EventHandler RangeChanged;
        protected virtual void OnRangeChanged()
        {
            if (RangeChanged != null)
                RangeChanged(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _thumb1 = this.GetTemplateChild("PART_Thumb1") as Thumb;
            _thumb2 = this.GetTemplateChild("PART_Thumb2") as Thumb;
            _transform1 = this.GetTemplateChild("PART_Transform1") as TranslateTransform;
            _transform2 = this.GetTemplateChild("PART_Transform2") as TranslateTransform;
            _trackerTransform = this.GetTemplateChild("PART_TrackBarTransform") as TransformGroup;
            _rangeTransform = this.GetTemplateChild("PART_RangeTransform") as TranslateTransform;
            _trackBar = this.GetTemplateChild("PART_TrackBar") as Rectangle;
            _rangeLabel = this.GetTemplateChild("PART_RangeLabel") as TextBlock;
            if (_thumb1 != null)
            {
                _thumb1.DragStarted += this.OnDragStarted;
                _thumb1.DragDelta += this.OnDragDelta;
            }
            if (_thumb2 != null)
            {
                _thumb2.DragStarted += this.OnDragStarted2;
                _thumb2.DragDelta += this.OnDragDelta2;
            }
        }

        #endregion

        #region Private Methods

        private void UpdateThumbPosition()
        {
            if (_updating)
            {
                if (_trackerTransform != null && _transform1 != null && _transform2 != null)
                {
                    var scale = _trackerTransform.Children[0] as ScaleTransform;
                    var translate = _trackerTransform.Children[1] as TranslateTransform;
                    if (_transform2.X > _transform1.X)
                        _trackBar.Width = (_transform2.X - _transform1.X);
                    translate.X = _transform1.X + (_thumb1.RenderSize.Width / 2);
                }
                return;
            }
            if (_transform1 != null && _thumb1 != null)
            {
                _transform1.X = (double)(((this.RenderSize.Width - (_thumb1.RenderSize.Width)) / (this.Maximum - this.Minimum)) * (this.Value1 - this.Minimum));
            }
            if (_transform2 != null && _thumb2 != null)
            {
                _transform2.X = (double)(((this.RenderSize.Width - (_thumb2.RenderSize.Width)) / (this.Maximum - this.Minimum)) * (this.Value2 - this.Minimum));
            }
            if (_trackerTransform != null && _transform1 != null && _transform2 != null && _thumb1 != null && _trackBar != null)
            {
                var scale = _trackerTransform.Children[0] as ScaleTransform;
                var translate = _trackerTransform.Children[1] as TranslateTransform;
                if (_transform2.X > _transform1.X)
                    _trackBar.Width = (_transform2.X - _transform1.X);
                translate.X = _transform1.X + (_thumb1.RenderSize.Width / 2);
            }
        }

        private void UpdateRange()
        {
            this.Range = this.Value2 - this.Value1;
        }

        private void UpdateRangeTransform()
        {
            if (_rangeTransform != null && _transform1 != null && _transform2 != null && _rangeLabel != null)
                _rangeTransform.X = ((_transform2.X - _transform1.X) / 2) + (_rangeLabel.RenderSize.Width / 2) + _transform1.X;
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Value1 < this.Minimum)
                this.Value1 = this.Minimum;
            else if (this.Value1 > this.Maximum)
                this.Value1 = this.Maximum;
            if (this.Value2 < this.Minimum)
                this.Value2 = this.Minimum;
            else if (this.Value2 > this.Maximum)
                this.Value2 = this.Maximum;
            if (this.Value1 > this.Value2)
                this.Value1 = this.Value2;
            this.UpdateThumbPosition();
            this.UpdateRangeTransform();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _positionOnThumb1 = Mouse.GetPosition(_thumb1);
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            _updating = true;
            var x = Mouse.GetPosition(this).X - _positionOnThumb1.X;
            if (x < 0)
            {
                x = 0;
            }
            else if (x > this.RenderSize.Width - _thumb1.RenderSize.Width)
            {
                x = this.RenderSize.Width - _thumb1.RenderSize.Width;
            }
            var value = (int)Math.Round((double)(x / ((this.RenderSize.Width - _thumb1.RenderSize.Width) / (this.Maximum - this.Minimum)))) + this.Minimum;
            if (value <= this.Value2)
            {
                _transform1.X = x;
                this.Value1 = value;
            }
            _updating = false;
            this.UpdateRangeTransform();
        }

        private void OnDragStarted2(object sender, DragStartedEventArgs e)
        {
            _positionOnThumb2 = Mouse.GetPosition(_thumb2);
        }

        private void OnDragDelta2(object sender, DragDeltaEventArgs e)
        {
            _updating = true;
            var x = Mouse.GetPosition(this).X - _positionOnThumb2.X;
            if (x < 0)
            {
                x = 0;
            }
            else if (x > this.RenderSize.Width - _thumb2.RenderSize.Width)
            {
                x = this.RenderSize.Width - _thumb2.RenderSize.Width;
            }
            var value = (int)Math.Round((double)(x / ((this.RenderSize.Width - _thumb2.RenderSize.Width) / (this.Maximum - this.Minimum)))) + this.Minimum;
            if (value >= this.Value1)
            {
                _transform2.X = x;
                this.Value2 = value;
            }
            _updating = false;
            this.UpdateRangeTransform();
        }

        #endregion

    }
}
