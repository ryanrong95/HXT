using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 重新生成的OrderItem，自动归类，有时候OrderItemTax里的value值为空
    /// 这时调用王辉的dll，传税费，这条orderitem的信息就不会过去
    /// 这时候客户端确认订单的时候，就会缺少这条信息
    /// 这里打个补丁，如果有OrderItemTax里value为空，则补充之后，重新调用王辉的dll
    /// </summary>
    public class SCReOrderBillHandler : SCHandler
    {
        public SCReOrderBillHandler(List<MatchViewModel> selectedItems, Order currentOrder)
        {
            SelectedItems = selectedItems;
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            try
            {
                // 只有新生成的OrderItem 才有这样的情况
                var newOrderItems = SelectedItems.Where(t => !string.IsNullOrEmpty(t.NewOrderItemID)).ToList();
                if (newOrderItems.Count() == 0)
                {
                    return;
                }

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    foreach (var item in newOrderItems)
                    {
                        var orderItemTaxes = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Where(t => t.OrderItemID == item.NewOrderItemID && t.Value == null).ToList();
                        //value 赋值为0就可以，只要过去，保证客户确认订单的时候能显示就行
                        // 后续如果价格不对，跟单会重新生成对账单
                        foreach (var tax in orderItemTaxes)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new { Value = 0M }, t => t.ID == tax.ID);                           
                        }
                    }

                    reponsitory.Submit();
                }

                Task.Run(()=>{
                    var Order = new Views.OrdersView().Where(t => t.ID == CurrentOrder.ID).FirstOrDefault();
                    Order.GenerateBill();
                });
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }
    }
}
