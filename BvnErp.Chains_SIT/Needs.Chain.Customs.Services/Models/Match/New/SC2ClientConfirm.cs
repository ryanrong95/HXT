using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户确认
    /// 只是因为分批到货，先报关一部分，其他都没有改变，没有需要重新归类的产品，没有数量变更，没有订单变更，直接发送客户确认
    /// </summary>
    public class SC2ClientConfirm : SCHandler
    {
        public SC2ClientConfirm(List<MatchViewModel> selectedItems,  Order currentOrder)
        {
            SelectedItems = selectedItems;
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            var orderItems = SelectedItems.Where(t => string.IsNullOrEmpty(t.NewOrderItemID)).ToList();
            if (orderItems.Count() == 0)
            {
                MatchPost2ClientDirectConfirm directConfirm = new MatchPost2ClientDirectConfirm(CurrentOrder);
                directConfirm.Post();
            }
           
            if (next != null)
            {
                next.handleRequest();
            }
        }
    }
}
