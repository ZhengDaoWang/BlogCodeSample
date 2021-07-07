using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PptxMultiPath2WpfShapePathSample
{
    public static class ShapeGeometryHelper
    {

        public readonly struct FormulaProperties
        {
            public readonly double t;
            public readonly double h;
            public readonly double w;
            public readonly double hd2;
            public readonly double wd2;
            public readonly double vc;
            public readonly double hc;

            public FormulaProperties(double width, double height)
            {
                t = 0;
                w = width;
                h = height;
                hd2 = h / 2;
                wd2 = w / 2;
                vc = height / 2;
                hc = width / 2;
            }
        }


        public static long EmuToPixel(this long eum)
        {
            const long defaultDpi = 96;
            return eum / 914400 * defaultDpi;
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
