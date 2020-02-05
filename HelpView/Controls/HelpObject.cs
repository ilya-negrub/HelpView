using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace HelpView.Controls
{
    public class HelpObject : FrameworkElement
    {
        public string NameObject => (string)uiElement.GetValue(FrameworkElement.NameProperty);
        public bool IsVisibleObject => uiElement.IsVisible == true;
        public Rect Bounds => GetBounds();

        internal UIElement UIElement => uiElement;

        UIElement uiElement;
        public HelpObject(UIElement uiElement)
        {
            this.uiElement = uiElement;
        }

        private Rect GetBounds()
        {
            if (!uiElement.IsVisible)
                return new Rect();

            double w = (double)uiElement.GetValue(FrameworkElement.ActualWidthProperty);
            double h = (double)uiElement.GetValue(FrameworkElement.ActualHeightProperty);

            Rect rect = uiElement.TransformToVisual(this).TransformBounds(new Rect(0, 0, w, h));
            return rect;
        }

        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);
            Rect rect = GetBounds();
            dc.DrawRectangle(new VisualBrush(uiElement), new Pen(), rect);
        }

    }
}
