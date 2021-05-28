using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace RectangleSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine(Rectangle.RenderedGeometry.ToString());
            Path.Data = new RectangleGeometry(new Rect(100, 128, 150, 150));
            Debug.WriteLine(Path.RenderedGeometry.ToString());
        }
    }
}
