using System.Reflection;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Path = System.IO.Path;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;

namespace CreateOpenXmlHyperlinkSample
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var mainExecuteDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var pptFilePath = Path.Combine(mainExecuteDirectory!, "YourPresentation.pptx");


            // 打开一个PPTX文档
            using PresentationDocument presentationDocument = PresentationDocument.Open(pptFilePath, true);

            var slideIdList = presentationDocument.PresentationPart!.Presentation.SlideIdList;
            if (slideIdList is null || !slideIdList.Any())
            {
                return;
            }

            var firstSlideId =(SlideId) slideIdList.First();
            // 获取页面内容
            var firstSlidePart = (SlidePart) presentationDocument.PresentationPart.GetPartById(firstSlideId.RelationshipId!.Value!);



            //设置跳转到第三页
            var targetSlideId = (SlideId) slideIdList.ChildElements[2];
            var targetSlidePart = (SlidePart) presentationDocument.PresentationPart.GetPartById(targetSlideId.RelationshipId!.Value!);
            var relationshipId = firstSlidePart.CreateRelationshipToPart(targetSlidePart);

            var shapeElements = firstSlidePart.Slide.CommonSlideData!.ShapeTree!.Elements<Shape>().ToList();

            //第一个形状设置跳转第三页
            shapeElements[0]!.NonVisualShapeProperties!.NonVisualDrawingProperties!.HyperlinkOnClick = new HyperlinkOnClick()
            {
                Action = PptAction.SlideJump,
                Id = relationshipId
            };


            //第二个形状设置跳转下一页
            var shapeElement = shapeElements[1];
            shapeElement.NonVisualShapeProperties!.NonVisualDrawingProperties!.HyperlinkOnClick = new HyperlinkOnClick()
            {
                Action = PptAction.JumpNextSlide,
            };

            var filePath = Path.Combine(mainExecuteDirectory!, "两只老虎-原声.mp3");
            var fileHyperlinkRelationship = firstSlidePart.AddHyperlinkRelationship(new Uri(filePath, UriKind.Absolute), true);
            //第三个形状设置打开文件
            shapeElements[2]!.NonVisualShapeProperties!.NonVisualDrawingProperties!.HyperlinkOnClick = new HyperlinkOnClick()
            {
                Action = PptAction.OpenFile,
                Id = fileHyperlinkRelationship.Id
            };

            //第四个形状设置打开网页链接
            var httpHyperlinkRelationship = firstSlidePart.AddHyperlinkRelationship(new Uri($"http://www.baidu.com", UriKind.Absolute), true);
            shapeElements[3]!.NonVisualShapeProperties!.NonVisualDrawingProperties!.HyperlinkOnClick = new HyperlinkOnClick()
            {
                Id = httpHyperlinkRelationship.Id
            };

            // 保存并关闭文档
            presentationDocument.Save();
        }
    }

    public static class PptAction
    {
        /// <summary>
        /// 跳转页面
        /// </summary>
        public const string SlideJump = "ppaction://hlinksldjump";

        /// <summary>
        /// 跳转下一页
        /// </summary>
        public const string JumpNextSlide = "ppaction://hlinkshowjump?jump=nextslide";

        /// <summary>
        /// 打开文件
        /// </summary>
        public const string OpenFile = "ppaction://hlinkfile";

    }
}
