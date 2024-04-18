using Needs.Linq;
using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 代理订单日志
    /// </summary>
    [Serializable]
    public class OrderLog : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public string UserID { get; set; }

        public Enums.OrderStatus OrderStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public OrderLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //主键ID（OrderLog +8位年月日+10位流水号）
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderLog);
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderLogs
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    AdminID = this.AdminID,
                    UserID = this.UserID,
                    OrderStatus = (int)this.OrderStatus,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}