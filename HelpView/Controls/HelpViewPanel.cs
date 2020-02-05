using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace HelpView.Controls
{

    public class HelpViewPanel : Grid
    {
        private static List<HelpViewPanel> instances = new List<HelpViewPanel>();
        //public static List<HelpViewPanel> Instances { get => instances; }

        private bool isShow = false;
        private Dictionary<string, HelpObject> helpObjects = new Dictionary<string, HelpObject>();
        private Action clickNextHelpObjectAction;


        #region AttachedProperty
        public static string GetAttached(DependencyObject obj)
        {
            return (string)obj.GetValue(AttachedProperty);
        }

        public static void SetAttached(DependencyObject obj, string value)
        {
            obj.SetValue(AttachedProperty, value);
        }

        public static readonly DependencyProperty AttachedProperty =
            DependencyProperty.RegisterAttached("Attached", typeof(string), typeof(HelpViewPanel), new PropertyMetadata(string.Empty, AttachedPropertyChange));
        
        private static void AttachedPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is string name && !string.IsNullOrEmpty(name))
            {
                element.Loaded += (sender, e) => 
                {
                    if (sender is UIElement uIElement)
                        HelpViewPanel.instances.Where(w => w.Name == name).FirstOrDefault()?.RegisterAttached(uIElement);
                    
                };
                element.Unloaded += (sender, e) => 
                {
                    if (sender is UIElement uIElement)
                        HelpViewPanel.instances.Where(w => w.Name == name).FirstOrDefault()?.RegisterUnttached(uIElement);
                };
            }
        }
        #endregion

        #region DependencyProperty
        
        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow", typeof(bool), typeof(HelpViewPanel), new PropertyMetadata(false, ChangeInvalideteVisual));

        private static void ChangeInvalideteVisual(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HelpViewPanel helpViewPanel && e.NewValue is bool value)
            {
                helpViewPanel.isShow = value;
                if (value)
                    helpViewPanel.OnShow();
                else
                    helpViewPanel.OnHidden();
            }
        }

        public TimeSpan DurationViewHelpObject
        {
            get { return (TimeSpan)GetValue(DurationViewHelpObjectProperty); }
            set { SetValue(DurationViewHelpObjectProperty, value); }
        }

        public static readonly DependencyProperty DurationViewHelpObjectProperty =
            DependencyProperty.Register("DurationViewHelpObject", typeof(TimeSpan), typeof(HelpViewPanel), new PropertyMetadata(TimeSpan.Zero));



        public List<ScriptObject> Script
        {
            get { return (List<ScriptObject>)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value); }
        }

        public static readonly DependencyProperty ScriptProperty =
            DependencyProperty.Register("Script", typeof(List<ScriptObject>), typeof(HelpViewPanel), new PropertyMetadata(new List<ScriptObject>()));

        #endregion

        public HelpViewPanel()
        {
            instances.Add(this);

            this.Opacity = 0;
            Visibility = Visibility.Collapsed;
        }

        private void RegisterAttached(UIElement uIElement)
        {
            if (uIElement.GetValue(FrameworkElement.NameProperty) is string name 
                && !string.IsNullOrEmpty(name) 
                && uIElement.IsVisible
                && !helpObjects.ContainsKey(name))
            {
                var helpObject = new HelpObject(uIElement);
                helpObjects.Add(name, helpObject);
            }
        }

        private void RegisterUnttached(UIElement uIElement)
        {
            if (uIElement.GetValue(FrameworkElement.NameProperty) is string name 
                && !string.IsNullOrEmpty(name) 
                && helpObjects.TryGetValue(name, out HelpObject helpObject))
            {                
                helpObjects.Remove(name);
            }
        }
        

        private IEnumerable<ScriptObject> GetScripts(List<string> names)
        {
            //Binding registered controls and scripts
            var script = Script.Where(w => w.IsShow && names.Contains(w.NameHelpObject)).Select(s =>
            {
                s.HelpObject = helpObjects[s.NameHelpObject];
                return s;
            });


            foreach (var itemScript in script)
            {
                yield return itemScript;

                //Search item in ItemsControl
                if (itemScript.IsShowItem && itemScript.HelpObject.UIElement is ItemsControl itemsControl)
                {
                    HelpObject helpObjectItem = null;

                    double ic_w = (double)itemsControl.GetValue(FrameworkElement.ActualWidthProperty);
                    double ic_h = (double)itemsControl.GetValue(FrameworkElement.ActualHeightProperty);
                    Rect icRect = itemsControl.TransformToVisual(this).TransformBounds(new Rect(0, 0, ic_w, ic_h));

                    int attempItem = 0;
                    foreach (var item in itemsControl.Items)
                    {
                        if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is UIElement uiItem && uiItem.IsVisible)
                        {
                            double ui_w = (double)uiItem.GetValue(FrameworkElement.ActualWidthProperty);
                            double ui_h = (double)uiItem.GetValue(FrameworkElement.ActualHeightProperty);
                            Rect uiItemRect = uiItem.TransformToVisual(itemsControl).TransformBounds(new Rect(0, 0, ui_w, ui_h));

                            if (uiItemRect.X > 0 && uiItemRect.Y > 0)
                            {
                                helpObjectItem = new HelpObject(uiItem);
                                if (++attempItem >= 3) break;
                            }
                        }
                    }

                    if (helpObjectItem != null)
                    {
                        yield return new ScriptObject()
                        {
                            NameHelpObject = $"{itemScript.NameHelpObject}_Item",
                            HelpObject = helpObjectItem,
                            Description = string.IsNullOrEmpty(itemScript.DescriptionItem) ? itemScript.Description : itemScript.DescriptionItem,
                            DescriptionTemplate = itemScript.DescriptionItemTemplate == null ? itemScript.DescriptionTemplate : itemScript.DescriptionItemTemplate,
                        };

                        //Search controls in Item
                        if (itemScript.Items != null)
                        {
                            foreach (var item in itemScript.Items.Where(w => w.IsShow))
                            {
                                var child = GetChildByName(helpObjectItem.UIElement, item.NameHelpObject);
                                if (child is UIElement uIElement)
                                {
                                    item.HelpObject = new HelpObject(uIElement);
                                    yield return item;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private async void OnShow()
        {
            double durationAnimation = 300;

            Visibility = Visibility.Visible;
            BeginOpacityAnimation(this, 1, durationAnimation);

            IEnumerable<ScriptObject> script = helpObjects.Select(s => s.Value).Where(w => w.IsVisibleObject).Select(s => 
                    new ScriptObject() 
                    {
                        NameHelpObject = s.NameObject, 
                        HelpObject = s 
                    });

            if (Script != null && Script.Count > 0)
            {
                var names = script.Select(s => s.NameHelpObject).ToList();
                script = GetScripts(names);
            }
            
            foreach (var item in script)
            {
                if (!isShow)
                    return;

                //Help description object
                var contentDescription = new ContentControl();
                Canvas.SetZIndex(contentDescription, int.MaxValue);
                contentDescription.Opacity = 0;
                contentDescription.Content = item.Description;
                contentDescription.Template = item.DescriptionTemplate;
                

                item.HelpObject.Opacity = 0;
                Children.Add(item.HelpObject);              
                Children.Add(contentDescription);          
                
                //Show controls
                BeginOpacityAnimation(item.HelpObject, 1, durationAnimation);
                BeginOpacityAnimation(contentDescription, 1, durationAnimation);
          
                //Wait to next help object
                if (DurationViewHelpObject != TimeSpan.Zero)
                    await ClickNextHelpObject((int)DurationViewHelpObject.TotalMilliseconds, out clickNextHelpObjectAction);
                else
                    await ClickNextHelpObject(out clickNextHelpObjectAction);

                //Hidden controls
                BeginOpacityAnimation(item.HelpObject, 0, durationAnimation, (s, e) => Children.Remove(item.HelpObject));
                BeginOpacityAnimation(contentDescription, 0, durationAnimation, (s, e) => Children.Remove(contentDescription));
            }

            IsShow = false;
        }

        private Task ClickNextHelpObject(out Action actionSendResult)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            actionSendResult = () => tcs.SetResult(true);
            return tcs.Task;
        }

        private Task ClickNextHelpObject(int duration, out Action actionSendResult)
        {   
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            bool isInvoke = false;
            actionSendResult = () =>
            {
                isInvoke = true;
                tcs.SetResult(true);
            };
            _ = Task.Factory.StartNew(async () =>
            {                
                await Task.Delay(duration);
                if (!isInvoke)
                    clickNextHelpObjectAction?.Invoke();
            });

            return tcs.Task;
        }



        private void OnHidden()
        {
            BeginOpacityAnimation(this, 0, 300, (s, e) => 
            {
                Visibility = Visibility.Collapsed;
                Children.OfType<HelpObject>().ToList().ForEach(f => Children.Remove(f));
            });
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            OnMouseClick();
        }

        private void OnMouseClick()
        {
            if (DurationViewHelpObject != TimeSpan.Zero)
            {
                IsShow = false;
                clickNextHelpObjectAction?.Invoke();
            }
            else
            {
                clickNextHelpObjectAction?.Invoke();                
            }
            clickNextHelpObjectAction = null;
        }

        #region Helpers
        private static T GetUiParent<T>(DependencyObject dependencyObject) where T : Visual
        {
            var parentObject = VisualTreeHelper.GetParent(dependencyObject);

            if (parentObject is T uiElement)
                return uiElement;

            return GetUiParent<T>(parentObject);
        }

        private static DependencyObject GetChildByName(DependencyObject parent, string name)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if ((string)child.GetValue(FrameworkElement.NameProperty) == name)
                {
                    return child;
                }
                else if (GetChildByName(child, name) is DependencyObject result)
                    return result;
            }
            return null;
        }
        private static void BeginOpacityAnimation(UIElement uIElement, double toOpacity, double duration, Action<object, EventArgs> completed = null)
        {
            var animation = new System.Windows.Media.Animation.DoubleAnimation()
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(duration),
            };

            if (completed != null)
                animation.Completed += (s, e) => completed(s, e);

            uIElement.BeginAnimation(OpacityProperty, animation);
        }

        #endregion
    }

   
}
