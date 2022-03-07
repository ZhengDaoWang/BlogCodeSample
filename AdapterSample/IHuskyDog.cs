using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterSample
{
    /// <summary>
    /// 哈士奇
    /// </summary>
    internal interface IHuskyDog:IDog
    {
        /// <summary>
        /// 破坏、拆家
        /// </summary>
        public void Demolition();
    }
}
