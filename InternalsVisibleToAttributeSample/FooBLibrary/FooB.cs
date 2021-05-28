using System;
using System.Collections.Generic;
using System.Text;

namespace FooBLibrary
{
   public class FooB
    {
        public void PrintA()
        {
            new FooALibrary.FooA().Print();
        }
    }
}
