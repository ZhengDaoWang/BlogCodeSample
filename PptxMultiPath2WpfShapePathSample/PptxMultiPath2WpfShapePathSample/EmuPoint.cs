using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PptxMultiPath2WpfShapePathSample
{
    public readonly struct EmuPoint
    {
        public EmuPoint(Emu x, Emu y)
        {
            X = x;
            Y = y;
        }

        public EmuPoint(double x, double y)
        {
            X = new Emu(x);
            Y = new Emu(y);
        }

        public Emu X { get; }
        public Emu Y { get; }
    }
}
