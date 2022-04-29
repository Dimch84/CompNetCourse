using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RoutingSimulator.UI.Shapes
{
    public class Line
    {
        private class LineLabel
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

            public LineLabel(Brush foreground)
            {
                _label = new Label();
                Foreground = foreground;
            }

            public void MoveTo(Line l)
            {
                var mid = GetMiddlePoint(l);
                Canvas.SetTop(_label, mid.Y);
                Canvas.SetLeft(_label, mid.X - Width/2);
                var angle = GetAngle(l) * 180 / Math.PI;
                var transform = new TransformGroup();
                if (Math.Abs(angle) > 90)
                    transform.Children.Add(new ScaleTransform() { ScaleY = -1, ScaleX = -1 });
                
                transform.Children.Add(new RotateTransform(angle));
                _label.RenderTransformOrigin = new Point(0.5, 0);
                _label.RenderTransform = transform;
            }

            private Point GetMiddlePoint(Line l)
            {
                return new Point((l.Start.PositionCenter.X + l.End.PositionCenter.X) / 2,
                                 (l.Start.PositionCenter.Y + l.End.PositionCenter.Y) / 2);
            }
            private double GetAngle(Line l)
            {
                return Math.Atan2(l.End.PositionCenter.Y - l.Start.PositionCenter.Y,
                                  l.End.PositionCenter.X - l.Start.PositionCenter.X);
            }
        }
        private System.Windows.Shapes.Line _line;
        private LineLabel _label;
        private Circle _start;
        private Circle _end;

        public Circle Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
                _start.PositionChanged += (s, e) =>
                {
                    _label.MoveTo(this);
                    UpdatePosition();
                };

                _start.MouseLeftButtonDown += (s, e) =>
                {
                    DragStart?.Invoke(this, EventArgs.Empty);
                };
                _start.MouseLeftButtonUp += (s, e) =>
                {
                    DragEnd?.Invoke(this, EventArgs.Empty);
                };
                _start.Disposing += (s, e) =>
                {
                    Disposing?.Invoke(this, e);
                };
            }
        }
        public Circle End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
                _end.PositionChanged += (s, e) =>
                {
                    _label.MoveTo(this);
                    UpdatePosition();
                };
                _end.MouseLeftButtonDown += (s, e) =>
                {
                    DragStart?.Invoke(this, EventArgs.Empty);
                };
                _end.MouseLeftButtonUp += (s, e) =>
                {
                    DragEnd?.Invoke(this, EventArgs.Empty);
                };
                _end.Disposing += (s, e) =>
                {
                    Disposing?.Invoke(this, e);
                };
            }
        }
        public Brush Stroke
        {
            get
            {
                return _line.Stroke;
            }
            set
            {
                _line.Stroke = value;
            }
        }
        public DoubleCollection StrokeDash
        {
            get
            {
                return _line.StrokeDashArray;
            }
            set
            {
                _line.StrokeDashArray = value;
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
        public double Thickness
        {
            get
            {
                return _line.StrokeThickness;
            }
            set
            {
                _line.StrokeThickness = value;
            }
        }
        public double Opacity
        {
            get
            {
                return _line.Opacity;
            }
            set
            {
                _line.Opacity = _label.Opacity = value;
            }
        }
        public string LabelText
        {
            get
            {
                return _label.Text;
            }
            set
            {
                _label.Text = value;
            }
        }
        public IEnumerable<UIElement> UIElements
        {
            get
            {
                yield return _line;
                yield return _label.UILabel;
            }
        }

        public event EventHandler Disposing;
        public event EventHandler DragStart;
        public event EventHandler DragEnd;

        public Line(Circle start, Circle end, Brush stroke, Brush labelColor, double opacity, double thickness = 1, double strokeDashLength = 0)
        {
            _line = new System.Windows.Shapes.Line();
            Canvas.SetZIndex(_line, int.MinValue);
            _label = new LineLabel(labelColor);
            Start = start;
            End = end;
            Stroke = stroke;
            LabelColor = labelColor;
            Opacity = opacity;
            Thickness = thickness;
            if(strokeDashLength > 0)
                StrokeDash = new DoubleCollection() { strokeDashLength };
        }

        private void UpdatePosition()
        {
            _line.X1 = Start.PositionCenter.X;
            _line.Y1 = Start.PositionCenter.Y;
            _line.X2 = End.PositionCenter.X;
            _line.Y2 = End.PositionCenter.Y;
        }
    }
}
