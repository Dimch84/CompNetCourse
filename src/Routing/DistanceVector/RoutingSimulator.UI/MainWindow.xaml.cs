using RoutingSimulator.Core;
using RoutingSimulator.Core.Exceptions;
using RoutingSimulator.UI.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RoutingSimulator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Mode
        {
            Default,
            Delete,
            Send
        }

        private DispatcherTimer _timer = new DispatcherTimer();

        private const double SendLineDashLength = 5;
        private const double MovingOpacity = 0.5;
        private const double StationaryOpacity = 1.0;
        private const double CircleRadius = 15;
        private const double LineThickness = 1.0;
        private const double PathLineThickness = 3.0;
        private readonly Brush CircleFill = new SolidColorBrush(Colors.White);
        private readonly Brush CircleStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush CircleLabelColor = new SolidColorBrush(Colors.Black);
        private readonly Brush CommunicatingCircleFill = new SolidColorBrush(Colors.Red);
        private readonly Brush LineStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush PathLineStroke = new SolidColorBrush(Colors.Red);
        private List<Shapes.Line> _lines = new List<Shapes.Line>();
        private List<Shapes.Circle> _circles = new List<Shapes.Circle>();
        private Shapes.Line _tempLine = null;
        private Mode _currentMode = Mode.Default;
        private Dictionary<Mode, Cursor> _modeCursorMap = new Dictionary<Mode, Cursor>()
        {
            {Mode.Default, Cursors.Arrow },
            {Mode.Delete, Cursors.Arrow },
            {Mode.Send, Cursors.Cross} 
        };
        private Dictionary<Mode, string> _modeDescriptionMap = new Dictionary<Mode, string>()
        {
            {Mode.Default, "Left-click to add new routers, right-click to add connections between them." },
            {Mode.Delete, "Left-click on a router to remove it." },
            {Mode.Send, "Left-click and drag to connect two routers and send a packet between them." }
        };

        private UndirectedWeightedGraph<string> graph;
        private Dictionary<Circle, Node<string>> _circleNodeMap = new Dictionary<Circle, Node<string>>();
        private Dictionary<Shapes.Line, Tuple<Node<string>, Node<string>>> _lineNodeMap = new Dictionary<Shapes.Line, Tuple<Node<string>, Node<string>>>();

        public MainWindow()
        {
            InitializeMouseEventHandlers();
            InitializeComponent();
            EnableCanvasMouseEvents();
            modeDescriptionLabel.Content = _modeDescriptionMap[_currentMode];
            graph = new UndirectedWeightedGraph<string>();
            _timer.Tick += (s, e) =>
            {
                graph.Tick();
            };
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 10); // every 10ms
            _timer.Start();
            canvas.PreviewMouseDown += (s, e) =>
            {
                ResetAllStyles();
            };
        }

        #region Helper Methods
        private void ResetCircleStyle(Circle c)
        {
            c.Fill = new SolidColorBrush(Colors.White);
            c.Stroke = new SolidColorBrush(Colors.Black);
            c.LabelColor = new SolidColorBrush(Colors.Black);
            c.Opacity = StationaryOpacity;
        }

        private void ResetAllStyles()
        {
            foreach(var line in _lines)
            {
                line.Stroke = LineStroke;
                line.Thickness = LineThickness;
            }
            foreach(var circle in _circles)
            {
                ResetCircleStyle(circle);
            }
        }

        private Circle CreateCircle(bool isTransaprent = true)
        {
            var c = new Circle(CircleRadius, fill: CircleFill.Clone(),
                                                   stroke: CircleStroke.Clone(),
                                                   labelColor: CircleLabelColor.Clone(),
                                                   opacity: isTransaprent ? MovingOpacity : StationaryOpacity);
            c.MouseRightButtonUp += OnCircleMouseRightButtonUp;
            c.MouseRightButtonDown += OnCircleMouseRightButtonDown;
            c.MouseLeftButtonDown += OnCircleMouseLeftButtonDown;
            c.MouseLeftButtonUp += OnCircleMouseLeftButtonUp;
            c.MouseEnter += OnCircleMouseEnter;
            c.MouseLeave += OnCircleMouseLeave;
            c.MouseMove += OnCircleMouseMove;
            return c;
        }
        #endregion

        #region CircleMouseEventHandlers
        private EventHandler<MouseEventArgs> OnCircleMouseRightButtonUp;
        private EventHandler<MouseEventArgs> OnCircleMouseRightButtonDown;
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonDown;
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonUp;
        private EventHandler<MouseEventArgs> OnCircleMouseEnter;
        private EventHandler<MouseEventArgs> OnCircleMouseLeave;
        private EventHandler<MouseEventArgs> OnCircleMouseMove;
        private MouseEventHandler OnEmptyCircleMouseMove;
        private MouseButtonEventHandler OnEmptyCircleMouseRightButtonUp;
        private MouseButtonEventHandler OnEmptyCircleMouseLeftButtonUp;
        private void InitializeMouseEventHandlers()
        {
            OnCircleMouseLeftButtonDown = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    c.Opacity = MovingOpacity;
                    Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault() as UIElement);
                }
                else if(_currentMode == Mode.Delete)
                {
                    var c = s as Circle;
                    canvas.Children.Remove(c);
                    graph.RemoveNode(_circleNodeMap[c]);
                    _circles.Remove(c);
                    _circleNodeMap.Remove(c);
                    c.Dispose();
                }
                else if(_currentMode == Mode.Send)
                {
                    var start = s as Circle;
                    start.Fill = CommunicatingCircleFill.Clone();
                    _tempLine = new Shapes.Line(start,
                                            Circle.Empty,
                                            new SolidColorBrush(Colors.Red),
                                            new SolidColorBrush(Colors.Red),
                                            MovingOpacity, LineThickness, SendLineDashLength);
                    canvas.Children.Add(_tempLine);
                }
            };
            OnCircleMouseLeftButtonUp = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    Mouse.Capture(null);
                    c.Opacity = StationaryOpacity;
                    if(!_circleNodeMap.ContainsKey(c))
                    {
                        var node = new Node<string>(c.Text);
                        _circleNodeMap.Add(c, node);
                        _circles.Add(c);
                        graph.AddNode(node);
                    }
                }
                else if(_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if (_tempLine == null || _tempLine.Start == c)
                        return;
                    ResetCircleStyle(_tempLine.Start);
                    ResetCircleStyle(c);
                    // send a packet from _tempLine.Start to _tempLine.End
                    var startNode = _circleNodeMap[_tempLine.Start];
                    var endNode = _circleNodeMap[c];
                    try
                    {
                        var path = graph.FindShortestPath(startNode, endNode).ToArray();
                        var lines = new List<Shapes.Line>();
                        for(int i = 0; i < path.Length-1; i++)
                        {
                            var line = _lineNodeMap.Where(x => (x.Value.Item1 == path[i] && x.Value.Item2 == path[i + 1]) ||
                                                         (x.Value.Item1 == path[i + 1] && x.Value.Item2 == path[i])
                                                         )
                                                   .Select(x => x.Key)
                                                   .FirstOrDefault();
                            lines.Add(line);
                        }
                        ResetAllStyles();
                        var circles = _circleNodeMap.Where(x => path.Contains(x.Value)).Select(x => x.Key);
                        foreach(var line in lines)
                        {
                            line.Stroke = PathLineStroke;
                            line.Thickness = PathLineThickness;
                        }
                        foreach(var circle in circles)
                        {
                            circle.Fill = CommunicatingCircleFill;
                        }
                    }
                    catch(PathDoesNotExistException)
                    {
                        MessageBox.Show("The selected routers are not connected.", "No connection", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            };
            OnCircleMouseEnter = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    this.Cursor = Cursors.Hand;
                    DisableCanvasMouseEvents();

                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                    }
                }
                else if (_currentMode == Mode.Delete)
                {
                    this.Cursor = Cursors.No;
                }
                else if (_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if(_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = c.PositionCenter;
                        if (c != _tempLine.Start)
                            c.Fill = CommunicatingCircleFill.Clone();
                    }
                }
            };
            OnCircleMouseLeave = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    this.Cursor = Cursors.Arrow;
                    EnableCanvasMouseEvents();
                }
                else if (_currentMode == Mode.Delete)
                {
                    this.Cursor = Cursors.Arrow;
                }
                else if(_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if (_tempLine != null && _tempLine.Start != c)
                        ResetCircleStyle(c);
                }
            };
            OnCircleMouseMove = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    var pos = args.GetPosition(canvas);
                    if (pos.X < CircleRadius)
                        pos.X = CircleRadius;
                    if (pos.X >= canvas.ActualWidth - CircleRadius)
                        pos.X = canvas.ActualWidth - CircleRadius;
                    if (pos.Y < CircleRadius)
                        pos.Y = CircleRadius;
                    if (pos.Y >= canvas.ActualHeight - CircleRadius)
                        pos.Y = canvas.ActualHeight - CircleRadius;
                    if(Mouse.Captured == c.UIElements.Where(x => x is Ellipse).FirstOrDefault())
                    {
                        c.PositionCenter = pos;
                    }
                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                        args.Handled = true;
                    }
                }
                else if(_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                        args.Handled = true;
                    }
                }
            };

            OnEmptyCircleMouseMove += (sender, args) =>
            {
                var pos = args.GetPosition(canvas);
                if(_currentMode == Mode.Default)
                {
                    if (pos.X < 0)
                        pos.X = 0;
                    if (pos.X >= canvas.ActualWidth)
                        pos.X = canvas.ActualWidth - 1;
                    if (pos.Y < 0)
                        pos.Y = 0;
                    if (pos.Y >= canvas.ActualHeight)
                        pos.Y = canvas.ActualHeight - 1;
                    if (_tempLine == null)
                        return;
                    if (args.RightButton == MouseButtonState.Pressed)
                        _tempLine.End.PositionCenter = pos;
                    else
                        _tempLine = null;
                }
                else if(_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                        _tempLine.End.PositionCenter = pos;
                }
            };

            OnCircleMouseRightButtonDown = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var start = s as Circle;
                    _tempLine = new Shapes.Line(start,
                                                Circle.Empty,
                                                LineStroke,
                                                LineStroke,
                                                LineThickness,
                                                MovingOpacity);
                    _tempLine.Disposing += (sender, e) =>
                    {
                        canvas.Children.Remove(sender as Shapes.Line);
                    };
                    _tempLine.DragStart += (sender, e) =>
                    {
                        if(_currentMode == Mode.Default)
                            (sender as Shapes.Line).Opacity = MovingOpacity;
                    };
                    _tempLine.DragEnd += (sender, e) =>
                    {
                        if(_currentMode == Mode.Default)
                            (sender as Shapes.Line).Opacity = StationaryOpacity;
                    };
                    canvas.Children.Add(_tempLine);
                }
            };

            OnCircleMouseRightButtonUp = (s, args) =>
            {
                if (_currentMode == Mode.Default)
                {
                    if (_tempLine == null)
                        return;
                    var c = s as Circle;
                    if (_tempLine.Start == c)
                        return;
                    var window = new IntegerInputWindow(this);
                    window.InputEntered += (sender, e) =>
                    {
                        _tempLine.LabelText = e.ToString();
                        _tempLine.End = c;
                        _tempLine.Opacity = StationaryOpacity;
                        _lines.Add(_tempLine);

                        var startNode = _circleNodeMap[_tempLine.Start];
                        var endNode = _circleNodeMap[_tempLine.End];
                        _lineNodeMap.Add(_tempLine,
                            new Tuple<Node<string>, Node<string>>(startNode, endNode));
                        graph.AddEdge(startNode, endNode, e);

                        _tempLine = null;
                    };
                    window.ShowDialog();
                }
            };

            OnEmptyCircleMouseRightButtonUp = (s, args) =>
            {
                if (_currentMode == Mode.Default)
                {
                    if (_tempLine != null)
                        canvas.Children.Remove(_tempLine);
                    _tempLine = null;
                }
            };

            OnEmptyCircleMouseLeftButtonUp = (s, args) =>
            {
                if (_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                    {
                        canvas.Children.Remove(_tempLine);
                    }
                    _tempLine = null;
                }
            };
        }

        private void DisableCanvasMouseEvents()
        {
            canvas.MouseMove -= OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp -= OnEmptyCircleMouseRightButtonUp;
            canvas.MouseLeftButtonUp -= OnEmptyCircleMouseLeftButtonUp;
        }

        private void EnableCanvasMouseEvents()
        {
            canvas.MouseMove += OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp += OnEmptyCircleMouseRightButtonUp;
            canvas.MouseLeftButtonUp += OnEmptyCircleMouseLeftButtonUp;
        }


        #endregion

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_currentMode == Mode.Default)
            {
                var pos = e.GetPosition(canvas);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Mouse.Captured == null)
                    {
                        var c = CreateCircle(true);
                        canvas.Children.Add(c);
                        Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault());
                    }
                }
            }
        }

        private void mode_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            switch(rb.Name)
            {
                case "deleteMode":
                    _currentMode = Mode.Delete;
                    break;
                case "sendMode":
                    _currentMode = Mode.Send;
                    break;
                default:
                    _currentMode = Mode.Default;
                    break;
            }
            if(modeDescriptionLabel != null)
                modeDescriptionLabel.Content = _modeDescriptionMap[_currentMode];
        }
    }
}
