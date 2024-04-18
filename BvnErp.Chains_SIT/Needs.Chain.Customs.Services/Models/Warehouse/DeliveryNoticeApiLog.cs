using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 香港库房ApiLog
    /// </summary>
    public class DeliveryNoticeApiLog : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// BatchID
        /// </summary>
        public string BatchID { get; set; } = string.Empty;

        /// <summary>
        /// 大订单 ID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 小订单 ID
        /// </summary>
        public string TinyOrderID { get; set; } = string.Empty;

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 请求内容
        /// </summary>
        public string RequestContent { get; set; } = string.Empty;

        /// <summary>
        /// 返回内容
        /// </summary>
        public string ResponseContent { get; set; } = string.Empty;

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

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNoticeApiLogs>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DeliveryNoticeApiLogs()
                    {
                        ID = this.ID,
                        BatchID = this.BatchID,
                        OrderID = this.OrderID,
                        TinyOrderID = this.TinyOrderID,
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeliveryNoticeApiLogs>(new
                    {
                        OrderID = this.OrderID,
                        BatchID = this.BatchID,
                        TinyOrderID = this.TinyOrderID,
                        Url = this.Url,
                        RequestContent = this.RequestContent,
                        ResponseContent = this.ResponseContent,
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

    }
}
