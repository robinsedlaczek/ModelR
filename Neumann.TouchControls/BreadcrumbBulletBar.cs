using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Neumann.TouchControls
{
    public class BreadcrumbBulletBar : Control
    {

        #region Private Fields

        private Panel _panel;
        internal bool _isUpdating;

        #endregion

        #region Constructors

        public BreadcrumbBulletBar()
        {
            this.DefaultStyleKey = typeof(BreadcrumbBulletBar);
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Properties

        #region BulletCount

        public int BulletCount { get { return (int)GetValue(BulletCountProperty); } set { SetValue(BulletCountProperty, value); } }
        public static readonly DependencyProperty BulletCountProperty =
            DependencyProperty.Register("BulletCount", typeof(int), typeof(BreadcrumbBulletBar),
            new PropertyMetadata(1, OnBulletCountPropertyChanged));

        private static void OnBulletCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BreadcrumbBulletBar)d).CreateBullets((int)e.NewValue);
        }

        #endregion

        #region SelectedIndex

        public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(BreadcrumbBulletBar),
            new PropertyMetadata(0, OnSelectedIndexPropertyChanged));
        
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as BreadcrumbBulletBar;
            if (element._panel == null) return;
            var index = (int)e.NewValue;
            var oldIndex = (int)e.OldValue;
            if (oldIndex < element._panel.Children.Count)
            {
                ((BreadcrumbBullet)element._panel.Children[oldIndex]).IsActive = false;
            }
            if (index < element._panel.Children.Count)
            {
                ((BreadcrumbBullet)element._panel.Children[index]).IsActive = true;
            }
            element.OnSelectedIndexChanged();
        }

        #endregion

        #endregion

        #region Events

        #region SelectedIndexChanged

        public event EventHandler SelectedIndexChanged;
        private void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        #endregion

        #region BulletCountChanged

        public event EventHandler BulletCountChanged;
        private void OnBulletCountChanged()
        {
            if (BulletCountChanged != null)
                BulletCountChanged(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = this.GetTemplateChild("PART_BreadcrumbPanel") as Panel;
        }

        #endregion
        
        #region Private Methods

        private void CreateBullets(int count)
        {
            if (_panel == null) return;
            foreach (var child in _panel.Children)
            {
                var bullet = child as FrameworkElement;
                bullet.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnBulletMouseLeftButtonUp));
            }
            _panel.Children.Clear();
            if (count < 0) return;
            for (int i = 0; i < count; i++)
            {
                var bullet = new BreadcrumbBullet();
                bullet.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnBulletMouseLeftButtonUp));
                _panel.Children.Add(bullet);
            }
            if (count > 0)
            {
                ((BreadcrumbBullet)_panel.Children[0]).IsActive = true;
                this.SelectedIndex = 0;
            }
            this.OnBulletCountChanged();
        }
        
        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CreateBullets(this.BulletCount);
        }

        private void OnBulletMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.SelectedIndex = _panel.Children.IndexOf(sender as UIElement);
        }

        #endregion

    }
}
