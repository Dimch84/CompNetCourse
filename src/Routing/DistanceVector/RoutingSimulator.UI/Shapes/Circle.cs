using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoutingSimulator.UI.Shapes
{
    public class Circle : IDisposable
    {
        private class CircleLabel
        {
            private Label _label;
            public Label UILabel
            {
                get
                {
                    return _label;
                }
            }
            public string Text
            {
                get
                {
                    return _label.Content.ToString();
                }
                set
                {
                    _label.Content = value;
                }
            }
            public Brush Foreground
            {
                get
                {
                    return _label.Foreground;
                }
                set
                {
                    _label.Foreground = value;
                }
            }
            public Brush Background
            {
                get
                {
                    return _label.Background;
                }
                set
                {
                    _label.Background = value;
                }
            }
            public double Height
            {
                get
                {
                    _label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return _label.DesiredSize.Height;
                }
            }
            public double Width
            {
                get
                {
                    _label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return _label.DesiredSize.Width;
                }
            }
            public double Opacity
            {
                get
                {
                    return _label.Opacity;
                }
                set
                {
                    _label.Opacity = value;
                }
            }

            public static CircleLabel Empty
            {
                get
                {
                    var l = new CircleLabel();
                    _id--;
                    l.Text = "";
                    return l;
                }
            }

            public CircleLabel() : this(new SolidColorBrush(Colors.Black), null)
            { }

            public CircleLabel(Brush foreground, Brush background)
            {
                _label = new Label();
                Foreground = foreground;
                Background = background;
                Text = Id;
            }

            private static int _id = 0;
            private const string Letters = "abcdefghijklmnopqrstuvwxyz";
            private static string Id
            {
                get
                {
                    string result = "";
                    int b = Letters.Length;
                    int tmpId = _id++;
                    while(tmpId >= 0)
                    {
                        result = Letters[tmpId % b] + result;
                        tmpId /= b;
                        tmpId--;
                    }
                    return result;
                }
            }
        }
        private Ellipse _circle;
        private CircleLabel _label;
        private Point _position;

        public string Text
        {
            get
            {
                return _label.Text;
            }
        }
        public Brush Fill
        {
            get
            {
                return _circle.Fill;
            }
            set
            {
                _circle.Fill = value;
            }
        }
        public Brush Stroke
        {
            get
            {
                return _circle.Stroke;
            }
            set
            {
                _circle.Stroke = value;
            }
        }
        public Brush LabelColor
        {
            get
            {
                return _label.Foreground;
            }
            set
            {
                _label.Foreground = value;
            }
        }
        public double Opacity
        {
            get
            {
                return _circle.Opacity;
            }
            set
            {
                _circle.Opacity = _label.Opacity = value;
            }
        }
        public double Radius
        {
            get
            {
                return _circle.Width / 2;
            }
            set
            {
                _circle.Width = _circle.Height = 2 * value;
            }
        }
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                PositionChanged?.Invoke(this, _position);
                Canvas.SetTop(_circle, _position.Y);
                Canvas.SetLeft(_circle, _position.X);
                Canvas.SetTop(_label.UILabel, _position.Y + _circle.Height / 2 - _label.Height / 2);
                Canvas.SetLeft(_label.UILabel, _position.X + _circle.Width / 2 - _label.Width / 2);
            }
        }
        public Point PositionCenter
        {
            get
            {
                return new Point(_position.X + Radius, _position.Y + Radius);
            }
            set
            {
                Position = new Point(value.X - Radius, value.Y - Radius);
            }
        }
        public IEnumerable<UIElement> UIElements
        {
            get
            {
                yield return _circle;
                yield return _label.UILabel;
            }
        }

        public static Circle Empty
        {
            get
            {
                return new Circle();
            }
        }

        public event EventHandler Disposing;
        public event EventHandler<Point> PositionChanged;
        public event EventHandler<MouseEventArgs> MouseEnter;
        public event EventHandler<MouseEventArgs> MouseLeave;
        public event EventHandler<MouseEventArgs> MouseLeftButtonDown;
        public event EventHandler<MouseEventArgs> MouseLeftButtonUp;
        public event EventHandler<MouseEventArgs> MouseRightButtonDown;
        public event EventHandler<MouseEventArgs> MouseRightButtonUp;
        public event EventHandler<MouseEventArgs> MouseMove;

        private Circle()
        {
            _circle = new Ellipse();
            _label = CircleLabel.Empty;
            Radius = 0;
            Position = new Point(0, 0);
            InitMouseEvents();
        }

        public Circle(double radius) : this(radius, new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black))
        { }

        public Circle(double radius, Brush fill, Brush stroke, Brush labelColor, double opacity = 1)
        {
            _circle = new Ellipse();
            _label = new CircleLabel(labelColor, null);
            Radius = radius;
            Fill = fill;
            Stroke = stroke;
            Position = new Point(0, 0);
            Opacity = opacity;
            InitMouseEvents();
        }

        private void InitMouseEvents()
        {
            _circle.MouseEnter += (s, e) => MouseEnter?.Invoke(this, e);
            _circle.MouseLeave += (s, e) => MouseLeave?.Invoke(this, e);
            _circle.MouseLeftButtonDown += (s, e) => MouseLeftButtonDown?.Invoke(this, e);
            _circle.MouseLeftButtonUp += (s, e) => MouseLeftButtonUp?.Invoke(this, e);
            _circle.MouseRightButtonDown += (s, e) => MouseRightButtonDown?.Invoke(this, e);
            _circle.MouseRightButtonUp += (s, e) => MouseRightButtonUp?.Invoke(this, e);
            _circle.MouseMove += (s, e) => MouseMove?.Invoke(this, e);

            _label.UILabel.MouseEnter += (s, e) => MouseEnter?.Invoke(this, e);
            _label.UILabel.MouseLeave += (s, e) => MouseLeave?.Invoke(this, e);
            _label.UILabel.MouseLeftButtonDown += (s, e) => MouseLeftButtonDown?.Invoke(this, e);
            _label.UILabel.MouseLeftButtonUp += (s, e) => MouseLeftButtonUp?.Invoke(this, e);
            _label.UILabel.MouseRightButtonDown += (s, e) => MouseRightButtonDown?.Invoke(this, e);
            _label.UILabel.MouseRightButtonUp += (s, e) => MouseRightButtonUp?.Invoke(this, e);
            _label.UILabel.MouseMove += (s, e) => MouseMove?.Invoke(this, e);
        }

        public void Dispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
