using System.Windows;
using System.Windows.Media;
using static ArcToCubicBezierSample.ArcToCubicBezierHelper;

namespace ArcToCubicBezierSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var streamGeometry = new StreamGeometry();
                using (var context = streamGeometry.Open())
                {
                    context.BeginFigure(new Point(0, 16), false, false);
                    context.ArcTo(new Point(16, 0), new Size(16, 16), 0, false, SweepDirection.Counterclockwise, true, true);
                }
                var pen = new Pen();
                RefreshPen(pen);
                drawingContext.DrawGeometry(new SolidColorBrush(Colors.Black), pen, streamGeometry);
            }

            this.PathGrid.Background = new VisualBrush(drawingVisual) { Stretch = Stretch.None };
        }

        /// <summary>
        /// 依据用户的设置，更新传入笔的颜色，粗细以及线型(绘制PPT多路径用到)
        /// </summary>
        private void RefreshPen(Pen pen, bool isStroke = true)
        {
            pen.Brush = new SolidColorBrush(Colors.Black);
            pen.Thickness = 2;
            //pen.DashStyle = new System.Windows.Media.DashStyle(0, 1);
            pen.StartLineCap = PenLineCap.Flat;
            pen.EndLineCap = pen.StartLineCap;
            pen.LineJoin = PenLineJoin.Round;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = "A16.000,16.000,0,0,0,16.000,0.000";
            var (x1, y1, x2, y2, x, y) = ArcToBezier(16, 0, 0, 16, 16, 16, 0, false, false);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var streamGeometry = new StreamGeometry();
                using (var context = streamGeometry.Open())
                {
                    context.BeginFigure(new Point(0, 16), false, false);
                    context.BezierTo(new Point(x1, y1), new Point(x2, y2), new Point(x, y), true, true);
                }
                var pen = new Pen();
                RefreshPen(pen);
                drawingContext.DrawGeometry(new SolidColorBrush(Colors.Black), pen, streamGeometry);
            }

            this.PathGrid.Background = new VisualBrush(drawingVisual) { Stretch = Stretch.None };

        }
    }
}
