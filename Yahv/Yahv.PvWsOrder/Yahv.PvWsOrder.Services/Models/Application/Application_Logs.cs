using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 收付款申请日志
    /// </summary>
    public class Application_Logs : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ApplicationID { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public ApprovalStatus Status { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public Application_Logs()
        {
            this.Status = ApprovalStatus.Waiting;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Application_Logs
                {
                    ID = this.ID,
                    ApplicationID = this.ApplicationID,
                    StepName = this.StepName,
                    Status = (int)this.Status,
                    AdminID = this.AdminID,
                    CreateDate = DateTime.Now,
                    Summary = this.Summary,
                });
            };
        }
    }
}
