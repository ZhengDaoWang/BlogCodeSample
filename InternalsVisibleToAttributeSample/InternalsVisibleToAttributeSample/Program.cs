using System;

namespace InternalsVisibleToAttributeSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var fooA = new FooALibrary.FooA();
            fooA.Print();
        }
    }
}
