using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using dotnetCampus.OpenXmlUnitConverter;
using Microsoft.Office.Core;
using Application = Microsoft.Office.Interop.PowerPoint.Application;
using Presentation = Microsoft.Office.Interop.PowerPoint.Presentation;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;
using OfficeSlide = Microsoft.Office.Interop.PowerPoint.Slide;

namespace OpenXmlInsertShape
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShapeTypeComboBox.ItemsSource = Enum.GetNames<A.ShapeTypeValues>();
        }


        private string _filePath;
        private bool _isOpen;
        private void OpenPptxButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = PptxFilePathTextBox.Text;
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("请输入所要校验的Pptx文件路径");
                return;
            }

            filePath = filePath.Replace("\"", "");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("请输入正确的Pptx文件路径");
                return;
            }
            _filePath=filePath;

            if (_isOpen)
            {
                Process.Start(new ProcessStartInfo("explorer")
                {
                    ArgumentList =
                    {
                        _filePath
                    }
                });
            }
            else
            {
                OpenPptxButton.Content = "保存并且打开";
                _isOpen=true;
            }


        }

        private Shape CreateShape(A.ShapeTypeValues shapeType)
        {
            Shape shape1 = new Shape();

            NonVisualShapeProperties nonVisualShapeProperties1 = new NonVisualShapeProperties();

            NonVisualDrawingProperties nonVisualDrawingProperties1 = new NonVisualDrawingProperties(){ Id = (UInt32Value)4U, Name = "等腰三角形 3" };

            A.NonVisualDrawingPropertiesExtensionList nonVisualDrawingPropertiesExtensionList1 = new A.NonVisualDrawingPropertiesExtensionList();

            A.NonVisualDrawingPropertiesExtension nonVisualDrawingPropertiesExtension1 = new A.NonVisualDrawingPropertiesExtension(){ Uri = "{FF2B5EF4-FFF2-40B4-BE49-F238E27FC236}" };

            OpenXmlUnknownElement openXmlUnknownElement1 = OpenXmlUnknownElement.CreateOpenXmlUnknownElement("<a16:creationId xmlns:a16=\"http://schemas.microsoft.com/office/drawing/2014/main\" id=\"{B655AE28-C5C9-E8F0-5889-4D2ED5EEF436}\" />");

            nonVisualDrawingPropertiesExtension1.Append(openXmlUnknownElement1);

            nonVisualDrawingPropertiesExtensionList1.Append(nonVisualDrawingPropertiesExtension1);

            nonVisualDrawingProperties1.Append(nonVisualDrawingPropertiesExtensionList1);
            NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties1 = new NonVisualShapeDrawingProperties();
            ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties1 = new ApplicationNonVisualDrawingProperties();

            nonVisualShapeProperties1.Append(nonVisualDrawingProperties1);
            nonVisualShapeProperties1.Append(nonVisualShapeDrawingProperties1);
            nonVisualShapeProperties1.Append(applicationNonVisualDrawingProperties1);

            ShapeProperties shapeProperties1 = new ShapeProperties();

            A.Transform2D transform2D1 = new A.Transform2D(){ };

            double.TryParse(XTextBox.Text, out var x);
            double.TryParse(YTextBox.Text, out var y);
            double.TryParse(WidthTextBox.Text, out var width);
            double.TryParse(HeightTextBox.Text, out var height);

            A.Offset offset1 = new A.Offset()
            {
                X = new Pixel(x).ToEmu().ToOpenXmlInt64Value() , 
                Y = new Pixel(y).ToEmu().ToOpenXmlInt64Value()
            };
            A.Extents extents1 = new A.Extents()
            {
                Cx =new Pixel(width).ToEmu().ToOpenXmlInt64Value() , 
                Cy = new Pixel(height).ToEmu().ToOpenXmlInt64Value() , 
            };

            transform2D1.Append(offset1);
            transform2D1.Append(extents1);

            A.PresetGeometry presetGeometry1 = new A.PresetGeometry(){ Preset = shapeType };
            A.AdjustValueList adjustValueList1 = new A.AdjustValueList();

            presetGeometry1.Append(adjustValueList1);

            shapeProperties1.Append(transform2D1);
            shapeProperties1.Append(presetGeometry1);

            ShapeStyle shapeStyle1 = new ShapeStyle();

            A.LineReference lineReference1 = new A.LineReference(){ Index = (UInt32Value)2U };

            A.SchemeColor schemeColor1 = new A.SchemeColor(){ Val = A.SchemeColorValues.Accent1 };
            A.Shade shade1 = new A.Shade(){ Val = 15000 };

            schemeColor1.Append(shade1);

            lineReference1.Append(schemeColor1);

            A.FillReference fillReference1 = new A.FillReference(){ Index = (UInt32Value)1U };
            A.SchemeColor schemeColor2 = new A.SchemeColor(){ Val = A.SchemeColorValues.Accent1 };

            fillReference1.Append(schemeColor2);

            A.EffectReference effectReference1 = new A.EffectReference(){ Index = (UInt32Value)0U };
            A.SchemeColor schemeColor3 = new A.SchemeColor(){ Val = A.SchemeColorValues.Accent1 };

            effectReference1.Append(schemeColor3);

            A.FontReference fontReference1 = new A.FontReference(){ Index = A.FontCollectionIndexValues.Minor };
            A.SchemeColor schemeColor4 = new A.SchemeColor(){ Val = A.SchemeColorValues.Light1 };

            fontReference1.Append(schemeColor4);

            shapeStyle1.Append(lineReference1);
            shapeStyle1.Append(fillReference1);
            shapeStyle1.Append(effectReference1);
            shapeStyle1.Append(fontReference1);

        

            shape1.Append(nonVisualShapeProperties1);
            shape1.Append(shapeProperties1);
            shape1.Append(shapeStyle1);
            return shape1;
        }

        private void InsertButton_OnClick(object sender, RoutedEventArgs e)
        {
            var presentationDocument = PresentationDocument.Open(_filePath, true);
            var presentation = presentationDocument.PresentationPart?.Presentation;
            var slideIdList = presentation?.SlideIdList;
            if (slideIdList == null)
            {
                return;
            }

            var slideId = slideIdList.ChildElements.OfType<SlideId>().FirstOrDefault();
            if (slideId is null)
            {
                return;
            }
            var slidePart = (SlidePart)presentationDocument.PresentationPart!.GetPartById(slideId.RelationshipId!);
            var slide = slidePart.Slide;


            var selectedItem = ShapeTypeComboBox.SelectedItem;

            var shapeTypeValues = Enum.Parse<A.ShapeTypeValues>(selectedItem.ToString());
            var shape = CreateShape(shapeTypeValues);
            slide.CommonSlideData?.ShapeTree?.Append(shape);


    
            presentationDocument.Save();
            presentationDocument.Dispose();

            string pptxFilePath = PptxFilePathTextBox.Text;
            int slideIndex = 1; // 要截取的幻灯片索引，从1开始

            var tempPath = Path.GetTempPath();
            var outputImagePath = Path.Combine(tempPath,$"{Guid.NewGuid():N}.png");

            CaptureSlide(pptxFilePath, slideIndex, outputImagePath);
        }

        public void CaptureSlide(string pptxFilePath, int slideIndex, string outputImagePath)
        {
            // 创建PowerPoint应用程序对象
            Application pptApplication = new Application();

            // 打开PPTX文件
            Presentation presentation = pptApplication.Presentations.Open(pptxFilePath, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            // 获取指定索引的幻灯片
            OfficeSlide slide = presentation.Slides[slideIndex];

            // 截图并保存到指定路径
            slide.Export(outputImagePath, "png",1920,1080);

            // 关闭PowerPoint应用程序
            presentation.Close();
            //Marshal.ReleaseComObject(presentation);
            pptApplication.Quit();
            //Marshal.ReleaseComObject(pptApplication);

            PptxSlideImage.Source = new BitmapImage(new Uri(outputImagePath));
        }

       private readonly Random _random = new Random();
        private void ShapeTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WidthTextBox.Text = _random.Next(150, 300).ToString();
            HeightTextBox.Text = _random.Next(150, 300).ToString();
            XTextBox.Text = _random.Next(0, 600).ToString();
            YTextBox.Text = _random.Next(0, 600).ToString();
        }
    }
}