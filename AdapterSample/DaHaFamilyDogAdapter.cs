using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{

    /// <summary>
    /// 哈仔十一家的狗适配器
    /// </summary>
    internal class DaHaFamilyDogAdapter:IDog
    {
        public IDog Dog { get; set; }

        public DaHaFamilyDogAdapter(IDog dog)
        {
            Dog = dog;
        }

        /// <summary>
        /// 牧鸡
        /// </summary>
        public void HerdingChicken()
        {
            Console.WriteLine($"{Dog.GetType().Name} 在牧鸡");
        }

        /// <summary>
        /// 卖萌
        /// </summary>
        public void ActingCute()
        {
            Console.WriteLine($"{Dog.GetType().Name} 在卖萌");
        }

        public void Eat()
        {
            Dog.Eat();
        }

        public void Bark()
        {
            Dog.Bark();
        }
    }
}
