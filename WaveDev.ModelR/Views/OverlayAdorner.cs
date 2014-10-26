using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WaveDev.ModelR.Views
{
    /// <summary> 
    /// Overlays a control with the specified content.
    /// </summary> 
    /// <typeparam name="TOverlay">The type of content to create the overlay from.</typeparam> 
    public class OverlayAdorner<TOverlay> : Adorner, IDisposable where TOverlay : UIElement, new()
    {
        #region Private Fields

        private UIElement _adorningElement;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Layer.Remove(this);
        }

        #endregion

        #region Public Static Members

        /// <summary> 
        /// Overlays the element with the specified instance of TOverlay.
        /// </summary> 
        /// <param name="elementToAdorn">Element to overlay.</param> 
        /// <param name="adorningElement">The content of the overlay.</param> 
        /// <returns>Returns the overlay disposable.</returns> 
        public static IDisposable Overlay(UIElement elementToAdorn, TOverlay adorningElement)
        {
            var adorner = new OverlayAdorner<TOverlay>(elementToAdorn, adorningElement);

            adorner.Layer = AdornerLayer.GetAdornerLayer(elementToAdorn);
            adorner.Layer.Add(adorner);

            return adorner as IDisposable;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Private ctor. Use the static Overlay method to get an instance of the overlay adorner.
        /// </summary>
        /// <param name="elementToAdorn">Element to overlay.</param> 
        /// <param name="adorningElement">The content of the overlay.</param> 
        private OverlayAdorner(UIElement elementToAdorn, UIElement adorningElement)
            : base(elementToAdorn)
        {
            _adorningElement = adorningElement;

            if (adorningElement != null)
                AddVisualChild(adorningElement);

            Focusable = true;
        }

        #endregion

        #region Private Members

        private AdornerLayer Layer
        {
            get;
            set;
        }

        #endregion

        #region Overrides

        protected override int VisualChildrenCount
        {
            get
            {
                return _adorningElement == null ? 0 : 1;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_adorningElement != null)
            {
                var adorningPoint = new Point(0, 0);
                _adorningElement.Arrange(new Rect(adorningPoint, AdornedElement.DesiredSize));
            }

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && _adorningElement != null)
                return _adorningElement;

            return base.GetVisualChild(index);
        }

        #endregion

    }
}
