using dotnetCampus.OpenXmlUnitConverter;
using System;
using System.Text;

namespace GroupShapeTransformationMatricesSample
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

        /// <summary>
        /// 线条坐标信息转字符串
        /// </summary> 
        /// <param name="stringPath">路径字符串</param>
        /// <param name="xEmu">eum单位的x坐标</param>
        /// <param name="yEmu">eum单位的y坐标</param>
        /// <returns>目标点坐标</returns>
        public static EmuPoint LineToToString(StringBuilder stringPath, double xEmu, double yEmu)
        {
            return CommonHelper.LineToToString(stringPath, new Emu(xEmu), new Emu(yEmu));
        }

        public static long EmuToPixel(this long eum)
        {
            const long defaultDpi = 96;
            return eum / 914400 * defaultDpi;
        }
    }
}
