using System;
using System.ComponentModel;
using System.Windows;

namespace Neumann.TouchControls
{
    public class AnimationBehavior
    {
        public static readonly DependencyProperty AttachAnimationProperty =
            DependencyProperty.RegisterAttached("AttachAnimation", typeof(AnimationType), typeof(AnimationBehavior),
            new PropertyMetadata(AnimationType.None, OnAttachAnimationPropertyChanged));

        private static void OnAttachAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animationType = (AnimationType)e.NewValue;
            var element = d as FrameworkElement;
            if (element != null)
            {
                if (DesignerProperties.GetIsInDesignMode(element)) return;
                if (animationType != AnimationType.None)
                {
                    var typeName = typeof(AnimationBehavior).Namespace + "." + animationType.ToString();
                    var type = Type.GetType(typeName);
                    var animation = Activator.CreateInstance(type) as AttachableAnimation;
                    SetAttachedAnimation(element, animation);
                    animation.AnimationDirection = GetDirection((UIElement)d);
                    animation.Attach(element);
                }
                else
                {
                    var animation = GetAttachedAnimation(element) as AttachableAnimation;
                    if (animation != null)
                        animation.Detach();
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static AnimationType GetAttachAnimation(UIElement element)
        {
            return (AnimationType)element.GetValue(AttachAnimationProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetAttachAnimation(UIElement element, AnimationType value)
        {
            element.SetValue(AttachAnimationProperty, value);
        }
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached("Direction", typeof(AnimationDirection), typeof(AnimationBehavior),
            new PropertyMetadata(AnimationDirection.Default, OnDirectionPropertyChanged));

        private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var direction = (AnimationDirection)e.NewValue;
            var element = d as FrameworkElement;
            if (element != null)
            {
                var animation = GetAttachedAnimation(element) as AttachableAnimation;
                if (animation != null)
                {
                    animation.AnimationDirection = direction;
                }
            }
        }

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
        
        internal static readonly DependencyProperty AttachedAnimationProperty =
            DependencyProperty.RegisterAttached("AttachedAnimation", typeof(AttachableAnimation), typeof(AnimationBehavior),
            new PropertyMetadata(null));

        internal static AttachableAnimation GetAttachedAnimation(UIElement element)
        {
            return element.GetValue(AttachedAnimationProperty) as AttachableAnimation;
        }

        internal static void SetAttachedAnimation(UIElement element, AttachableAnimation value)
        {
            element.SetValue(AttachedAnimationProperty, value);
        }
    }
}
