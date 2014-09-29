using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Neumann.TouchControls
{
    public class SizeAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "WidthAnimation";
        }

        protected override void OnAttached(FrameworkElement associatedObject)
        {
            var button = associatedObject as Button;
            if (button != null)
                associatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.OnClick));
        }

        protected override void OnDetaching()
        {
            var button = this.AnimationTarget as Button;
            if (button != null)
                button.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(this.OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (this.Storyboard != null)
                this.Storyboard.Begin();
        }
    }

    public class GrowingAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            this.StartOnLoad = false;
            return "GrowingAnimation";
        }
    }

    public class EntryAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "EntryAnimation";
        }
    }

    public class ExpansionLayoutAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "ExpansionLayoutAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown ||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    }
                }
            }
        }
    }

    public class ExpansionRenderAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "ExpansionRenderAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (this.AnimationDirection == AnimationDirection.LeftToRight)
            {
                this.AnimationTarget.RenderTransformOrigin = new Point(0, 0);
            }
            else if (this.AnimationDirection == AnimationDirection.RightToLeft)
            {
                this.AnimationTarget.RenderTransformOrigin = new Point(1, 1);
            }
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {

                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown ||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    }
                }
            }
        }
    }

    public class ContractionLayoutAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "ContractionLayoutAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown ||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.LayoutTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    }
                }
            }
        }
    }

    public class ContractionRenderAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "ContractionRenderAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (this.AnimationDirection == AnimationDirection.LeftToRight)
            {
                this.AnimationTarget.RenderTransformOrigin = new Point(0, 0);
            }
            else if (this.AnimationDirection == AnimationDirection.RightToLeft)
            {
                this.AnimationTarget.RenderTransformOrigin = new Point(1, 1);
            }
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {

                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown ||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    }
                }
            }
        }
    }

    public class SlideInRenderAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "SlideInRenderAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.From = this.AnimationTarget.RenderSize.Width;
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.From = this.AnimationTarget.RenderSize.Height;
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
                    }

                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.TopDown)
                    {
                        animation.From = animation.From *-1;
                        this.AnimationTarget.RenderTransformOrigin = new Point(0, 0);
                    }
                    else if (this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.BottomUp ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        this.AnimationTarget.RenderTransformOrigin = new Point(1, 1);
                    }
                }
            }
        }
    }

    public class SlideOutRenderAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "SlideOutRenderAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as DoubleAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        animation.To = this.AnimationTarget.RenderSize.Width;
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
                    }
                    else if (this.AnimationDirection == AnimationDirection.TopDown ||
                        this.AnimationDirection == AnimationDirection.BottomUp)
                    {
                        animation.To = this.AnimationTarget.RenderSize.Height;
                        animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
                    }

                    if (this.AnimationDirection == AnimationDirection.LeftToRight ||
                        this.AnimationDirection == AnimationDirection.TopDown)
                    {
                        animation.To = animation.To * -1;
                        this.AnimationTarget.RenderTransformOrigin = new Point(0, 0);
                    }
                    else if (this.AnimationDirection == AnimationDirection.RightToLeft ||
                        this.AnimationDirection == AnimationDirection.BottomUp ||
                        this.AnimationDirection == AnimationDirection.Default)
                    {
                        this.AnimationTarget.RenderTransformOrigin = new Point(1, 1);
                    }
                }
            }
        }
    }

    public class SlideInLayoutAnimation : AttachableAnimation
    {
        private Thickness _orginalMargin;

        protected override string ProvideAnimationResourceName()
        {
            _orginalMargin = this.AnimationTarget.Margin;
            return "SlideInLayoutAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as ThicknessAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight)
                        animation.From = new Thickness((this.AnimationTarget.RenderSize.Width * -1), 0, 0, 0);
                    else if (this.AnimationDirection == AnimationDirection.RightToLeft || this.AnimationDirection == AnimationDirection.Default)
                        animation.From = new Thickness(0, 0, (this.AnimationTarget.RenderSize.Width * -1), 0);
                    else if (this.AnimationDirection == AnimationDirection.TopDown)
                        animation.From = new Thickness(0, (this.AnimationTarget.RenderSize.Height * -1), 0, 0);
                    else if (this.AnimationDirection == AnimationDirection.BottomUp)
                        animation.From = new Thickness(0, 0, 0, (this.AnimationTarget.RenderSize.Height * -1));
                }
            }
        }

        protected override void OnDetaching()
        {
            this.AnimationTarget.Margin = _orginalMargin;
            base.OnDetaching();
        }
    }

    public class SlideOutLayoutAnimation : AttachableAnimation
    {
        private Thickness _orginalMargin;

        protected override string ProvideAnimationResourceName()
        {
            _orginalMargin = this.AnimationTarget.Margin;
            return "SlideOutLayoutAnimation";
        }

        protected override void OnStoryboardPrepared(Storyboard storyboard)
        {
            if (storyboard.Children.Count > 0)
            {
                var animation = storyboard.Children[0] as ThicknessAnimation;
                if (animation != null)
                {
                    if (this.AnimationDirection == AnimationDirection.LeftToRight)
                        animation.To = new Thickness((this.AnimationTarget.RenderSize.Width * -1), 0, 0, 0);
                    else if (this.AnimationDirection == AnimationDirection.RightToLeft || this.AnimationDirection == AnimationDirection.Default)
                        animation.To = new Thickness(0, 0, (this.AnimationTarget.RenderSize.Width * -1), 0);
                    else if (this.AnimationDirection == AnimationDirection.TopDown)
                        animation.To = new Thickness(0, (this.AnimationTarget.RenderSize.Height * -1), 0, 0);
                    else if (this.AnimationDirection == AnimationDirection.BottomUp)
                        animation.To = new Thickness(0, 0, 0, (this.AnimationTarget.RenderSize.Height * -1));
                }
            }
        }

        protected override void OnDetaching()
        {
            this.AnimationTarget.Margin = _orginalMargin;
            base.OnDetaching();
        }
    }

    public class FadeOutAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "FadeOutAnimation";
        }
    }

    public class FadeInAnimation : AttachableAnimation
    {
        protected override string ProvideAnimationResourceName()
        {
            return "FadeInAnimation";
        }
    }
}
