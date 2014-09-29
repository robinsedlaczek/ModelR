using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Neumann.TouchControls
{
    public class BreadcrumbBullet : Control
    {

        #region Private Fields
        
        #endregion

        #region Constructors

        public BreadcrumbBullet()
        {
            this.DefaultStyleKey = typeof(BreadcrumbBullet);
        }

        #endregion

        #region Public Properties

        #region IsActive

        public bool IsActive { get { return (bool)GetValue(IsActiveProperty); } set { SetValue(IsActiveProperty, value); } }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(BreadcrumbBullet),
            new PropertyMetadata(false));

        #endregion

        #region Index

        public int Index { get { return (int)GetValue(IndexProperty); } set { SetValue(IndexProperty, value); } }
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(BreadcrumbBullet),
            new PropertyMetadata(0));

        #endregion

        #endregion

        #region Events

        public event EventHandler Click;
        private void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion

        #region Event Handling

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.OnClick();
        }
        
        #endregion

    }
}
