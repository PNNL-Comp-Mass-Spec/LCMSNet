using System;
using System.Windows;
using System.Windows.Media;

namespace LcmsNet.Method.Views
{
    public class DrawingVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects.
        private readonly VisualCollection _children;

        public DrawingVisualHost()
        {
            _children = new VisualCollection(this);
        }

        public void AddDrawingVisual(DrawingVisual visual)
        {
            _children.Add(visual);
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
