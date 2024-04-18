using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceItemAmountCalc
    {
        public List<string> ordersID { get; set; }

        public InvoiceItemAmountCalc(List<string> OrderIDS)
        {
            this.ordersID = OrderIDS;
        }

        public List<InvoiceItemAmountHelp> AmountResult()
        {
            List<InvoiceItemAmountHelp> AmountHelper = new List<InvoiceItemAmountHelp>();
            GroupPremiumsHelp[] group_premiums;
            List<OrderItemCountHelper> linq_item;

            using (var orderItemView = new Needs.Ccs.Services.Views.OrderItemsView())
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                linq_item = (from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                             where ordersID.Contains(item.OrderID)
                             select new OrderItemCountHelper
                             {
                                 OrderItemID = item.ID,
                                 OrderID = item.OrderID
                             }).ToList();

                var linq_premiums = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                    where ordersID.Contains(item.OrderID) && item.Status == (int)Enums.Status.Normal
                                    select new
                                    {
                                        OrderID = item.OrderID,
                                        Type = (Enums.OrderPremiumType)item.Type,
                                        Count = item.Count,
                                        UnitPrice = item.UnitPrice,
                                        //Currency = item.Currency,
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

            foreach (var item in ordersID)
            {
                InvoiceItemAmountHelp helper = new InvoiceItemAmountHelp();
                helper.OrderID = item;
                helper.OrderItemCount = linq_item.Where(t => t.OrderID == item).Count();
                helper.AgencyFee = (group_premiums.SingleOrDefault(t => t.OrderID == item
                           && t.Type == OrderPremiumType.AgencyFee)?.TotalPrice ?? 0m) ;
                helper.MiscFees = (group_premiums.Where(t => t.OrderID == item
                            && t.Type != OrderPremiumType.AgencyFee
                            && t.Type != OrderPremiumType.InspectionFee)).Sum(g=>g.TotalPrice);

                AmountHelper.Add(helper);
            }

            return AmountHelper;
        }
    }

    public class OrderItemCountHelper
    {
        public string OrderItemID { get; set; }
        public string OrderID { get; set; }
    }

    public class InvoiceItemAmountHelp
    {
        public string OrderID { get; set; }
        public int OrderItemCount { get; set; }
        public decimal AgencyFee { get; set; }
        public decimal MiscFees { get; set; }
    }
}
