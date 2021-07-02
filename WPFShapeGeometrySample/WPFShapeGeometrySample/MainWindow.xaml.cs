using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFShapeGeometrySample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CombinedMode.ItemsSource = new List<GeometryCombineMode>()
            {
                GeometryCombineMode.Xor,
                GeometryCombineMode.Intersect,
                GeometryCombineMode.Union,
                GeometryCombineMode.Exclude
            };
        }

        private void CreateCombinedGeometry(GeometryCombineMode combineMode)
        {
            var ellipseGeometry = new EllipseGeometry(new Point(0, 0), 50, 50);
            var rectangleGeometry = new RectangleGeometry(new Rect(new Point(0, 0), new Size(50, 50)), 0, 0);
            var combinedGeometry = new CombinedGeometry(ellipseGeometry, rectangleGeometry)
            {
                GeometryCombineMode = combineMode
            };
            CombinedPath.Data = combinedGeometry;

        }


        private string _currentCombineMode;

        public string CurrentCombineMode
        {
            get => _currentCombineMode;
            set
            {
                _currentCombineMode = value;
                if (Enum.TryParse(_currentCombineMode, out GeometryCombineMode mode))
                {
                    CreateCombinedGeometry(mode);
                }
            }
        }
    }
}
