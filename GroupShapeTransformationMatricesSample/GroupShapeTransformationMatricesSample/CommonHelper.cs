using dotnetCampus.OpenXmlUnitConverter;
using System;
using System.Text;

namespace GroupShapeTransformationMatricesSample
{
    public static class CommonHelper
    {

        private static EmuPoint GetEllipsePoint(double a, double b, double theta)
        {
            if (a == 0 || b == 0)
            {
                return new EmuPoint(new Emu(0), new Emu(0));
            }
            var aSinTheta = a * Math.Sin(theta);
            var bCosTheta = b * Math.Cos(theta);
            var circleRadius = Math.Sqrt(aSinTheta * aSinTheta + bCosTheta * bCosTheta);
            return new EmuPoint(new Emu(a * bCosTheta / circleRadius), new Emu(b * aSinTheta / circleRadius));
        }

        public static string PixelToString(Pixel x) =>
            // 太小了很看不到形状，丢失精度，这里的值都是采用形状的大小进行填充，所以参数都是相对大小就可以
            (x.Value * 1.000).ToString("0.000");

        public static string EmuToPixelString(this double emuValue)
        {
            var emu = new Emu(emuValue);
            return EmuToPixelString(emu);
        }

        public static string EmuToPixelString(Emu emu)
        {
            return PixelToString(emu.ToPixel());
        }

        public static double DegreeToRadiansAngle(Degree x)
        {
            return x.DoubleValue * Math.PI / 180;
        }

        private const string Comma = ",";

        public static EmuPoint ArcToToString(StringBuilder stringPath, EmuPoint currentPoint,
            Emu widthRadius,
            Emu heightRadius,
            Degree startAngleString, Degree swingAngleString)
        {
            const string comma = Comma;

            var stAng = DegreeToRadiansAngle(startAngleString);
            var swAng = DegreeToRadiansAngle(swingAngleString);

            var wR = widthRadius.Value;
            var hR = heightRadius.Value;

            var p1 = GetEllipsePoint(wR, hR, stAng);
            var p2 = GetEllipsePoint(wR, hR, stAng + swAng);
            var pt = new EmuPoint(new Emu(currentPoint.X.Value - p1.X.Value + p2.X.Value),
              new Emu(currentPoint.Y.Value - p1.Y.Value + p2.Y.Value));

            var isLargeArcFlag = swAng >= Math.PI;
            currentPoint = pt;

            // 格式如下
            // A rx ry x-axis-rotation large-arc-flag sweep-flag x y
            // 这里 large-arc-flag 是 1 和 0 表示
            stringPath.Append("A")
                .Append(EmuToPixelString(wR)) //rx
                .Append(comma)
                .Append(EmuToPixelString(hR)) //ry
                .Append(comma)
                .Append("0") // x-axis-rotation
                .Append(comma)
                .Append(isLargeArcFlag ? "1" : "0") //large-arc-flag
                .Append(comma)
                .Append("1") // sweep-flag
                .Append(comma)
                .Append(EmuToPixelString(pt.X))
                .Append(comma)
                .Append(EmuToPixelString(pt.Y))
                .Append(' ');
            return currentPoint;
        }

        public static EmuPoint LineToToString(StringBuilder stringPath, Emu x, Emu y)
        {
            stringPath.Append($"L {EmuToPixelString(x)},{EmuToPixelString(y)} ");

            return new EmuPoint(x, y);
        }

        public static EmuPoint CubicBezToString(StringBuilder stringPath, Emu x1, Emu y1, Emu x2, Emu y2, Emu x, Emu y)
        {
            stringPath.Append($"C {EmuToPixelString(x1)},{EmuToPixelString(y1)},{EmuToPixelString(x2)},{EmuToPixelString(y2)},{EmuToPixelString(x)},{EmuToPixelString(y)}");

            return new EmuPoint(x, y);
        }

        public static double Pin(double x, double y, double z)
        {
            if (y < x)
            {
                return x;
            }
            else if (y > z)
            {
                return z;
            }
            else
            {
                return y;
            }
        }

        public static double Sin(double x, int y)
        {
            var degree = new Degree(y).DoubleValue;
            var angle = GetAngle(degree);
            var result = x * Math.Sin(angle);
            return result;
        }

        public static double Cos(double x, int y)
        {
            var degree = new Degree(y).DoubleValue;
            var angle = GetAngle(degree);
            var result = x * Math.Cos(angle);
            return result;
        }

        private static double GetAngle(double degree)
        {
            var angle = degree * Math.PI / 180;
            return angle;
        }

        public static double Abs(double value)
        {
            return Math.Abs(value);
        }

        public static double ATan2(double x, double y)
        {
            var radians = Math.Atan2(y, x);
            var angle = radians * 180 / Math.PI;
            return angle * 60000;
        }

        public static double Cat2(double x, double y, double z)
        {
            var angle = ATan2(y, z);
            var result = Cos(x, (int)angle);
            return result;
        }

        public static double Sat2(double x, double y, double z)
        {
            var angle = ATan2(y, z);
            var result = Sin(x, (int)angle);
            return result;
        }

        public static double Mod(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }
    }
}
