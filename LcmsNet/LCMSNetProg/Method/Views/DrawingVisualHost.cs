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

        /// <summary>
        /// Add a drawing visual to the collection
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="clearFirst">Clear the existing drawing visuals first (recommended)</param>
        public void AddDrawingVisual(DrawingVisual visual, bool clearFirst)
        {
            if (clearFirst)
            {
                _children.Clear();
            }

            _children.Add(visual);
        }

        public void ClearContent()
        {
            _children.Clear();
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount => _children.Count;

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
