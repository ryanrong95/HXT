using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FinancePaymentOperator : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 数据库字段

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Enums.PaymentOperatorType Type { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Summary
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>(new Layer.Data.Sqls.ScCustoms.FinancePaymentOperators
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>(new
                    {
                        AdminID = this.AdminID,
                        Type = (int)this.Type,
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>(
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
