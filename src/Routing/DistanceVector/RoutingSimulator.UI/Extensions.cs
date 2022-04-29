using RoutingSimulator.UI.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace RoutingSimulator.UI
{
    static class Extensions
    {
        public static void Add(this UIElementCollection collection, Circle c)
        {
            foreach (var el in c.UIElements)
                collection.Add(el);
        }
        public static void Remove(this UIElementCollection collection, Circle c)
        {
            foreach (var el in c.UIElements)
                collection.Remove(el);
        }

        public static void Add(this UIElementCollection collection, Line l)
        {
            foreach (var el in l.UIElements)
                collection.Add(el);
        }

        public static void Remove(this UIElementCollection collection, Line l)
        {
            foreach (var el in l.UIElements)
                collection.Remove(el);
        }

        public static bool Contains(this System.Windows.Shapes.Ellipse e, Point p)
        {
            Point center = new Point(
                  Canvas.GetLeft(e) + (e.Width / 2),
                  Canvas.GetTop(e) + (e.Height / 2));

            double _xRadius = e.Width / 2;
            double _yRadius = e.Height / 2;


            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;

            Point normalized = new Point(p.X - center.X,
                                         p.Y - center.Y);

            return ((double)(normalized.X * normalized.X)
                     / (_xRadius * _xRadius)) + ((double)(normalized.Y * normalized.Y) / (_yRadius * _yRadius))
                <= 1.0;
        }
    }
}
