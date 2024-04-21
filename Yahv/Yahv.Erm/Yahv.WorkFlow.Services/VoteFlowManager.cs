using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Common;
using Yahv.WorkFlow.Services.Extends;

namespace Yahv.WorkFlow.Services
{
    /// <summary>
    /// 审批流管理
    /// </summary>
    public class VoteFlowManager
    {
        IEnumerable<VoteFlow> voteFlows;
        Models.Origins.Application application;
        Models.Origins.ApplyVoteStep applyVoteStep;

        #region 索引器

        /// <summary>
        /// 获取审批流
        /// </summary>
        /// <param name="type">申请审批的类型</param>
        /// <param name="days">请假天数(请假申请时填写)</param>
        /// <returns></returns>
        public VoteFlow this[VoteFlowType type, float? days = null]
        {
            get
            {
                if (days == null)
                    return this.voteFlows.Single(item => item.Type == type);

                return this.voteFlows.Single(item => item.Type == type && item.LowerLimit < days && (item.UpperLimit == null || item.UpperLimit >= days));
            }
        }

        #endregion

        #region 构造器

        public VoteFlowManager For(Models.Origins.Application application)
        {
            this.application = application;
            return this;
        }

        public VoteFlowManager For(Models.Origins.ApplyVoteStep applyVoteStep)
        {
            this.applyVoteStep = applyVoteStep;
            return this;
        }

        VoteFlowManager()
        {
            var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "voteflows.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(json))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JArray jArray = (JArray)JToken.ReadFrom(reader);
                    this.voteFlows = jArray.ToObject<List<VoteFlow>>();
                }
            }
        }

        #endregion

        private static readonly object locker = new object();
        private static VoteFlowManager instance;
        public static VoteFlowManager Current
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        lock (locker)
                        {
                            instance = new VoteFlowManager();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 创建申请审批步骤
        /// </summary>
        internal void InitSteps()
        {
            if (application == null || application.ID == null)
                throw new Exception($"{nameof(Models.Origins.Application)}对象不能为空!");

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                //删除已经生成的申请审批步骤
                reponsitory.Delete<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(item => item.ApplicationID == application.ID);

                //获取审批流的审批步骤
                var voteSteps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>()
                    .Where(item => item.VoteFlowID == application.VoteFlowID).OrderBy(item => item.OrderIndex).ToArray();
                var ids = Underly.PKeyType.ApplyVoteStep.Pick(voteSteps.Length);

                //根据芯达通组织架构获取部门负责人
                var staff = XDTOrganization.StaffList.Single(item => item.AdminID == application.ApplicantID);
                var manager = XDTOrganization.StaffList.Single(item => item.DepartmentType == staff.DepartmentType && item.PostType != PostType.Staff);

                //生成申请审批步骤
                var applyVoteSteps = voteSteps.Select((item, index) => new Layers.Data.Sqls.PvbErm.ApplyVoteSteps()
                {
                    ID = ids[index],
                    ApplicationID = application.ID,
                    VoteStepID = item.ID,
                    IsCurrent = item.OrderIndex == 1 ? true : false,
                    AdminID = item.OrderIndex == 1 ? manager.AdminID : item.AdminID,
                    Status = (int)ApplyVoteStepStatus.ApprovalPending,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                }).ToArray();

                reponsitory.Insert(applyVoteSteps);
            }
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <returns></returns>
        internal bool Next()
        {
            if (applyVoteStep == null || applyVoteStep.ID == null)
                throw new Exception($"{nameof(Models.Origins.ApplyVoteStep)}对象不能为空!");

            bool isCompleted = false;

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                //更新当前Step的IsCurrent为false
                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                {
                    IsCurrent = false,
                    Status = (int)ApplyVoteStepStatus.Allow,
                }, item => item.ID == applyVoteStep.ID);

                var voteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.ID == applyVoteStep.VoteStepID);
                var nextVoteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>()
                    .SingleOrDefault(item => item.VoteFlowID == voteStep.VoteFlowID && item.OrderIndex > voteStep.OrderIndex);

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
                    }, item => item.ApplicationID == applyVoteStep.ApplicationID && item.VoteStepID == nextVoteStep.ID);
                }
            }

            return isCompleted;
        }

        /// <summary>
        /// 审批否决
        /// </summary>
        internal void Reject()
        {
            if (applyVoteStep == null || applyVoteStep.ID == null)
                throw new Exception($"{nameof(Models.Origins.ApplyVoteStep)}对象不能为空!");

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                var voteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.ID == applyVoteStep.VoteStepID);
                if (voteStep.OrderIndex > 1)
                {
                    //如果当前不是第一步审批，将第一步审批的IsCurrent改为true，每步审批的状态改回等待审批
                    var firstVoteStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Single(item => item.VoteFlowID == voteStep.VoteFlowID && item.OrderIndex == 1);
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true,
                        Status = (int)ApplyVoteStepStatus.ApprovalPending
                    }, item => item.ApplicationID == applyVoteStep.ApplicationID && item.VoteStepID == firstVoteStep.ID);
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = false,
                        Status = (int)ApplyVoteStepStatus.ApprovalPending
                    }, item => item.ApplicationID == applyVoteStep.ApplicationID && item.VoteStepID != firstVoteStep.ID);
                }
                else
                {
                    //如果当前是第一步审批，将状态改回等待审批
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true,
                        Status = (int)ApplyVoteStepStatus.ApprovalPending
                    }, item => item.ID == applyVoteStep.ID);
                }
            }
        }
    }
}
