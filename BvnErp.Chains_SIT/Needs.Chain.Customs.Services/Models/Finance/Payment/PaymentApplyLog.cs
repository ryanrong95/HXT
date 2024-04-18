using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请
    /// </summary>
    public class PaymentApplyLog : IUnique, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        public string PaymentApplyID { get; set; }
        
        public Admin Admin { get; set; }

        public Enums.PaymentApplyStatus PaymentApplyStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        //事件
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;   

        public PaymentApplyLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentApplyLog);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PaymentApplyLogs
                    {
                        ID = this.ID,
                        PaymentApplyID = this.PaymentApplyID,
                        AdminID = this.Admin.ID,
                        PaymentApplyStatus = (int)this.PaymentApplyStatus,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
    }
}
