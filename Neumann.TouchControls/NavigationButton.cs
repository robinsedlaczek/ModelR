using System.Windows;
using System.Windows.Controls;

namespace Neumann.TouchControls
{
    public class NavigationButton : Button
    {
        
        #region Constructors

        public NavigationButton()
        {
            this.DefaultStyleKey = typeof(NavigationButton);
        }

        #endregion

        #region Properties

        #region Direction

        public NavigationButtonDirection Direction { get { return (NavigationButtonDirection)GetValue(DirectionProperty); } set { SetValue(DirectionProperty, value); } }
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(NavigationButtonDirection), typeof(NavigationButton),
            new PropertyMetadata(NavigationButtonDirection.Left));

        #endregion

        #endregion

    }

    #region NavigationButtonDirection

    public enum NavigationButtonDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    #endregion

}
