using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HelpView.Controls
{
    public class ScriptObject : DependencyObject
    {
        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow", typeof(bool), typeof(ScriptObject), new PropertyMetadata(true));

        public bool IsShowItem
        {
            get { return (bool)GetValue(IsShowItemProperty); }
            set { SetValue(IsShowItemProperty, value); }
        }

        public static readonly DependencyProperty IsShowItemProperty =
            DependencyProperty.Register("IsShowItem", typeof(bool), typeof(ScriptObject), new PropertyMetadata(false));

        public string NameHelpObject
        {
            get { return (string)GetValue(NameHelpObjectProperty); }
            set { SetValue(NameHelpObjectProperty, value); }
        }

        public static readonly DependencyProperty NameHelpObjectProperty =
            DependencyProperty.Register("NameHelpObject", typeof(string), typeof(ScriptObject), new PropertyMetadata(null));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ScriptObject), new PropertyMetadata(null));

        public string DescriptionItem
        {
            get { return (string)GetValue(DescriptionItemProperty); }
            set { SetValue(DescriptionItemProperty, value); }
        }

        public static readonly DependencyProperty DescriptionItemProperty =
            DependencyProperty.Register("DescriptionItem", typeof(string), typeof(ScriptObject), new PropertyMetadata(null));


        public List<ScriptObject> Items
        {
            get { return (List<ScriptObject>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(List<ScriptObject>), typeof(ScriptObject), new PropertyMetadata(new List<ScriptObject>()));


        public ControlTemplate DescriptionTemplate { get; set; }
        public ControlTemplate DescriptionItemTemplate { get; set; }
        public HelpObject HelpObject { get; set; }
    }
}
