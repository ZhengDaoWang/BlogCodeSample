using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{
    internal class XiaoYeziAdapter : IShepherdDog
    {
        public IDog Dog { get; set; }

        public XiaoYeziAdapter(IDog dog)
        {
            Dog = dog;
        }

        public void Bark()
        {
            Dog.Bark();
        }

        public void Eat()
        {
            Dog.Eat();
        }

        public void Shepherd()
        {
            if (Dog is IShepherdDog shepherdDog)
            {
                shepherdDog.Shepherd();
            }
        }

        /// <summary>
        /// 牧鸡
        /// </summary>
        public void HerdingChicken()
        {
            Console.WriteLine($"{Dog.GetType().Name} 在牧鸡");
        }
    }
}
