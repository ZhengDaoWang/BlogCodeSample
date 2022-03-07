using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{
    internal class ShiYiAdapter : IHuskyDog
    {
        public IDog Dog { get; set; }

        public ShiYiAdapter(IDog dog)
        {
            Dog=dog;
        }

        /// <summary>
        /// 吠
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Bark()
        {
            Dog.Bark();
        }

        /// <summary>
        /// 拆家
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Demolition()
        {
            if (Dog is IHuskyDog huskyDog)
            {
                huskyDog.Demolition();
            }
        }

        /// <summary>
        /// 吃
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Eat()
        {
            Dog.Eat();
        }

        /// <summary>
        /// 卖萌
        /// </summary>
        public void ActingCute()
        {
            Console.WriteLine($"{Dog.GetType().Name} 卖萌啦！！");
        }
    }
}
