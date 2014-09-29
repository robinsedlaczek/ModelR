using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Neumann.TouchControls
{
    public class AnimationsExtension : MarkupExtension
    {
        public AnimationType AnimationType { get; set; }

        public AnimationsExtension()
        {
        }

        public AnimationsExtension(AnimationType animationType)
        {
            this.AnimationType = animationType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProvider != null)
            {
                var target = targetProvider.TargetObject as FrameworkElement;
                if (target != null)
                {
                    var targetEvent = targetProvider.TargetProperty as EventInfo;
                    if (targetEvent != null)
                    {
                        var delegateType = targetEvent.EventHandlerType;
                        var handler = this.GetType().GetMethod("GenericEventHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                        var del = Delegate.CreateDelegate(delegateType, this, handler);
                        return del;
                    }
                }
            }
            return null;
        }

        private void GenericEventHandler(object sender, EventArgs e)
        {
            var target = sender as FrameworkElement;
            if (target != null)
            {
                if (this.AnimationType != AnimationType.None)
                {
                    var typeName = typeof(AnimationBehavior).Namespace + "." + this.AnimationType.ToString();
                    var type = Type.GetType(typeName);
                    var animation = Activator.CreateInstance(type) as AttachableAnimation;
                    AnimationBehavior.SetAttachedAnimation(target, animation);
                    animation.Attach(target);
                }
            }
        }
    }
}
