using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 申请的审批步骤
    /// </summary>
    public class ApplyVoteStep : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public string VoteStepID { get; set; }

        /// <summary>
        /// 是否是当前审批步骤
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 审批的状态
        /// </summary>
        public ApprovalStatus Status { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Summary { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 审批人
        /// </summary>
        public Admin Admin { get; set; }
        /// <summary>
        /// 审批步骤
        /// </summary>
        public VoteStep VoteStep { get; set; }
        #endregion

        #region 构造器

        public ApplyVoteStep()
        {
            ApplyAgreed += ApplyVoteStep_Processed;
            ApplyRejected += ApplyVoteStep_Processed;
        }

        #endregion

        #region 事件

        public event SuccessHanlder ApplyAgreed;

        public event SuccessHanlder ApplyRejected;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbErm.ApplyVoteSteps()
                    {
                        ID = this.ID,
                        ApplicationID = this.ApplicationID,
                        VoteStepID = this.VoteStepID,
                        IsCurrent = this.IsCurrent,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        ApplicationID = this.ApplicationID,
                        VoteStepID = this.VoteStepID,
                        IsCurrent = this.IsCurrent,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        ModifyDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }
            }
        }

        internal bool Next()
        {
            bool isCompleted = false;

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                //更新当前Step的IsCurrent为false
                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                {
                    IsCurrent = false,
                    Status = (int)ApprovalStatus.Agree,
                }, item => item.ID == this.ID);

                var voteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.ID == this.VoteStepID);
                var nextVoteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>()
                    .FirstOrDefault(item => item.VoteFlowID == voteStep.VoteFlowID && item.OrderIndex > voteStep.OrderIndex);

                if (nextVoteStep == null)
                {
                    isCompleted = true;
                }
                else
                {
                    //更新下一个Step的IsCurrent为true
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true
                    }, item => item.ApplicationID == this.ApplicationID && item.VoteStepID == nextVoteStep.ID);
                }

                if (this != null && this.ApplyAgreed != null)
                {
                    this.ApplyAgreed(this, new SuccessEventArgs(ApprovalStatus.Agree));
                }
            }

            return isCompleted;
        }

        /// <summary>
        /// 审批否决
        /// </summary>
        internal void Reject()
        {
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                var voteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.ID == this.VoteStepID);
                if (voteStep.OrderIndex > 1)
                {
                    //如果当前不是第一步审批，将第一步审批的IsCurrent改为true，每步审批的状态改回等待审批
                    var firstVoteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.VoteFlowID == voteStep.VoteFlowID && item.OrderIndex == 1);
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true,
                        Status = (int)ApprovalStatus.Waiting
                    }, item => item.ApplicationID == this.ApplicationID && item.VoteStepID == firstVoteStep.ID);
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = false,
                        Status = (int)ApprovalStatus.Waiting
                    }, item => item.ApplicationID == this.ApplicationID && item.VoteStepID != firstVoteStep.ID);
                }
                else
                {
                    //如果当前是第一步审批，将状态改回等待审批
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true,
                        Status = (int)ApprovalStatus.Waiting
                    }, item => item.ID == this.ID);
                }

                if (this != null && this.ApplyRejected != null)
                {
                    this.ApplyRejected(this, new SuccessEventArgs(ApprovalStatus.Reject));
                }
            }
        }

        /// <summary>
        /// 审批处理完成，记录日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyVoteStep_Processed(object sender, SuccessEventArgs e)
        {
            var applyVoteStep = (ApplyVoteStep)sender;
            var status = (ApprovalStatus)e.Object;

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps()
                {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.ApplyVoteStepLog),
                    ApplicationID = applyVoteStep.ApplicationID,
                    VoteStepID = applyVoteStep.VoteStepID,
                    AdminID = applyVoteStep.AdminID,
                    Status = (int)status,
                    Summary = this.Summary,
                    CreateDate = DateTime.Now
                });
            }
        }

        #endregion
    }
}
