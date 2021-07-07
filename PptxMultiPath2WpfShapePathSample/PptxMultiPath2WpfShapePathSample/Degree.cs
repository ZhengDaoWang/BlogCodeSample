using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PptxMultiPath2WpfShapePathSample
{
    /// <summary>
    ///     表示openxml中的角度数值
    /// </summary>
    public class Degree
    {
        private const int MaxDegreeValue = 21600000;

        /// <summary>
        ///     参阅 http://officeopenxml.com/drwSp-custGeom.php
        /// </summary>
        private const double Precision = 60000.0;

        /// <summary>
        ///     270度
        /// </summary>
        public static readonly Degree Degree270 = FromDouble(270.0);

        /// <summary>
        ///     90度
        /// </summary>
        public static readonly Degree Degree90 = FromDouble(90);

        /// <summary>
        ///     180度
        /// </summary>
        public static readonly Degree Degree180 = FromDouble(180);


        private int _intValue;

        /// <summary>
        ///     每个单位数值对应1/60000度
        /// </summary>
        /// <param name="value">范围 0-21600000,超过会被取模</param>
        public Degree(int value)
        {
            IntValue = value;
        }

        /// <summary>
        ///     openxml表示的度数int值
        ///     在0-21600000之间
        /// </summary>
        public int IntValue
        {
            get => _intValue;
            private set
            {
                var d = value % MaxDegreeValue;
                if (d < 0) d += MaxDegreeValue;

                _intValue = d;
            }
        }

        /// <summary>
        ///     openxml表示的度数的double值
        ///     0-360
        /// </summary>
        public double DoubleValue => IntValue / Precision;

        /// <summary>
        ///     openxml表示的度数的弧度值
        ///     0-2pi
        /// </summary>
        public double ToRadiansValue()
        {
            return DoubleValue / 180 * Math.PI;
        }


        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var p = (Degree)obj;
            return IntValue == p.IntValue;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return IntValue.GetHashCode();
        }

        /// <summary>
        ///     角度相加
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Degree operator +(Degree a)
        {
            return a;
        }

        /// <summary>
        ///     角度相减
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Degree operator -(Degree a)
        {
            return new Degree(-a.IntValue);
        }

        /// <summary>
        ///     角度相加
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Degree operator +(Degree a, Degree b)
        {
            return new Degree(a.IntValue + b.IntValue);
        }

        /// <summary>
        ///     角度相减
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Degree operator -(Degree a, Degree b)
        {
            return a + -b;
        }

        /// <summary>
        ///     角度相乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Degree operator *(Degree a, double b)
        {
            return new Degree((int)(a.IntValue * b));
        }

        /// <summary>
        ///     角度相乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Degree operator *(double a, Degree b)
        {
            return new Degree((int)(a * b.IntValue));
        }

        /// <summary>
        ///     角度相除
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Degree operator /(Degree a, double b)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (b == 0) throw new DivideByZeroException();

            return new Degree((int)(a.IntValue / b));
        }

        /// <summary>
        ///     判断角度大于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Degree a, Degree b)
        {
            return a.IntValue > b.IntValue;
        }

        /// <summary>
        ///     判断角度小于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(Degree a, Degree b)
        {
            return b > a;
        }

        /// <summary>
        ///     判断角度等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Degree a, Degree b)
        {
            return a.IntValue == b.IntValue;
        }

        /// <summary>
        ///     判断角度不等于
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Degree a, Degree b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     指定度数1单位数值代表1度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Degree FromDouble(double value)
        {
            var v = (int)(value * Precision);
            return new Degree(v);
        }
    }
}
