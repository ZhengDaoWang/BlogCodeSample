using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace PPTanalysisSample
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

        private void btn_analysis_Click(object sender, RoutedEventArgs e)
        {
            //var folder =@"..\..\"+ System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = "RyzenTest.pptx";
            if (File.Exists(filePath))
            {
                this.txt_result.Text = string.Empty;
                this.mediaPlayer.Source = null;
                using (var presentationDocument = PresentationDocument.Open(filePath, false))
                {
                    var presentationPart = presentationDocument.PresentationPart;
                    var presentation = presentationPart.Presentation;

                    //解析文本
                    AnalysePPT_Text(presentationPart);

                    //解析视频
                    AnalysePPT_Media(presentationPart);
                }
            }

        }

        private void AnalysePPT_Media(PresentationPart presentationPart)
        {

            var slidePart_Midea = presentationPart.SlideParts.FirstOrDefault();
            var picture = slidePart_Midea.Slide.CommonSlideData.ShapeTree.OfType<DocumentFormat.OpenXml.Presentation.Picture>().FirstOrDefault();

            var videoFromFile = picture.NonVisualPictureProperties
                .ApplicationNonVisualDrawingProperties
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.VideoFromFile>();

            var openxmlPart = slidePart_Midea.GetReferenceRelationship(videoFromFile.Link.Value) as DataPartReferenceRelationship;

            //读取视频流
            var mediaStream = openxmlPart.DataPart.GetStream();
            var file =System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"RyzenTest.mp4");
            File.WriteAllBytes(file, ReadAllBytes(mediaStream));

            this.mediaPlayer.Source = new System.Uri(file);
            this.mediaPlayer.Play();

        }

        private void AnalysePPT_Text(PresentationPart presentationPart)
        {

            var presentation = presentationPart.Presentation;

            var slideIdList = presentation.SlideIdList;

            foreach (var slideId in slideIdList.ChildElements.OfType<SlideId>())
            {
                //获取页面内容
                var slidePart = presentationPart.GetPartById(slideId.RelationshipId) as SlidePart;
                var slide = slidePart.Slide;

                foreach (var paragraph in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                {
                    foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                    {
                        this.txt_result.Text += $"{text.Text} ";
                    }
                }
            }

        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
