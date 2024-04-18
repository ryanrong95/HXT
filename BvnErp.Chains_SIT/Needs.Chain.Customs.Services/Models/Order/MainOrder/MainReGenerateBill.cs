using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MainReGenerateBill
    {
        public List<ReGenerateBill> OrderParams { get; set; }
        public string approveOnOff { get; set; }
        public List<OneTinyOrderInfo> tinyOrderInfos  { get; set; }
        public Admin admin { get; set; }

        public MainReGenerateBill(List<ReGenerateBill> orderParams, Admin current)
        {
            this.OrderParams = orderParams;
            this.admin = current;
            tinyOrderInfos = new List<OneTinyOrderInfo>();
        }

        public void ReGenerate()
        {
            GroupPremiumsHelp[] group_premiums;
            var modelOrderIDs = OrderParams.Select(t => t.OrderID).ToList();

            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var linq_premiums = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                    where modelOrderIDs.Contains(item.OrderID)
                                    select new
                                    {
                                        OrderID = item.OrderID,
                                        Type = (Enums.OrderPremiumType)item.Type,
                                        Count = item.Count,
                                        UnitPrice = item.UnitPrice,                                      
                                        Rate = item.Rate,
                                    };
                group_premiums = (from item in linq_premiums.ToArray()
                                  group item by new { item.OrderID, item.Type } into groups
                                  select new GroupPremiumsHelp
                                  {
                                      OrderID = groups.Key.OrderID,
                                      Type = groups.Key.Type,
                                      TotalPrice = groups.Sum(item => item.Count * item.UnitPrice * item.Rate)
                                  }).ToArray();

               
            }
        }
    }


    /// <summary>
    /// 我的订单项附加费用帮助类
    /// </summary>
    class GroupPremiumsHelp
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 附加费用类型
        /// </summary>
        public Enums.OrderPremiumType Type { get; set; }

        /// <summary>
        /// 总值
        /// </summary>
        public decimal TotalPrice { get; set; }
    }

}
