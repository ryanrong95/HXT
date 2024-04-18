
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关订单
    /// 如果代报关订单装箱后，没有立即报关，报关日期和装箱日期超过5天需要收取仓储费用(更新 这种情况不收取 2022-04-26)
    /// 跟单匹配暂存后，会生成报关订单，如果报关日期-暂存录入日期5天，需要收取仓储费用
    /// 代报关的订单，先去看一下有没有暂存，再看封箱有没有超过5天没有报关
    /// </summary>
    public class DeclareStrategy : StockingStrategy
    {
        public string OrderID { get; set; }
        public DeclareStrategy(string OrderID)
        {
            this.OrderID = OrderID;
        }
        public override int CalculateDays()
        {
            DateTime dtTommorrow = DateTime.Now.AddDays(1);
            string[] orderids = this.OrderID.Split('-');
            string mainOrderID = orderids[0];

            //var packing = new PackingsView().Where(t => t.OrderID == this.OrderID).FirstOrDefault();
            ////是否封箱
            //if (packing != null)
            //{
            //    //已封箱，看封箱日期
            //    if(packing.PackingStatus == Enums.PackingStatus.Sealed)
            //    {
            //        return (dtTommorrow - packing.CreateDate.Date).Days;
            //    }
            //    else
            //    {
            //        return 0;
            //    }

            //}
            //else
            //{
                //没有封箱，看是否跟单匹配过暂存
                var res = new PvWmsTWaybillsTopView().Where(t => t.OrderID == this.OrderID || t.OrderID == mainOrderID).OrderBy(t => t.CreateDate).ToList();
                if (res.Count > 0)
                {
                    var storage = res.FirstOrDefault();
                    return (dtTommorrow - storage.CreateDate.Date).Days;
                }
            //}

            return 0;            
        }
    }
}
