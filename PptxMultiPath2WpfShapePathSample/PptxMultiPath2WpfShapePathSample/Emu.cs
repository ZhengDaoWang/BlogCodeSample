using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PptxMultiPath2WpfShapePathSample
{
    public readonly struct Emu
    {
        /// <summary>
        ///     创建 <see cref="Emu" /> 单位。
        /// </summary>
        /// <param name="value"><see cref="Emu" /> 单位数值。</param>
        public Emu(double value)
        {
            Value = value;
        }

        /// <summary>
        ///     获取 <see cref="Emu" /> 单位数值。
        /// </summary>
        public double Value { get; }

        /// <summary>
        ///     表示已初始化为零的 <see cref="Emu" /> 长度。
        /// </summary>
        public static readonly Emu Zero = new Emu(0);
    }
}
