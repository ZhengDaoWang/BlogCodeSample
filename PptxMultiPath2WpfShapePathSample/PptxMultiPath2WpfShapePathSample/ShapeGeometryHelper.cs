using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PptxMultiPath2WpfShapePathSample.CommonHelper;

namespace PptxMultiPath2WpfShapePathSample
{
    public static class ShapeGeometryHelper
    {

        /// <summary>
        /// 获取ppt形状计算公式的属性值
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// 可参考文档 dotnet OpenXML SDK 形状几何 Geometry 的计算公式含义.md
        private static (double h, double w, double l, double r, double t, double b, double hd2, double hd4, double hd5, double hd6, double hd8, double ss, double hc, double vc, double ls, double ss2, double ss4, double ss6, double ss8, double wd2, double wd4, double wd5, double wd6, double wd8, double wd10, double cd2, double cd4, double cd8) GetFormulaProperties(double width, double height)
        {
            height = new Pixel(height).PixelToEmu().Value;
            width = new Pixel(width).PixelToEmu().Value;
            var h = height;
            var w = width;
            var l = 0d;
            var r = width;
            var t = 0d;
            var b = height;
            var hd2 = height / 2;
            var hd4 = height / 4;
            var hd5 = height / 5;
            var hd6 = height / 6;
            var hd8 = height / 8;
            var ss = Math.Min(height, width);
            var hc = width / 2;
            var vc = height / 2;
            var ls = Math.Max(height, width);
            var ss2 = ss / 2;
            var ss4 = ss / 4;
            var ss6 = ss / 6;
            var ss8 = ss / 8;
            var wd2 = width / 2;
            var wd4 = width / 4;
            var wd5 = width / 5;
            var wd6 = width / 6;
            var wd8 = width / 8;
            var wd10 = width / 10;
            // 1°= 60000.0 Degree   c = circle = 360°= 60000.0 * 360 = 21600000 Degree
            const double c = 21600000d;
            var cd2 = c / 2;
            var cd4 = c / 4;
            var cd8 = c / 8;
            return (h, w, l, r, t, b, hd2, hd4, hd5, hd6, hd8, ss, hc, vc, ls, ss2, ss4, ss6, ss8, wd2, wd4, wd5, wd6, wd8, wd10, cd2, cd4, cd8);
        }


        public static long EmuToPixel(this long eum)
        {
            const long defaultDpi = 96;
            return eum / 914400 * defaultDpi;
        }

        /// <summary>
        /// 获取缩放后的【标注：弯曲线形】
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List<ShapePath> GetGeometryPathFromCallout2(double width, double height)
        {
            var (h, w, l, r, t, b, hd2, hd4, hd5, hd6, hd8, ss, hc, vc, ls, ss2, ss4, ss6, ss8, wd2, wd4, wd5, wd6, wd8, wd10, cd2, cd4, cd8) = GetFormulaProperties(width, height);
            //<avLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <gd name="adj1" fmla="val 18750" />
            //  <gd name="adj2" fmla="val -8333" />
            //  <gd name="adj3" fmla="val 18750" />
            //  <gd name="adj4" fmla="val -16667" />
            //  <gd name="adj5" fmla="val 112500" />
            //  <gd name="adj6" fmla="val -46667" />
            //</avLst>
            var adj1 = 18750d;
            var adj2 = -8333d;
            var adj3 = 18750d;
            var adj4 = -16667d;
            var adj5 = 112500d;
            var adj6 = -46667;

            //<gdLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <gd name="y1" fmla="*/ h adj1 100000" />
            //  <gd name="x1" fmla="*/ w adj2 100000" />
            //  <gd name="y2" fmla="*/ h adj3 100000" />
            //  <gd name="x2" fmla="*/ w adj4 100000" />
            //  <gd name="y3" fmla="*/ h adj5 100000" />
            //  <gd name="x3" fmla="*/ w adj6 100000" />
            //</gdLst>

            //  <gd name="y1" fmla="*/ h adj1 100000" />
            var y1 = h * adj1 / 100000;
            //  <gd name="x1" fmla="*/ w adj2 100000" />
            var x1 = w * adj2 / 100000;
            //  <gd name="y2" fmla="*/ h adj3 100000" />
            var y2 = h * adj3 / 100000;
            //  <gd name="x2" fmla="*/ w adj4 100000" />
            var x2 = w * adj4 / 100000;
            //  <gd name="y3" fmla="*/ h adj5 100000" />
            var y3 = h * adj5 / 100000;
            //  <gd name="x3" fmla="*/ w adj6 100000" />
            var x3 = w * adj6 / 100000;

            // <pathLst xmlns="http://schemas.openxmlformats.org/drawingml/2006/main">
            //  <path stroke="false" extrusionOk="false">
            //    <moveTo>
            //      <pt x="l" y="t" />
            //    </moveTo>
            //    <lnTo>
            //      <pt x="r" y="t" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="r" y="b" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="l" y="b" />
            //    </lnTo>
            //    <close />
            //  </path>
            //  <path fill="none" extrusionOk="false">
            //    <moveTo>
            //      <pt x="x1" y="y1" />
            //    </moveTo>
            //    <lnTo>
            //      <pt x="x2" y="y2" />
            //    </lnTo>
            //    <lnTo>
            //      <pt x="x3" y="y3" />
            //    </lnTo>
            //  </path>
            //</pathLst>

            var pathLst = new List<ShapePath>();

            //  <path extrusionOk="false">
            //    <moveTo>
            //      <pt x="l" y="t" />
            //    </moveTo>
            var currentPoint = new EmuPoint(l, t);
            var stringPath = new StringBuilder();
            stringPath.Append($"M {EmuToPixelString(currentPoint.X)},{EmuToPixelString(currentPoint.Y)} ");
            //    <lnTo>
            //      <pt x="r" y="t" />
            //    </lnTo>
            currentPoint = LineToToString(stringPath, r, t);
            //    <lnTo>
            //      <pt x="r" y="b" />
            //    </lnTo>
            currentPoint = LineToToString(stringPath, r, b);
            //    <lnTo>
            //      <pt x="l" y="b" />
            //    </lnTo>
            currentPoint = LineToToString(stringPath, l, b);
            //    <close />
            stringPath.Append("z ");

            pathLst.Add(new ShapePath(stringPath.ToString(),isStroke:false));


            //  <path fill="none" extrusionOk="false">
            //    <moveTo>
            //      <pt x="x1" y="y1" />
            //    </moveTo>
            stringPath.Clear();
            currentPoint = new EmuPoint(x1, y1);
            stringPath.Append($"M {EmuToPixelString(currentPoint.X)},{EmuToPixelString(currentPoint.Y)} ");
            //    <lnTo>
            //      <pt x="x2" y="y2" />
            //    </lnTo>
            currentPoint = LineToToString(stringPath, x2, y2);
            //    <lnTo>
            //      <pt x="x3" y="y3" />
            //    </lnTo>
            _ = LineToToString(stringPath, x3, y3);

            pathLst.Add(new ShapePath(stringPath.ToString(), FillMode.None));


            return pathLst;

        }

        private static EmuPoint LineToToString(StringBuilder stringPath, double xEmu, double yEmu)
        {
            return CommonHelper.LineToToString(stringPath, new Emu(xEmu), new Emu(yEmu));
        }


        /// <summary>
        /// OpenXml 三角函数的Sin函数：sin x y = (x * sin( y )) = (x * Math.Sin(y))
        /// </summary>
        /// <param name="x">ppt的数值</param>
        /// <param name="y">ppt表示角度的值</param>
        /// <returns></returns>
        public static double Sin(double x, int y)
        {
            var angle = GetAngle(y);
            return x * Math.Sin(angle);
        }

        /// <summary>
        /// OpenXml 三角函数的Cos函数：cos x y = (x * cos( y )) = (x * Math.Cos(y))
        /// </summary>
        /// <param name="x">ppt的数值</param>
        /// <param name="y">ppt表示角度的值</param>
        /// <returns></returns>
        public static double Cos(double x, int y)
        {
            var angle = GetAngle(y);
            return x * Math.Cos(angle);
        }

        /// <summary>
        /// OpenXml 三角函数的Tan函数：tan x y = (x * tan( y )) = (x * Math.Tan(y))
        /// </summary>
        /// <param name="x">ppt的数值</param>
        /// <param name="y">ppt表示角度的值</param>
        /// <returns></returns>
        public static double Tan(double x, int y)
        {
            var angle = GetAngle(y);
            return x * Math.Tan(angle);
        }

        /// <summary>
        /// ppt的值转为角度
        /// </summary>
        /// <param name="value">ppt表示角度的值</param>
        /// <returns></returns>
        private static double GetAngle(int value)
        {
            var degree = value / 60000.0;
            var angle = degree * Math.PI / 180;
            return angle;
        }
    }
}
