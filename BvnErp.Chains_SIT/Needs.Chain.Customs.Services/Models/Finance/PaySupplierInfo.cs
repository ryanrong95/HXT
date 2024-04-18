using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付汇供应商信息
    /// </summary>
    public class PaySupplierInfo : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 数据库字段

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 信息类型（应付/实付）
        /// </summary>
        public Enums.PaySupplierPayType PayType { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>(new Layer.Data.Sqls.ScCustoms.PaySupplierInfos
                    {
                        ID = this.ID,
                        DecHeadID = this.DecHeadID,
                        OrderID = this.OrderID,
                        ClientID = this.ClientID,
                        Amount= this.Amount,
                        Currency = this.Currency,
                        SupplierName = this.SupplierName,
                        PayType = (int)this.PayType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>(new
                    {
                        DecHeadID = this.DecHeadID,
                        OrderID = this.OrderID,
                        ClientID = this.ClientID,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        SupplierName = this.SupplierName,
                        PayType = (int)this.PayType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
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

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>(new
                {
                    Status = (int)Enums.Status.Delete,
                }, item => item.ID == this.ID);
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

        public static void AbandonByIDs(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string[] ids)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>(new
            {
                Status = (int)Enums.Status.Delete,
            }, item => ids.Contains(item.ID));
        }

    }
}
