using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 费用申请中的收款人账户
    /// </summary>
    public class CostApplyPayee : IUnique, IPersistence, IFulError, IFulSuccess
    {
        public string ID { get; set; } = string.Empty;

        public string AdminID { get; set; } = string.Empty;

        public string PayeeName { get; set; } = string.Empty;

        public string PayeeAccount { get; set; } = string.Empty;

        public string PayeeBank { get; set; } = string.Empty;

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; } = string.Empty;

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyPayees>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = (int)Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyPayees>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyPayees>(new Layer.Data.Sqls.ScCustoms.CostApplyPayees
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        PayeeName = this.PayeeName,
                        PayeeAccount = this.PayeeAccount,
                        PayeeBank = this.PayeeBank,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyPayees>(new
                    {
                        AdminID = this.AdminID,
                        PayeeName = this.PayeeName,
                        PayeeAccount = this.PayeeAccount,
                        PayeeBank = this.PayeeBank,
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
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
