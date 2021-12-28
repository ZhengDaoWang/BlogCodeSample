using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using dotnetCampus.OpenXmlUnitConverter;
using ShapeProperties = DocumentFormat.OpenXml.Presentation.ShapeProperties;

namespace PptDashConverterToWpfSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Brush Fill { get; } = new SolidColorBrush(Color.FromRgb(68, 114, 196));

        public double StrokeThickness => 9.333;
        public Brush Stroke => new SolidColorBrush(Color.FromRgb(68,114,196));

        public MainWindow()
        {
            InitializeComponent();

            PptxToGeometry("形状边框的默认虚线设置.pptx");
        }

        private void PptxToGeometry(string filePath)
        {
            if (!File.Exists(filePath) || !filePath.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var lines = new List<Line>();
            using var presentationDocument = PresentationDocument.Open(filePath, false);
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
                        if (presetGeometry.Preset == ShapeTypeValues.StraightConnector1)
                        {
                            var transform2D = shapeProperties.GetFirstChild<Transform2D>();
                            var extents = transform2D?.GetFirstChild<Extents>();
                            if (extents != null)
                            {
                                var width = new Emu(extents.Cx!.Value).ToPixel().Value;
                                var height = new Emu(extents.Cy!.Value).ToPixel().Value;


                                var presetDash = shapeProperties.GetFirstChild<Outline>()?.GetFirstChild<PresetDash>()?.Val;
                                var dashArray = GetDashArrayByPresetLineDashValues(presetDash);
                                var line = ConverterToGeometry( width, height, dashArray); 
                                lines.Add(line);
                            }
                        }
                    }
                }
            }

            this.ListBox.ItemsSource = lines;
        }

        private DoubleCollection GetDashArrayByPresetLineDashValues(PresetLineDashValues presetLineDashValues)
        {
            DoubleCollection dashStyle = presetLineDashValues switch
            {
                PresetLineDashValues.Solid => new(),
                PresetLineDashValues.Dot => new() { 0, 2 },
                PresetLineDashValues.Dash => new() { 3, 3 },
                PresetLineDashValues.LargeDash => new() { 8, 3 },
                PresetLineDashValues.DashDot => new() { 3, 3, 1, 3 },
                PresetLineDashValues.LargeDashDot => new() { 7.5, 3.5, 1, 3.5 },
                PresetLineDashValues.LargeDashDotDot => new() { 8, 3, 1, 3, 1, 3 },
                PresetLineDashValues.SystemDash => new() { 3, 1 },
                PresetLineDashValues.SystemDot => new() { 1, 1 },
                PresetLineDashValues.SystemDashDot => new() { 2, 2, 0, 2 },
                PresetLineDashValues.SystemDashDotDot => new() { 2, 2, 0, 2 },
                _ => new DoubleCollection()
            };
            return dashStyle;
        }

        private Line ConverterToGeometry(double width, double height, DoubleCollection dashDoubleCollection)
        {
            var line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = width,
                Y2 = height,
                StrokeDashArray = dashDoubleCollection,
                Stroke = Stroke,
                StrokeThickness = StrokeThickness
            };
            return line;
        }
    }
}
