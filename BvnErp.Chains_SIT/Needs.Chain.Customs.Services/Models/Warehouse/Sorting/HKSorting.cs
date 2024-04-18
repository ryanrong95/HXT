using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 分拣结果、到货确认、库存
    /// </summary>
    public class HKSorting : Sorting
    {
        /// <summary>
        /// 库存
        /// </summary>
       
        public HKSorting()
        {
            base.WarehouseType = Enums.WarehouseType.HongKong;
        }
    }
}