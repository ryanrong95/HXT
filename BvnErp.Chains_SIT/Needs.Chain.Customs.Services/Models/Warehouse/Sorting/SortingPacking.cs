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
    /// 分拣装箱信息
    /// 到货信息
    /// 到货的装箱信息
    /// </summary>
    public class SortingPacking: Sorting
    {
        public Packing Packing { get; set; }

        public Enums.ItemCategoryType ItemCategoryType { get; set; }
    }
}