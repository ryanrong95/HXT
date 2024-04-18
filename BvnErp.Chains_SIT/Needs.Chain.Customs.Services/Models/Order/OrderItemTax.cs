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
    /// 代理订单税率
    /// </summary>
    public class OrderItemTax : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                //主键ID（OrderItemID+Type）.MD5
                return this.id ?? string.Concat(this.OrderItemID, this.Type).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 税率类型：进口关税、出口关税、增值税、消费税
        /// </summary>
        public Enums.CustomsRateType Type { get; set; }

        /// <summary>
        /// 税率%
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 实收税率%
        /// </summary>
        public decimal ReceiptRate { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal? Value { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 产地附加税率
        /// </summary>
        public decimal OriginRate { get; set; }

        #endregion
        public OrderItemTax()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
