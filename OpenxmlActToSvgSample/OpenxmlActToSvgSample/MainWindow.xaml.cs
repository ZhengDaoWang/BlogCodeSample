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
            var wR = 172403;
            var hR = 152403;
            var stAng = c / 2;
            var swAng = 21600000d / 3;


            StringBuilder stringPath = new StringBuilder();
            var currentPoint = new Point(0, 0);
            stringPath.Append($"M {currentPoint.X} {currentPoint.Y}");

            var arcStr = OpenXmlArcToArcStr(stringPath, wR, hR, stAng, swAng, currentPoint);
            this.Path.Data = Geometry.Parse(arcStr);
        }

        /// <summary>
        /// OpenXml Arc 转为SVG Arc 字符串
        /// </summary>
        /// <param name="stringPath">路径字符串</param>
        /// <param name="wR">Emu椭圆半长轴</param>
        /// <param name="hR">Emu椭圆半短轴</param>
        /// <param name="stAng">Emu起始角</param>
        /// <param name="swAng">Emu摆动角</param>
        /// <param name="currentPoint">当前坐标</param>
        /// <returns></returns>
        private string OpenXmlArcToArcStr(StringBuilder stringPath, double wR, double hR, double stAng, double swAng, Point currentPoint)
        {
            const string comma = ",";

            //将Openxml的角度转为真实的角度
            var θ1 = new Angle((int)stAng).ToRadiansValue();
            var Δθ = new Angle((int)swAng).ToRadiansValue();
            //旋转角
            var φ = 60d;
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
            return stringPath.ToString();

        }


        /// <summary>
        /// OpenXml Arc 转为SVG Arc 字符串
        /// </summary>
        /// <param name="stringPath">路径字符串</param>
        /// <param name="rx">椭圆半长轴</param>
        /// <param name="ry">椭圆半短轴</param>
        /// <param name="φ">旋转角</param>
        /// <param name="stAng">起始角</param>
        /// <param name="swAng">摆动角</param>
        /// <param name="currentPoint">当前坐标</param>
        /// <returns></returns>
        private string OpenXmlArcToArcStrNew(StringBuilder stringPath, double rx, double ry, double φ, double stAng, double swAng, Point currentPoint)
        {
            const string comma = ",";

            //将Openxml的角度转为真实的角度
            var θ1 = stAng / 180 * Math.PI;
            var Δθ = swAng / 180 * Math.PI;
            //是否是大弧
            var isLargeArcFlag = Math.Abs(Δθ) > Math.PI;
            //是否是顺时针
            var isClockwise = Δθ > 0;


            //修复当椭圆弧线进行360°时，起始点和终点一样，会导致弧线变成点，因此-1°才进行计算
            if (System.Math.Abs(Δθ) == 2 * System.Math.PI)
            {
                Δθ = Δθ - Δθ / 360;
            }

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
            return stringPath.ToString();

        }

        /// <summary>
        /// 获取椭圆任意一点坐标
        /// </summary>
        /// <param name="rx">椭圆半长轴</param>
        /// <param name="ry">椭圆半短轴</param>
        /// <param name="Δθ">摆动角度(起始角的摆动角度，也就是起始角+摆动角=结束角)</param>
        /// <param name="θ1">起始角</param>
        /// <param name="φ">旋转角</param>
        /// <param name="currentPoint">起点</param>
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.Path.Data is not null)
            {
                var pathGeometry = PathGeometry.CreateFromGeometry(this.Path.Data);
                var pathFigure = pathGeometry.Figures[0];

                if (pathFigure.Segments[0] is ArcSegment arcSegment)
                {
                    var x1 = pathFigure.StartPoint.X;
                    var y1 = pathFigure.StartPoint.Y;
                    var rx = arcSegment.Size.Width;
                    var ry = arcSegment.Size.Height;
                    var phi = arcSegment.RotationAngle;
                    var fA = arcSegment.IsLargeArc ? 1 : 0;
                    var fs = arcSegment.SweepDirection is SweepDirection.Clockwise ? 1 : 0;
                    var x2 = arcSegment.Point.X;
                    var y2 = arcSegment.Point.Y;


                    var (startAngle, swAngle) = GetArcStartAngAndSwAng(x1, y1, x2, y2, fA, fs, rx, ry, phi);
                    StringBuilder stringPath = new StringBuilder();
                    var currentPoint = new Point(0, 0);
                    stringPath.Append($"M {x1} {y1}");
                    var openXmlArcToArcStrNew = OpenXmlArcToArcStrNew(stringPath, rx, ry, phi, startAngle, swAngle, pathFigure.StartPoint);
                    this.NewPath.Data = Geometry.Parse(openXmlArcToArcStrNew);
                }

            }
        }

        /// <summary>
        /// 获取弧线的开始角度和摆动角度
        /// </summary>
        /// <param name="x1">起点X</param>
        /// <param name="y1">起点Y</param>
        /// <param name="x2">终点X</param>
        /// <param name="y2">终点Y</param>
        /// <param name="fA">优劣弧:1 优弧  0劣弧</param>
        /// <param name="fs">顺逆时针绘制：1 顺时针  0 逆时针</param>
        /// <param name="rx">椭圆半长轴</param>
        /// <param name="ry">椭圆半短轴</param>
        /// <param name="φ">旋转角</param>
        /// <returns></returns>
        private static (double startAngle, double swAngle) GetArcStartAngAndSwAng(double x1, double y1, double x2, double y2, double fA, double fs, double rx, double ry, double φ)
        {
            //开始点的椭圆任意一点的二维矩阵方程式
            var matrixX1Y1 = DenseMatrix.OfArray(new double[2, 2]
                {
                    {Math.Cos(φ),Math.Sin(φ)},
                    {-Math.Sin(φ),Math.Cos(φ)}
                }) *
            DenseMatrix.OfArray(new double[2, 1]
            {
                { (x1-x2)/2},
                { (y1-y2)/2}
            });

            var x1_ = matrixX1Y1.Values[0];
            var y1_ = matrixX1Y1.Values[1];

            var a = Math.Pow(rx, 2) * Math.Pow(ry, 2) - Math.Pow(rx, 2) * Math.Pow(y1_, 2) - Math.Pow(ry, 2) * Math.Pow(x1_, 2);
            var b = Math.Pow(ry, 2) * Math.Pow(y1_, 2) + Math.Pow(ry, 2) * Math.Pow(x1_, 2);

            var matrixCxCy = Math.Sqrt(a / b) * DenseMatrix.OfArray(new[,]
            {
                { rx*y1_/ry},
                { -ry*x1_/rx}
            });

            var cx_ = matrixCxCy.Values[0];
            var cy_ = matrixCxCy.Values[1];


            //求开始角
            //cos<夹角> = 两向量之积 / 两向量模的乘积
            //< 夹角 > = arcCos(两向量之积 / 两向量模的乘积)

            //向量U的坐标
            double uv_x = 1;
            double uv_y = 0;

            //向量V的坐标
            double vv_x = (x1_ - cx_) / rx;
            double vv_y = (y1_ - cy_) / ry;


            var multi = uv_x * vv_x + uv_y * vv_y; //两向量的乘积
            var vu_val = Math.Sqrt(uv_x * uv_x + uv_y * uv_y);//向量U的模
            var vv_val = Math.Sqrt(vv_x * vv_x + vv_y * vv_y);//向量V的模
            var cosResult = multi / (vu_val * vv_val);

            var startAngle = Math.Acos(cosResult) * 180 / Math.PI;


            //求摆动角
            //cos<夹角> = 两向量之积 / 两向量模的乘积
            //< 夹角 > = arcCos(两向量之积 / 两向量模的乘积)

            //向量U的坐标
            uv_x = (x1_ - cx_) / rx;
            uv_y = (y1_ - cy_) / ry;

            //向量V的坐标
            vv_x = (-x1_ - cx_) / rx;
            vv_y = (-y1_ - cy_) / ry;

            multi = uv_x * vv_x + uv_y * vv_y; //两向量的乘积
            vu_val = Math.Sqrt(uv_x * uv_x + uv_y * uv_y);//向量U的模
            vv_val = Math.Sqrt(vv_x * vv_x + vv_y * vv_y);//向量V的模
            cosResult = multi / (vu_val * vv_val);

            var swAngle = Math.Acos(cosResult) * 180 / Math.PI;

            if (fs == 0)
            {
                swAngle = -swAngle;
            }
            else
            {
                swAngle = Math.Abs(swAngle);
            }


            return (startAngle, swAngle);

        }


    }
}
