using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Neumann.TouchControls
{
    public abstract class AttachableAnimation
    {
        private const string TEMPLATE_FILE = "AttachableAnimations.xaml";
        protected Storyboard Storyboard { get; set; }
        protected FrameworkElement AnimationTarget { get; set; }
        protected virtual bool StartOnLoad { get; set; }

        public AttachableAnimation()
        {
            this.StartOnLoad = true;
        }

        public void Attach(FrameworkElement element)
        {
            if (IsInDesigner) return;
            this.AnimationTarget = element;
            if (!element.IsLoaded)
            {
                element.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(this.OnLoaded));
            }
            else
            {
                this.OnLoaded(element, EventArgs.Empty);
            }
            element.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(this.OnUnloaded));
        }

        public void Detach()
        {
            if (IsInDesigner) return;
            if (this.AnimationTarget != null)
            {
                this.AnimationTarget.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(this.OnLoaded));
                this.AnimationTarget.RemoveHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(this.OnUnloaded));
                this.OnDetaching();
                for (int i = 0; i < this.AnimationTarget.Triggers.Count; i++)
                {
                    var trigger = this.AnimationTarget.Triggers[i];
                    foreach (var action in trigger.EnterActions)
                    {
                        var begin = action as BeginStoryboard;
                        if (begin != null)
                        {
                            begin.Storyboard.Stop();
                        }
                    }
                    this.AnimationTarget.Triggers.Remove(trigger);
                }
                this.AnimationTarget = null;
                this.Storyboard = null;
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            var element = sender as FrameworkElement;
            element.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(this.OnLoaded));
            this.OnAttached(element);
            this.AttachAnimation(element);
        }

        private void OnUnloaded(object sender, EventArgs e)
        {
            this.Detach();
        }

        protected virtual void OnAttached(FrameworkElement associatedObject) { }
        protected virtual void OnDetaching() { }

        protected abstract string ProvideAnimationResourceName();

        protected ResourceDictionary ProvideResourceDictionary()
        {
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("/" + this.GetType().Assembly.GetName().Name +
                ";component/" + TEMPLATE_FILE, UriKind.Relative);
            return dictionary;
        }

        private void AttachAnimation(FrameworkElement associatedObject)
        {
            var dictionary = this.ProvideResourceDictionary();
            if (dictionary != null)
            {
                var obj = dictionary[this.ProvideAnimationResourceName()];
                if (obj is Storyboard)
                {
                    this.Storyboard = obj as Storyboard;
                    foreach (var item in this.Storyboard.Children)
                    {
                        item.SetValue(Storyboard.TargetProperty, associatedObject);
                    }
                }
                else if (obj is AnimationPlaceholder)
                {
                    var destinationElement = associatedObject as FrameworkElement;
                    AnimationTarget = destinationElement;
                    if (destinationElement == null) return;
                    var placeholder = obj as AnimationPlaceholder;
                    this.Storyboard = this.ClonePlaceholderItems(placeholder, destinationElement);
                    this.Storyboard.Completed += this.OnStoryboardCompleted;
                    if (this.StartOnLoad)
                        this.Storyboard.Begin();
                }
            }
        }

        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            var storyboard = sender as Storyboard;
            if (storyboard == null) return;
            storyboard.Completed -= this.OnStoryboardCompleted;
            var destinationElement = AnimationTarget as FrameworkElement;
            destinationElement.RenderTransform = null;
        }

        protected Storyboard ClonePlaceholderItems(AnimationPlaceholder placeholder, FrameworkElement destinationElement)
        {
            if (placeholder.Triggers.Count == 0) return null;
            var orgTrigger = placeholder.Triggers[0] as EventTrigger;
            if (orgTrigger == null) return null;
            var orgBeginStoryboard = orgTrigger.Actions[0] as BeginStoryboard;
            if (orgBeginStoryboard == null) return null;
            var orgStoryboard = orgBeginStoryboard.Storyboard;
            destinationElement.RenderTransformOrigin = placeholder.RenderTransformOrigin;
            destinationElement.RenderTransform = placeholder.RenderTransform.Clone();
            destinationElement.LayoutTransform = placeholder.LayoutTransform.Clone();
            var storyboard = new Storyboard();
            foreach (var ani in orgStoryboard.Children)
            {
                var newAnimation = ani.Clone();
                storyboard.Children.Add(newAnimation);
                newAnimation.SetValue(Storyboard.TargetProperty, destinationElement);
            }
            var trigger = new EventTrigger(orgTrigger.RoutedEvent);
            trigger.Actions.Add(new BeginStoryboard() { Storyboard = storyboard });
            destinationElement.Triggers.Add(trigger);
            this.OnStoryboardPrepared(storyboard);
            return storyboard;
        }

        protected virtual void OnStoryboardPrepared(Storyboard storyboard)
        {
        }

        private static bool IsInDesigner
        {
            get
            {
                return Application.Current == null ||
                       Application.Current.MainWindow == null ||
                       DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow);
            }
        }

        public AnimationDirection AnimationDirection { get; set; }
    }
}
