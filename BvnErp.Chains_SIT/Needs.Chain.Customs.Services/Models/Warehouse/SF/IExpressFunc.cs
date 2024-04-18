using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public interface IExpressFunc
    {
        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        string RequestOrder();

    }
}
