using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// CostApply 在 Api 运行中的日志
    /// </summary>
    public class CostApplyApiLog : IUnique, IFulError, IFulSuccess
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string BatchID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string PaymentNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string RequestContent { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ResponseContent { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyApiLogs>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyApiLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyApiLogs
                    {
                        ID = this.ID,
                        BatchID = this.BatchID,
                        CostApplyID = this.CostApplyID,
                        PaymentNoticeID = this.PaymentNoticeID,
                        Url = this.Url,
                        RequestContent = this.RequestContent,
                        ResponseContent = this.ResponseContent,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplyApiLogs>(new
                    {
                        BatchID = this.BatchID,
                        CostApplyID = this.CostApplyID,
                        PaymentNoticeID = this.PaymentNoticeID,
                        Url = this.Url,
                        RequestContent = this.RequestContent,
                        ResponseContent = this.ResponseContent,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
                this.OnEnterSuccess();
            }
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
