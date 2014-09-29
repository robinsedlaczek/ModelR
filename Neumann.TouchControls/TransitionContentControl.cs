using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Neumann.TouchControls
{
    public class TransitionContentControl : ContentControl
    {

        #region Private Fields

        private Storyboard _fadeInStoryboard;
        private Storyboard _fadeOutStoryboard;

        #endregion

        #region Constructors

        public TransitionContentControl()
        {
            this.DefaultStyleKey = typeof(TransitionContentControl);
            this.Loaded += this.OnLoaded;
            var descriptor = DependencyPropertyDescriptor.FromProperty(TransitionContentControl.ContentProperty, typeof(TransitionContentControl));
            if (descriptor != null)
            {
                descriptor.AddValueChanged(this, this.OnContentPropertyChanged);
            }
        }

        #endregion

        #region Properties

        #region TransitionAnimation

        public bool TransitionAnimation { get { return (bool)GetValue(TransitionAnimationProperty); } set { SetValue(TransitionAnimationProperty, value); } }
        public static readonly DependencyProperty TransitionAnimationProperty =
            DependencyProperty.Register("TransitionAnimation", typeof(bool), typeof(TransitionContentControl),
            new PropertyMetadata(true));

        #endregion
        
        #region ContentCore

        internal object ContentCore { get { return (object)GetValue(ContentCoreProperty); } set { SetValue(ContentCoreProperty, value); } }
        public static readonly DependencyProperty ContentCoreProperty =
            DependencyProperty.Register("ContentCore", typeof(object), typeof(TransitionContentControl),
            new PropertyMetadata(null));

        #endregion
        
        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            _fadeInStoryboard = this.GetTemplateChild("PART_FadeInStoryboard") as Storyboard;
            _fadeOutStoryboard = this.GetTemplateChild("PART_FadeOutStoryboard") as Storyboard;
        }

        #endregion

        #region Event Handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ContentCore = this.Content;
            if (this.TransitionAnimation)
            {
                VisualStateManager.GoToState(this, "FadeIn", false);
            }
            else
            {
                var border = this.GetTemplateChild("border") as Border;
                if (border != null)
                    border.Opacity = 1;
            }
        }

        private void OnContentPropertyChanged(object sender, EventArgs e)
        {
            if (this.TransitionAnimation)
            {
                if (_fadeOutStoryboard != null)
                {
                    _fadeOutStoryboard.Completed += this.OnFadeOutStoryboardCompleted;
                    VisualStateManager.GoToState(this, "FadeOut", false);
                }
            }
            else
            {
                this.ContentCore = this.Content;
            }
        }

        private void OnFadeOutStoryboardCompleted(object sender, EventArgs e)
        {
            this.ContentCore = this.Content;
            _fadeOutStoryboard.Completed -= this.OnFadeOutStoryboardCompleted;
            VisualStateManager.GoToState(this, "FadeIn", false);
        }

        #endregion

    }
}
