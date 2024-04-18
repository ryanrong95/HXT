using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 抵用券
    /// </summary>
    public class FinanceVoucher : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 数据库字段

        /// <summary>
        /// 抵用券ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 面值
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 应用到的 OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 使用人ID
        /// </summary>
        public string UseAdminID { get; set; }

        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? UseTime { get; set; }

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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVouchers>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinanceVouchers>(new Layer.Data.Sqls.ScCustoms.FinanceVouchers
                    {
                        ID = this.ID,
                        Amount = this.Amount,
                        OrderID = this.OrderID,
                        UseAdminID = this.UseAdminID,
                        UseTime = this.UseTime,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceVouchers>(new
                    {
                        Amount = this.Amount,
                        OrderID = this.OrderID,
                        UseAdminID = this.UseAdminID,
                        UseTime = this.UseTime,
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceVouchers>(
                    new
                    {
                        Status = Enums.Status.Delete
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

    }
}
