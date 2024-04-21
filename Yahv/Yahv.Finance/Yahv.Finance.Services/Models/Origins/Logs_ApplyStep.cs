using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 审批日志
    /// </summary>
    public class Logs_ApplyStep : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public ApplyType Type { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproverID { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审批结果
        /// </summary>
        public ApprovalStatus Status { get; set; }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 审批人
        /// </summary>
        public Admin Approver { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Logs_ApplyStep()
                {
                    CreateDate = DateTime.Now,
                    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.LogsApply),
                    Status = (int)this.Status,
                    Type = (int)this.Type,
                    ApplyID = this.ApplyID,
                    Summary = this.Summary,
                    ApproverID = this.ApproverID,
                });
            }
        }
        #endregion
    }
}