using Needs.Linq;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 执行计划
    /// </summary>
    public delegate void Entering();

    /// <summary>
    /// 附加价值
    /// </summary>
    public class Premium : IUnique, IPersist
    {
        public Premium()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性

        public string ID { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }
        /// <summary>
        /// 名目
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 描述说明
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 总额
        /// </summary>
        public decimal Total
        {
            get
            {
                return this.Count * this.Price;
            }
        }

        #endregion

        #region 持久化

        public void InEnter()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Premium);
                    reponsitory.Insert(this.ToLinq());
                }
            }
        }

        public void Enter()
        {
            Order.Refund(this.OrderID, this.InEnter);
        }

        #endregion



    }
}