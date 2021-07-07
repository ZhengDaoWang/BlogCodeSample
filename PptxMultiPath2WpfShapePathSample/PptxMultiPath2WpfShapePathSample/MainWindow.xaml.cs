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
using Path = System.Windows.Shapes.Path;
using ShapeProperties = DocumentFormat.OpenXml.Presentation.ShapeProperties;

namespace PptxMultiPath2WpfShapePathSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = @"C:\Users\Ryzen\Desktop\测验\五边形.pptx";
            if (!string.IsNullOrEmpty(FilePathText.Text))
            {
                filePath = FilePathText.Text.Trim();
            }
            PptxMultiPathToGeometry(filePath);
        }
        private void PptxMultiPathToGeometry(string filePath)
        {
            if (!File.Exists(filePath) || !filePath.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
            {
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
                                        var points = GetPentagonPoints(width.Value.EmuToPixel(), height.Value.EmuToPixel());
                                        RenderGeometry(points);
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

        private List<Path> CreatePathLst(List<GeometryPath> geometryPaths)
        {
            var pathLst = new List<Path>();
            foreach (var geometryPath in geometryPaths)
            {
                var geometry = Geometry.Parse(geometryPath.Path);
                var path = new Path
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
