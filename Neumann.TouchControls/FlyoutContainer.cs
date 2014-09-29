using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Neumann.TouchControls
{
    public class FlyoutContainer : Selector
    {
        public FlyoutContainer()
        {
            this.DefaultStyleKey = typeof(FlyoutContainer);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var panel = e.AddedItems[0] as FlyoutBase;
            if (panel != null)
            {
                foreach (var item in this.Items)
                {
                    ((FlyoutBase)item).IsOpen = false;
                }
                panel.IsOpen = true;
                panel.Focus();
            }
            base.OnSelectionChanged(e);
        }
    }
}
