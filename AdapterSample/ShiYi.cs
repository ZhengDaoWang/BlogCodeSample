using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{
    /// <summary>
    /// 十一
    /// </summary>
    internal class ShiYi : IHuskyDog
    {
        public void Bark()
        {
            Console.WriteLine($"I Am {nameof(ShiYi)} 汪 汪 汪 !!!!!");
        }

        public void Demolition()
        {
            Console.WriteLine($"I Am {nameof(ShiYi)} ,拆家啦 !!!!!");
        }

        public void Eat()
        {
            Console.WriteLine($"I Am {nameof(ShiYi)} 好吃得停不下来 !!!!!");
        }
    }
}
