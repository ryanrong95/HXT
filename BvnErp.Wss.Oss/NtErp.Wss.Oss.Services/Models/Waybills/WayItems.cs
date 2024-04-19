using Needs.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 运单项
    /// </summary>
    public class WayItems : BaseItems<WayItem>
    {
        internal WayItems()
        {
        }

        public WayItems(IEnumerable<WayItem> enums) : base(enums)
        {
        }

        internal WayItems(IEnumerable<WayItem> enums, ItemStart<WayItem> action) : base(enums, action)
        {
        }

        /// <summary>
        /// 添加运单项
        /// </summary>
        /// <param name="entity"></param>
        override public void Add(WayItem entity)
        {
            base.Add(new WayItem
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                WaybillID = entity.WaybillID,
                Weight = entity.Weight,
            });
        }

        #region 持久化
        public void Enter()
        {
            foreach (var item in this)
            {
                item.Enter();
            }
        }

        #endregion 
    }
}
