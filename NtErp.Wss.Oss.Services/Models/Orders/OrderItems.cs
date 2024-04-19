using Needs.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    public class OrderItems : BaseItems<OrderItem>
    {
        internal OrderItems()
        {
        }

        public OrderItems(IEnumerable<OrderItem> enums) : base(enums)
        {

        }

        internal OrderItems(IEnumerable<OrderItem> enums, ItemStart<OrderItem> action) : base(enums, action)
        {
        }

        public decimal Total
        {
            get
            {
                return this.Where(t => t.Status == OrderItemStatus.Normal).Sum(t => t.Total);
            }
        }

        public OrderItem this[string index]
        {
            get
            {
                return this.SingleOrDefault(t => t.ID == index);
            }
        }

        /// <summary>
        /// 添加产品项
        /// </summary>
        /// <param name="item"></param>
        override public void Add(OrderItem item)
        {
            base.Add(item);

        }

    }
}
