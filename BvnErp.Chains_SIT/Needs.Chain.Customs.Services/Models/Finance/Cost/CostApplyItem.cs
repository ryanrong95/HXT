using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class CostApplyItem : IUnique, IPersistence, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        public string CostApplyID { get; set; }
        public Enums.FinanceFeeType FeeType { get; set; }
        public decimal Amount { get; set; }
        public string FeeDesc { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public int? DyjID { get; set; }

        public int? DyjFeeType { get; set; }

        public string EmployeeID { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public CostApplyItem()
        {
            this.Status = Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {

        }

        virtual protected void OnEnterSuccess()
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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
