using System.Windows;

namespace Neumann.TouchControls
{
    public class TargetAnimationBehavior
    {

        #region IsOn

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.RegisterAttached("IsOn", typeof(bool), typeof(TargetAnimationBehavior),
            new PropertyMetadata(false, OnIsOnPropertyChanged));

        private static void OnIsOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            var isOn = (bool)e.NewValue;
            var target = GetTarget(element);
            if (target != null)
            {
                var direction = GetDirection(element);
                var onAnimation = GetOnAnimation(element);
                var offAnimation = GetOffAnimation(element);
                AnimationBehavior.SetDirection(target, direction);
                AnimationBehavior.SetAttachAnimation(target, isOn ? onAnimation : offAnimation);
            }
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetIsOn(UIElement element)
        {
            return (bool)element.GetValue(IsOnProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetIsOn(UIElement element, bool value)
        {
            element.SetValue(IsOnProperty, value);
        }

        #endregion

        #region Target

        public static readonly DependencyProperty TargetProperty =
                    DependencyProperty.RegisterAttached("Target", typeof(UIElement), typeof(TargetAnimationBehavior),
                    new PropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static UIElement GetTarget(UIElement element)
        {
            return (UIElement)element.GetValue(TargetProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetTarget(UIElement element, UIElement value)
        {
            element.SetValue(TargetProperty, value);
        }

        #endregion

        #region OnAnimation

        public static readonly DependencyProperty OnAnimationProperty =
            DependencyProperty.RegisterAttached("OnAnimation", typeof(AnimationType), typeof(TargetAnimationBehavior),
            new PropertyMetadata(AnimationType.SlideOutLayoutAnimation));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static AnimationType GetOnAnimation(UIElement element)
        {
            return (AnimationType)element.GetValue(OnAnimationProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetOnAnimation(UIElement element, AnimationType value)
        {
            element.SetValue(OnAnimationProperty, value);
        }

        #endregion

        #region OffAnimation

        public static readonly DependencyProperty OffAnimationProperty =
                    DependencyProperty.RegisterAttached("OffAnimation", typeof(AnimationType), typeof(TargetAnimationBehavior),
                    new PropertyMetadata(AnimationType.SlideInLayoutAnimation));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static AnimationType GetOffAnimation(UIElement element)
        {
            return (AnimationType)element.GetValue(OffAnimationProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetOffAnimation(UIElement element, AnimationType value)
        {
            element.SetValue(OffAnimationProperty, value);
        }

        #endregion

        #region Direction

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached("Direction", typeof(AnimationDirection), typeof(TargetAnimationBehavior),
            new PropertyMetadata(AnimationDirection.Default));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static AnimationDirection GetDirection(UIElement element)
        {
            return (AnimationDirection)element.GetValue(DirectionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetDirection(UIElement element, AnimationDirection value)
        {
            element.SetValue(DirectionProperty, value);
        }

        #endregion

    }
}
