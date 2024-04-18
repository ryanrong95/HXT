using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public abstract class IOutGoodsAdd
    {
        public string OrderID { get; set; }
        public List<string> OrderItems { get; set; }
        public Client Client { get; set; }

        public Order order { get; set; }
        /// <summary>
        /// 判断这个客户的开票类型，如果是服务费发票，则不需要计入发出商品
        /// </summary>
        /// <returns></returns>
        public bool isNeedToOutGoods()
        {
            bool isTotal = false;
            if (this.OrderItems.Count() > 0)
            {
                if (this.Client != null)
                {
                    if(this.Client.Agreement.InvoiceType == Enums.InvoiceType.Full)
                        isTotal = true;
                }
                else
                {
                    var order = new Needs.Ccs.Services.Views.OrdersViewBase<Order>().Where(t => t.ID == this.OrderID).FirstOrDefault();
                    this.order = order;
                    if (order.ClientAgreement.InvoiceType == Enums.InvoiceType.Full)
                        isTotal = true;
                }                
            }
            return isTotal;
        }
        public abstract void addOutGoods();
       
    }
}
