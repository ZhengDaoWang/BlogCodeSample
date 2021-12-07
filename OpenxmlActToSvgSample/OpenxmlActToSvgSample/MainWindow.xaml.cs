using dotnetCampus.OpenXmlUnitConverter;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OpenxmlActToSvgSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Openxml的360° circle
            const double c = 21600000d;
            //circle divide 4
            var cd4 = c / 4;

            //<arcTo wR="152403" hR="152403" stAng="cd4" swAng="-5400000" />
            var wR = 152403;
            var hR = 152403;
            var stAng = cd4;
            var swAng = -5400000d;


            StringBuilder stringPath=new StringBuilder();
            var currentPoint=new Point(0, 0);
            stringPath.Append($"M {currentPoint.X} {currentPoint.Y}");

            ParseOpenxmlArcTo(stringPath, wR, hR, stAng, swAng, currentPoint);
            this.Path.Data=Geometry.Parse(stringPath.ToString());
        }

        private Point  ParseOpenxmlArcTo(StringBuilder stringPath, double wR, double hR, double stAng, double swAng, Point currentPoint)
        {
            const string comma = ",";

            //将Openxml的角度转为真实的角度
            var θ1 = new Angle((int)stAng).ToRadiansValue();
            var Δθ = new Angle((int)swAng).ToRadiansValue();
            //旋转角
            var φ = 0d;
            //是否是大弧
            var isLargeArcFlag = Math.Abs(Δθ) > Math.PI;
            //是否是顺时针
            var isClockwise = Δθ > 0;


            //修复当椭圆弧线进行360°时，起始点和终点一样，会导致弧线变成点，因此-1°才进行计算
            if (System.Math.Abs(Δθ) == 2 * System.Math.PI)
            {
                Δθ = Δθ - Δθ / 360;
            }

            var rx = new Emu(wR).ToPixel().Value;
            var ry = new Emu(hR).ToPixel().Value;

            //获取终点坐标
            var pt = GetArBitraryPoint(rx, ry, Δθ, θ1, φ, currentPoint);

            currentPoint = pt;

            // 格式如下
            // A rx ry x-axis-rotation large-arc-flag sweep-flag x y
            // 这里 large-arc-flag 是 1 和 0 表示
            stringPath.Append("A")
                   .Append(rx) //rx
                   .Append(comma)
                   .Append(ry) //ry
                   .Append(comma)
                   .Append(φ) // x-axis-rotation
                   .Append(comma)
                   .Append(isLargeArcFlag ? "1" : "0") //large-arc-flag
                   .Append(comma)
                   .Append(isClockwise ? "1" : "0") // sweep-flag
                   .Append(comma)
                   .Append(pt.X)
                   .Append(comma)
                   .Append(pt.Y)
                   .Append(' ');
            return currentPoint;

        }

        /// <summary>
        /// 获取椭圆任意一点坐标
        /// </summary>
        /// <returns></returns>
        private static Point GetArBitraryPoint(double rx, double ry, double Δθ, double θ1, double φ, Point currentPoint)
        {
            //开始点的椭圆任意一点的二维矩阵方程式
            var matrixX1Y1 = DenseMatrix.OfArray(new double[2, 1]
            {
                { currentPoint.X},
                { currentPoint.Y}
            });

            var matrix1 = DenseMatrix.OfArray(new double[2, 2]
            {
            { Math.Cos(φ),-Math.Sin(φ)},
            { Math.Sin(φ),Math.Cos(φ)}
            });
            var matrix2 = DenseMatrix.OfArray(new double[2, 1]
            {
                { rx*Math.Cos(θ1)},
                { ry*Math.Sin(θ1)}
            });
            var matrixCxCy = matrixX1Y1 - (matrix1 * matrix2);

            //终点的椭圆任意一点的二维矩阵方程式
            var matrix3 = DenseMatrix.OfArray(new double[2, 1]
            {
                { rx*Math.Cos(θ1+Δθ)},
                { ry*Math.Sin(θ1+Δθ)}
            });

            var matrixX2Y2 = matrix1 * matrix3 + matrixCxCy;

            return new Point(matrixX2Y2.Values[0], matrixX2Y2.Values[1]);

        }
    }
}
