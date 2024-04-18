using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单管控
    /// </summary>
    abstract public class OrderControlBase : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// 管控类型
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        protected Admin Admin { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 订单管控项
        /// </summary>
        abstract public OrderControlItems Items { get; set; }

        #endregion

        /// <summary>
        /// 设置审核人
        /// </summary>
        /// <param name="admin"></param>
        public void SetAdmin(Admin admin)
        {
            Admin = admin;
        }

        /// <summary>
        /// 根据订单的管控信息判断是否取消订单挂起
        /// </summary>
        protected void CancelOrderHangUp()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var controlStepsView = new Views.OrderControlStepsView(reponsitory);
                var linq = from entity in controlStepsView
                           join control in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>() on entity.OrderControlID equals control.ID
                           where control.OrderID == this.Order.ID
                           orderby entity.CreateDate descending
                           group entity by new { entity.OrderControlID } into entities
                           select entities.FirstOrDefault();

                int count = linq.Where(step => step.ControlStatus != Enums.OrderControlStatus.Approved).Count();
                if (count == 0)
                {
                    //如果全部审批通过，则取消订单挂起
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = false }, item => item.ID == this.Order.ID);

                    //公司单需要修改订单状态到待报关
                    if (this.Order.Type != Enums.OrderType.Outside)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                        {
                            OrderStatus = (int)Enums.OrderStatus.QuoteConfirmed
                        }, o => o.ID == this.Order.ID);
                    }
                }
            }
        }
    }
}