using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System;
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

                    //解析多媒体
                    AnalysePPT_Media(presentationPart);
                }
            }

        }

        /// <summary>
        /// 解析视频
        /// </summary>
        /// <param name="presentationPart"></param>
        private void AnalysePPT_Video(DocumentFormat.OpenXml.Presentation.Picture video, SlidePart slidePart)
        {
            var videoFromFile = video.NonVisualPictureProperties
                .ApplicationNonVisualDrawingProperties
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.VideoFromFile>();


            var openxmlPart_video = slidePart.GetReferenceRelationship(videoFromFile.Link.Value) as DataPartReferenceRelationship;
            //读取视频流
            var videoStream = openxmlPart_video.DataPart.GetStream();
            var file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Guid.NewGuid().ToString()}.mp4");
            File.WriteAllBytes(file, ReadAllBytes(videoStream));

            this.mediaPlayer.Source = new System.Uri(file);
            this.mediaPlayer.Play();
        }

        /// <summary>
        /// 解析图片
        /// </summary>
        /// <param name="presentationPart"></param>
        private void AnalysePPT_Picture(DocumentFormat.OpenXml.Presentation.Picture picture, SlidePart slidePart)
        {
            var pictureFromFile = picture.BlipFill.Blip.Embed.Value;
            var openxmlPart_picture = slidePart.GetPartById(pictureFromFile);
            var pictureStream = openxmlPart_picture.GetStream();
            var file_picture = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Guid.NewGuid().ToString()}.jpg");
            File.WriteAllBytes(file_picture, ReadAllBytes(pictureStream));
        }
        /// <summary>
        /// 解析音频
        /// </summary>
        /// <param name="presentationPart"></param>
        private void AnalysePPT_Audio(DocumentFormat.OpenXml.Presentation.Picture audio,SlidePart slidePart)
        {
            var videoFromFile = audio.NonVisualPictureProperties
                .ApplicationNonVisualDrawingProperties
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.AudioFromFile>();


            var openxmlPart_video = slidePart.GetReferenceRelationship(videoFromFile.Link.Value) as DataPartReferenceRelationship;
            //音频
            var videoStream = openxmlPart_video.DataPart.GetStream();
            var file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Guid.NewGuid().ToString()}.mp3");
            File.WriteAllBytes(file, ReadAllBytes(videoStream));

            //this.mediaPlayer.Source = new System.Uri(file);
            //this.mediaPlayer.Play();
        }

        private void AnalysePPT_Media(PresentationPart presentationPart)
        {
            var slidePart_Midea = presentationPart.SlideParts.FirstOrDefault();
            foreach (var media in slidePart_Midea.Slide.CommonSlideData.ShapeTree.OfType<DocumentFormat.OpenXml.Presentation.Picture>())
            {
                var mediaType = CheckMediaType(media);
                switch (mediaType)
                {
                    case MediaTypeEnum.Audio:
                        AnalysePPT_Audio(media, slidePart_Midea);
                        break;
                    case MediaTypeEnum.Picture:
                        AnalysePPT_Picture(media, slidePart_Midea);
                        break;
                    case MediaTypeEnum.Video:
                        AnalysePPT_Video(media, slidePart_Midea);
                        break;
                    case MediaTypeEnum.Unknown:
                        break;
                }
            }
        }


        private MediaTypeEnum CheckMediaType(DocumentFormat.OpenXml.Presentation.Picture media)
        {
            var audioFromFile = media.NonVisualPictureProperties
                .ApplicationNonVisualDrawingProperties
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.AudioFromFile>();
            var pictureFromFile = media.BlipFill.Blip.Embed.Value;

            var videoFromFile = media.NonVisualPictureProperties
                .ApplicationNonVisualDrawingProperties
                .GetFirstChild<DocumentFormat.OpenXml.Drawing.VideoFromFile>();

            if (audioFromFile != null)
            {
                return MediaTypeEnum.Audio;
            }
            else if (videoFromFile!=null)
            {
                return MediaTypeEnum.Video;
            }
            else if (!string.IsNullOrEmpty(pictureFromFile) && videoFromFile == null && audioFromFile == null)
            {
                return MediaTypeEnum.Picture;
            }
            else
            {
                return MediaTypeEnum.Unknown;
            }
        }

        /// <summary>
        /// 解析文本
        /// </summary>
        /// <param name="presentationPart"></param>
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


        enum MediaTypeEnum
        {
            Unknown = 0,
            Picture = 1,
            Video = 2,
            Audio = 3
        }
    }
}
