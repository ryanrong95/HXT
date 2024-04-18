using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExpireOrder : IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpireDate { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        public int ExpiredDays
        {
            get
            {
                if (this.ExpireDate != null)
                {
                    TimeSpan ts = DateTime.Now - this.ExpireDate;
                    return ts.Days;
                }
                else
                {
                    return 0;
                } 
            }
        }
        public Admin Admin { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public ExpireOrder()
        {
            this.Status = Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExpireOrders
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        OrderID = this.OrderID,
                        Amount = this.Amount,
                        ExpireDate = this.ExpireDate,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
