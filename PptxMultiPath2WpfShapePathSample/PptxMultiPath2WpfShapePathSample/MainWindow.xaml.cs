using System;
using System.Collections.Generic;
using System.IO;
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
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using ShapeProperties = DocumentFormat.OpenXml.Presentation.ShapeProperties;
using static PptxMultiPath2WpfShapePathSample.ShapeGeometryHelper;
using System.Reflection;

namespace PptxMultiPath2WpfShapePathSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath;
        public MainWindow()
        {
            InitializeComponent();
            var mainExecuteDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilePathText.Text = filePath = System.IO.Path.Combine(mainExecuteDirectory, @"Test.pptx");
        }
        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePathText.Text))
            {
                PptxMultiPathToGeometry(filePath);
            }
            else
            {
                PptxMultiPathToGeometry(FilePathText.Text);
            }
        }
        private void PptxMultiPathToGeometry(string filePath)
        {
            if (!File.Exists(filePath) || !filePath.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("请输入正确的pptx文件路径");
                return;
            }
            using (var presentationDocument = PresentationDocument.Open(filePath, false))
            {
                var presentationPart = presentationDocument.PresentationPart;
                var presentation = presentationPart?.Presentation;
                var slideIdList = presentation?.SlideIdList;
                if (slideIdList == null)
                {
                    return;
                }
                foreach (var slideId in slideIdList.ChildElements.OfType<SlideId>())
                {
                    var slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId);
                    var slide = slidePart.Slide;
                    foreach (var shapeProperties in slide.Descendants<ShapeProperties>())
                    {
                        var presetGeometry = shapeProperties.GetFirstChild<PresetGeometry>();
                        if (presetGeometry != null && presetGeometry.Preset.HasValue)
                        {
                            if (presetGeometry.Preset == ShapeTypeValues.BorderCallout2)
                            {
                                var transform2D = shapeProperties.GetFirstChild<Transform2D>();
                                var extents = transform2D?.GetFirstChild<Extents>();
                                if (extents != null)
                                {
                                    var width = extents.Cx;
                                    var height = extents.Cy;
                                    if (width.HasValue && height.HasValue)
                                    {
                                        var geometryPaths = GetGeometryPathFromBorderCallout2(new Emu(width).EmuToPixel().Value, new Emu(height).EmuToPixel().Value);
                                        RenderGeometry(geometryPaths);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RenderGeometry(List<GeometryPath> geometryPaths)
        {
            if (PathGrid.Children.Count > 0)
            {
                PathGrid.Children.Clear();
            }
            var pathGeometry = CreatePathLst(geometryPaths);
            foreach (var path in pathGeometry)
            {
                PathGrid.Children.Add(path);
            }
        }

        private List<System.Windows.Shapes.Path> CreatePathLst(List<GeometryPath> geometryPaths)
        {
            var pathLst = new List<System.Windows.Shapes.Path>();
            foreach (var geometryPath in geometryPaths)
            {
                var geometry = Geometry.Parse(geometryPath.Path);
                var path = new System.Windows.Shapes.Path
                {
                    Data = geometry,
                    Fill = geometryPath.IsFilled ? new SolidColorBrush(Color.FromRgb(68, 114, 196)) : null,
                    Stroke = geometryPath.IsStroke ? new SolidColorBrush(Color.FromRgb(47, 82, 143)) : null,
                };
                pathLst.Add(path);
            }
            return pathLst;
        }
    }
}
