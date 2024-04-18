using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 转报关订单
    /// 报关日期-入库日期（不是转报关订单生成日期）超过5天，收取仓储费用
    /// </summary>
    public class ToDeclareStrategy : StockingStrategy
    {
        public string OrderID { get; set; }
        public ToDeclareStrategy(string OrderID)
        {
            this.OrderID = OrderID;
        }
        public override int CalculateDays()
        {
            DateTime dtTommorrow = DateTime.Now.AddDays(1);
            string[] orderids = this.OrderID.Split('-');
            string mainOrderID = orderids[0];

            var res = new PvWsOrderBaseOrderItemView().Where(t => t.TinyOrderID == this.OrderID).ToList();
            var storageIDs = res.Select(t => t.StorageID).ToList();

            var earliestStorage = new CgStoragesTopViewOrigin().Where(t => storageIDs.Contains(t.ID)).OrderBy(t => t.EnterDate).FirstOrDefault();
            if (earliestStorage != null)
            {
                DateTime dtStorage = earliestStorage.EnterDate;
                return (dtTommorrow - dtStorage.Date).Days;
            }

            return 0;
        }
    }
}
