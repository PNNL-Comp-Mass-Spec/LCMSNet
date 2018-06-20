using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FluidicsSDK.Graphic
{
    public static class Extensions
    {
        /// <summary>
        /// Tries to freeze a freezable
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool TryFreeze(this Freezable t)
        {
            if (t.CanFreeze)
            {
                t.Freeze();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw a polygon on the canvas, using the specified points
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="points"></param>
        public static void DrawPolygon(this DrawingContext drawing, Brush brush, Pen pen, IList<Point> points)
        {
            drawing.DrawPolygonOrLine(brush, pen, points, true);
        }

        /// Draw a polyline on the canvas, using the specified points
        public static void DrawPolyline(this DrawingContext drawing, Brush brush, Pen pen, IList<Point> points)
        {
            drawing.DrawPolygonOrLine(brush, pen, points, false);
        }

        private static void DrawPolygonOrLine(this DrawingContext drawing, Brush brush, Pen pen, IList<Point> points, bool isPolygon)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                // Add the initial point
                context.BeginFigure(points[0], true, isPolygon);

                // Add all other points
                context.PolyLineTo(points.Skip(1).ToArray(), true, false);
            }

            geometry.TryFreeze();

            drawing.DrawGeometry(brush, pen, geometry);
        }
    }
}
