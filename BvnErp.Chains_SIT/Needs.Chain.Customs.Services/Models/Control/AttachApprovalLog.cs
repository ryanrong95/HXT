using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 附加审批日志
    /// </summary>
    public class AttachApprovalLog : IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// OrderControlID
        /// </summary>
        public string OrderControlID { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 日志文字内容
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AttachApprovalLogs>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.AttachApprovalLogs
                    {
                        ID = this.ID,
                        OrderControlID = this.OrderControlID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AttachApprovalLogs>(new
                    {
                        ID = this.ID,
                        OrderControlID = this.OrderControlID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
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
