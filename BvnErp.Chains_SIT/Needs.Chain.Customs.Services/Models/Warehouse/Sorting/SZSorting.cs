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
    [Serializable]
    public class SZSorting : Sorting
    {
        public SZSorting()
        {
            base.WarehouseType = Enums.WarehouseType.ShenZhen;
            base.DecStatus = Enums.SortingDecStatus.Yes;
        }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 报关单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 已送货数量
        /// </summary>
        public decimal DeliveriedQuantity { get; set; }
        //{
        //    get
        //    {
        //        using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
        //        {
        //            var exitNoticeItems = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
        //                                  where item.SortingID == this.ID && item.Status == (int)Enums.Status.Normal
        //                                  select item;
        //            if (exitNoticeItems.Count() == 0)
        //            {
        //                return 0;
        //            }
        //            else
        //            {
        //                return exitNoticeItems.Select(item => item.Quantity).Sum();
        //            }
        //        }
        //    }
        //}

        public string DateBoxIndex { get; set; }
    }
}