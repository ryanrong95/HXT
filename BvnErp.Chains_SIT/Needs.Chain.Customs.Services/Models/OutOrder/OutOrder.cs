using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 销项
    /// 销售单
    /// 
    /// </summary>
    public abstract class OutOrder
    {
        
    }

    /// <summary>
    /// 销售单
    /// </summary>
    public class HTOutOrder : OutOrder
    {
        public HTOutOrder()
        {

        }

        public HTOutOrder(DecHead decHead)
        {
            //根据报关单完成的销售单
        }
    }

    /// <summary>
    /// 恒远销售单
    /// </summary>
    public class SZOutOrder : OutOrder
    {
        public SZOutOrder()
        {

        }

        public SZOutOrder(DecHead decHead)
        {
            //根据报关单完成的销售单
        }
    }
}
