using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 员工的审批日志
    /// </summary>
    public class Logs_StaffApproval : IUnique
    {
        #region 属性

        string id;

        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.StaffID, this.ApprovalStep.ToString()).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public StaffApprovalStep ApprovalStep { get; set; }

        /// <summary>
        /// 应聘审批状态
        /// </summary>
        public StaffApprovalStatus ApprovalStatus { get; set; }

        /// <summary>
        /// 通知入职状态
        /// </summary>
        public StaffNoticeReportStatus NoticeReportStatus { get; set; }

        /// <summary>
        /// 入职报到状态
        /// </summary>
        public StaffEntryReportStatus EntryReportStatus { get; set; }

        /// <summary>
        /// 内容上下文
        /// </summary>
        public string Context { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        public Staff Staff { get; set; }

        public Admin Admin { get; set; }

        public string StatusDec
        {
            get
            {
                if (ApprovalStep == StaffApprovalStep.Notice)
                {
                    return NoticeReportStatus.GetDescription();
                }
                else if (ApprovalStep == StaffApprovalStep.Entry)
                {
                    return EntryReportStatus.GetDescription();
                }
                else
                {
                    return ApprovalStatus.GetDescription();
                }
            }
        }

        public Logs_StaffApprovalContext Logs_StaffApprovalContext
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.Context) ? new Logs_StaffApprovalContext() : this.Context.JsonTo<Logs_StaffApprovalContext>();
            }
        }

        #endregion

        public Logs_StaffApproval()
        {
            this.ApprovalStatus = StaffApprovalStatus.Waiting;
            this.NoticeReportStatus = StaffNoticeReportStatus.UnNotified;
            this.EntryReportStatus = StaffEntryReportStatus.WaitingReport;
        }

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbErm.Logs_StaffApprovals>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbErm.Logs_StaffApprovals()
                    {
                        ID = this.ID,
                        StaffID = this.StaffID,
                        AdminID = this.AdminID,
                        ApprovalStep = (int)this.ApprovalStep,
                        ApprovalStatus = (int)this.ApprovalStatus,
                        NoticeReportStatus = (int)this.NoticeReportStatus,
                        EnterReportStatus = (int)this.EntryReportStatus,
                        Context = this.Context,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.Logs_StaffApprovals>(new
                    {
                        StaffID = this.StaffID,
                        AdminID = this.AdminID,
                        ApprovalStep = (int)this.ApprovalStep,
                        ApprovalStatus = (int)this.ApprovalStatus,
                        NoticeReportStatus = (int)this.NoticeReportStatus,
                        EnterReportStatus = (int)this.EntryReportStatus,
                        Context = this.Context,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                    }, t => t.ID == this.ID);
                }
            }
        }

        #endregion
    }

    public class Logs_StaffApprovalContext
    {
        /// <summary>
        /// 报到日期
        /// </summary>
        public string ReportDate { get; set; }

        /// <summary>
        /// 摘要日志
        /// </summary>
        public string Summary { get; set; }
    }
}
