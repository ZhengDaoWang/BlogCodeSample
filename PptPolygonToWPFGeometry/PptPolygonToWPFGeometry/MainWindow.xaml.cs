using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using static PptPolygonToWPFGeometry.ShapeGeometryHelper;
using Point = System.Windows.Point;
using ShapeProperties = DocumentFormat.OpenXml.Presentation.ShapeProperties;

namespace PptPolygonToWPFGeometry
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

        private void PptxToGeometry(string filePath)
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
                            if (presetGeometry.Preset == ShapeTypeValues.Pentagon)
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

        /// <summary>
        /// 获取五边形顶点坐标
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// 该五边形定义出自ECMA-376-Fifth-Edition-Part-1-Fundamentals-And-Markup-Language-Reference
        /// \OfficeOpenXML-DrawingMLGeometries文档的presetShapeDefinitions.xml
        private List<Point> GetPentagonPoints(double width, double height)
        {
            var properties = new FormulaProperties(width, height);

            //<avLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <gd name="hf" fmla="val 105146" />
            //  <gd name="vf" fmla="val 110557" />
            //</avLst>
            var hf = 105146d;
            var vf = 110557d;

            //<gdLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <gd name="swd2" fmla="*/ wd2 hf 100000" />
            //  <gd name="shd2" fmla="*/ hd2 vf 100000" />
            //  <gd name="svc" fmla="*/ vc  vf 100000" />
            //  <gd name="dx1" fmla="cos swd2 1080000" />
            //  <gd name="dx2" fmla="cos swd2 18360000" />
            //  <gd name="dy1" fmla="sin shd2 1080000" />
            //  <gd name="dy2" fmla="sin shd2 18360000" />
            //  <gd name="x1" fmla="+- hc 0 dx1" />
            //  <gd name="x2" fmla="+- hc 0 dx2" />
            //  <gd name="x3" fmla="+- hc dx2 0" />
            //  <gd name="x4" fmla="+- hc dx1 0" />
            //  <gd name="y1" fmla="+- svc 0 dy1" />
            //  <gd name="y2" fmla="+- svc 0 dy2" />
            //  <gd name="it" fmla="*/ y1 dx2 dx1" />
            //</gdLst>

            //  <gd name="swd2" fmla="*/ wd2 hf 100000" />
            var swd2 = properties.wd2 * hf / 100000;
            //  <gd name="shd2" fmla="*/ hd2 vf 100000" />
            var shd2 = properties.hd2 * vf / 100000;
            //  <gd name="svc" fmla="*/ vc  vf 100000" />
            var svc = properties.vc * vf / 100000;
            //  <gd name="dx1" fmla="cos swd2 1080000" />
            var dx1 = Cos(swd2, 1080000);
            //  <gd name="dx2" fmla="cos swd2 18360000" />
            var dx2 = Cos(swd2, 18360000);
            //  <gd name="dy1" fmla="sin shd2 1080000" />
            var dy1 = Sin(shd2, 1080000);
            //  <gd name="dy2" fmla="sin shd2 18360000" />
            var dy2 = Sin(shd2, 18360000);
            //  <gd name="x1" fmla="+- hc 0 dx1" />
            var x1 = properties.hc - dx1;
            //  <gd name="x2" fmla="+- hc 0 dx2" />
            var x2 = properties.hc - dx2;
            //  <gd name="x3" fmla="+- hc dx2 0" />
            var x3 = properties.hc + dx2;
            //  <gd name="x4" fmla="+- hc dx1 0" />
            var x4 = properties.hc + dx1;
            //  <gd name="y1" fmla="+- svc 0 dy1" />
            var y1 = svc - dy1;
            //  <gd name="y2" fmla="+- svc 0 dy2" />
            var y2 = svc - dy2;
            //  <gd name="it" fmla="*/ y1 dx2 dx1" />

            // <pathLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <path>
            //    <moveTo>
            //      <pt x="x1" y="y1" />
            //    </moveTo>
            //    <lnTo>
            //      <pt x="hc" y="t" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="x4" y="y1" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="x3" y="y2" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="x2" y="y2" />
            //    </lnTo>
            //    <close />
            //  </path>
            //</pathLst>
            var points = new List<Point>(5)
            {
                new Point(x1, y1),
                new Point(properties.hc,properties.t),
                new Point(x4, y1),
                new Point(x3, y2),
                new Point(x2, y2),
            };
            return points;
        }

        private void RenderGeometry(List<Point> points)
        {
            if (points.Count > 0)
            {
                var streamGeometry = new StreamGeometry();
                using var context = streamGeometry.Open();
                context.BeginFigure(points[0], true, true);
                context.PolyLineTo(points, true, true);
                this.Path.Data = streamGeometry;
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = @"C:\Users\Ryzen\Desktop\测验\五边形.pptx";
            if (!string.IsNullOrEmpty(FilePathText.Text))
            {
                filePath = FilePathText.Text.Trim();
            }
            PptxToGeometry(filePath);

        }
    }
}
