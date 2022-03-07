using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{
    /// <summary>
    /// 小叶子、小椰汁
    /// </summary>
    internal class XiaoYezi : IShepherdDog
    {
        public void Bark()
        {
            Console.WriteLine($"I Am {nameof(XiaoYezi)} 汪 汪 汪 !!!!!");
        }

        public void Eat()
        {
            Console.WriteLine($"I Am {nameof(XiaoYezi)} 好吃 !!!!!");
        }

        /// <summary>
        /// 牧羊
        /// </summary>
        public void Shepherd()
        {
            Console.WriteLine($"I Am {nameof(XiaoYezi)}， 我在牧羊 !!!!!");
        }
    }
}
