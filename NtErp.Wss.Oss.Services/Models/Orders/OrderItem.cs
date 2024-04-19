using Needs.Linq;
using Needs.Utils.Converters;
using Newtonsoft.Json;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItem : IUnique, IPersist
    {
        public OrderItem()
        {
            this.Status = OrderItemStatus.Normal;
            this.CreateDate = UpdateDate = DateTime.Now;
        }

        #region 属性

        /// <summary>
        /// 考虑重新规范 (ServiceID)
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// ServiceID
        /// </summary>

        public string ServiceID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 重量 [标准单位：千克(kg)]
        /// </summary>
        public decimal? Weight { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public OrderItemFrom From { get; set; }
        /// <summary>
        /// 订单项状态
        /// </summary>
        public OrderItemStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string Leadtime { get; set; }

        public string Note { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 产品
        /// </summary>
        public StandardProduct Product { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public Company Supplier { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public decimal Total
        {
            get
            {
                return (this.Quantity * this.UnitPrice).Twoh();
            }
        }

        #endregion

        #region 持久化

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        internal void InEnter()
        {
            this.Supplier.Enter();
            this.Product.Enter();

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                }
                if (reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>().Count(t => t.ID == this.ID) == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

        }


        public void Enter()
        {
            Order.Refund(this.OrderID, this.InEnter);
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void InAbandon()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.CvOss.OrderItems>(new
                {
                    Status = this.Status = OrderItemStatus.Delete
                }, item => item.ID == this.ID);
            }
        }

        public void Abandon()
        {
            Order.Refund(this.OrderID, this.InAbandon);
            if (this != null && this.EnterSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
