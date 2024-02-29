using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;

namespace CreateOpenXmlHyperlinkSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 打开一个PPTX文档
            using PresentationDocument presentationDocument = PresentationDocument.Open(@"C:\Users\11019\Downloads\YourPresentation.pptx", true);

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

            var shape = firstSlidePart.Slide.CommonSlideData!.ShapeTree!.GetFirstChild<Shape>();
            shape!.NonVisualShapeProperties!.NonVisualDrawingProperties!.HyperlinkOnClick = new HyperlinkOnClick()
            {
                Action = PptAction.SlideJump,
                Id = relationshipId
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
    }
}
