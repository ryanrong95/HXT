using Layers.Data.Sqls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 管理要求
    /// </summary>
    public class ForVoteFlow
    {
        Application application;

        /// <summary>
        /// 构造器
        /// </summary>
        internal ForVoteFlow(Application application)
        {
            this.application = application;
        }

        #region 生成申请的审批步骤

        /// <summary>
        /// 生成申请审批步骤
        /// </summary>
        public void Init()
        {
            using (var reponsitory = new PvbErmReponsitory())
            {
                //删除已经生成的申请审批步骤
                reponsitory.Delete<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(item => item.ApplicationID == this.application.ID);

                //获取审批流的审批步骤
                var voteSteps = VoteFlowManager.Interior.Single(item => item.ID == this.application.VoteFlowID).Steps;
                var ids = Underly.PKeyType.ApplyVoteStep.Pick(voteSteps.Length);

                //设置部门负责人ID
                var approveID = string.Empty;
                switch (this.application.ApplicationType)
                {
                    case ApplicationType.Leave:
                        approveID = this.application.Resignation.DeptLeaderID;
                        break;
                    case ApplicationType.Offtime:
                        approveID = this.application.OffTimeContext.ApproveID;
                        break;
                    case ApplicationType.Overtime:
                        approveID = this.application.OverTimeContext.ApproveID;
                        break;
                    case ApplicationType.WorkCard:
                        approveID = this.application.WorkCardContext.ApproveID;
                        break;
                    case ApplicationType.InternalTraining:
                        approveID = this.application.InternalTrainingContext.ApproveID;
                        break;
                    case ApplicationType.ArchiveBorrow:
                        approveID = this.application.ArchiveBorrowContext.ApproveID;
                        break;
                    case ApplicationType.ArchiveLending:
                        approveID = this.application.ArchiveLendingContext.ApproveID;
                        break;
                    case ApplicationType.ArchiveDestroy: 
                        approveID = this.application.ArchiveDestroyContext.AuditManager;
                        break;
                }

                //生成申请审批步骤
                var applyVoteSteps = voteSteps.Select((item, index) => new Layers.Data.Sqls.PvbErm.ApplyVoteSteps()
                {
                    ID = ids[index],
                    ApplicationID = this.application.ID,
                    VoteStepID = item.ID,
                    IsCurrent = item.OrderIndex == 1 ? true : false,
                    AdminID = item.OrderIndex == 1 && !string.IsNullOrEmpty(approveID) ? approveID : item.AdminID,
                    Status = (int)ApprovalStatus.Waiting,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                }).ToArray();

                reponsitory.Insert(applyVoteSteps);
            }
        }

        #endregion

        #region 审批通过

        /// <summary>
        /// 更新申请审批步骤的状态
        /// </summary>
        /// <param name="summary">审批意见</param>
        /// <param name="allow">
        /// 真：allow
        /// 假：Veto
        /// </param>
        /// <returns>是否为最后一步</returns>
        public bool Next(string summary, bool allow = true)
        {
            bool isLastStep = false;

            using (var reponsitory = new PvbErmReponsitory())
            {
                //申请步骤
                var currentStep = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>()
                                             .Single(item => item.ApplicationID == application.ID && item.IsCurrent);

                //审批步骤
                var voteFlow = VoteFlowManager.Interior.Single(item => item.ID == this.application.VoteFlowID);
                var currentVoteStep = voteFlow.Steps.Single(item => item.ID == currentStep.VoteStepID);
                var nextVoteStep = voteFlow.Steps.FirstOrDefault(item => item.OrderIndex == (currentVoteStep.OrderIndex + 1));

                //如果没有下一步审批，则当前步骤为最后一步
                if (nextVoteStep == null)
                {
                    isLastStep = true;
                }

                //审批通过
                if (allow)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = false,
                        Status = (int)ApprovalStatus.Agree,
                        //审批意见
                        Summary = summary,
                        ModifyDate = DateTime.Now
                    }, item => item.ID == currentStep.ID);

                    if (nextVoteStep != null)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                        {
                            IsCurrent = true,
                            ModifyDate = DateTime.Now
                        }, item => item.ApplicationID == this.application.ID && item.VoteStepID == nextVoteStep.ID);
                    }
                }
                //审批否决
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = false,
                        Status = (int)ApprovalStatus.Waiting,
                        ModifyDate = DateTime.Now
                    }, item => item.ApplicationID == this.application.ID);

                    var firstVoteStep = voteFlow.Steps.Single(item => item.OrderIndex == 1);
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        IsCurrent = true,
                        Status = (int)ApprovalStatus.Waiting,
                        ModifyDate = DateTime.Now
                    }, item => item.ApplicationID == this.application.ID && item.VoteStepID == firstVoteStep.ID);

                    //当前步骤的审批意见
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
                    {
                        Summary = summary
                    }, item => item.ID == currentStep.ID);
                }

                //审批日志
                reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps()
                {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.ApplyVoteStepLog),
                    ApplicationID = currentStep.ApplicationID,
                    VoteStepID = currentStep.VoteStepID,
                    AdminID = currentStep.AdminID,
                    Status = (int)(allow ? ApprovalStatus.Agree : ApprovalStatus.Reject),
                    Summary = summary,
                    CreateDate = DateTime.Now
                });

            }

            return isLastStep;
        }

        #endregion

        #region 审批否决

        /// <summary>
        /// 审批否决
        /// </summary>
        public void Reject(string summary)
        {
            this.Next(summary, false);
        }

        #endregion

    }

    /// <summary>
    /// 审批流管理
    /// </summary>
    public class VoteFlowManager : IEnumerable<MyVoteFlow>
    {
        IEnumerable<MyVoteFlow> data;

        #region 构造器

        VoteFlowManager()
        {
            this.Init();
        }

        #endregion

        #region 索引器

        /// <summary>
        /// 获取审批流
        /// </summary>
        /// <param name="type">申请审批的类型</param>
        /// <param name="days">请假天数(请假申请时填写)</param>
        /// <returns></returns>
        public MyVoteFlow this[ApplicationType type, decimal? days = null]
        {
            get
            {
                MyVoteFlow my;
                if (days == null)
                {
                    my = Interior.Single(item => item.Type == type);
                }
                else
                {
                    my = Interior.Single(item => item.Type == type
                        && item.LowerLimit < days
                        && (item.UpperLimit == null || item.UpperLimit >= days));
                }
                return my;
            }
        }

        #endregion

        /// <summary>
        /// 初始化审批流基础数据
        /// </summary>
        public void Init()
        {
            var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "voteflows.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(json))
            using (JsonTextReader reader = new JsonTextReader(file))
            using (var reponsitory = new PvbErmReponsitory())
            {
                JArray jArray = (JArray)JToken.ReadFrom(reader);
                var voteFlows = jArray.ToObject<List<VoteFlow>>();

                var flows = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>().ToArray();
                var steps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().ToArray();

                var linqs = from flow in flows
                            join step in steps on flow.ID equals step.VoteFlowID into _steps
                            let config = voteFlows.SingleOrDefault(item => item.ID == flow.ID)
                            select new MyVoteFlow
                            {
                                ID = flow.ID,
                                Name = flow.Name,
                                Type = config.Type,
                                Steps = _steps.Select(item => new Step
                                {
                                    ID = item.ID,
                                    Name = item.Name,
                                    AdminID = item.AdminID,
                                    OrderIndex = item.OrderIndex
                                }).OrderBy(item => item.OrderIndex).ToArray(),
                                LowerLimit = config.LowerLimit,
                                UpperLimit = config.UpperLimit
                            };
                this.data = linqs.ToList();
            }
        }

        /// <summary>
        /// 为申请服务
        /// </summary>
        /// <param name="application">申请对象</param>
        /// <returns>返回 申请管理</returns>
        public ForVoteFlow For(Application application)
        {
            if (application == null || application.ID == null)
                throw new Exception($"{nameof(Application)}对象不能为空!");

            #region 线程槽

            var nds = Thread.GetNamedDataSlot($"oject_{application.ID}_{nameof(Application)}");
            ForVoteFlow fvf = Thread.GetData(nds) as ForVoteFlow;

            if (fvf == null)
            {
                Thread.SetData(nds, fvf = new ForVoteFlow(application));
            }

            #endregion

            return fvf;
        }

        public IEnumerator<MyVoteFlow> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static VoteFlowManager Current
        {
            get
            {
                return Interior;
            }
        }

        private static readonly object locker = new object();
        private static VoteFlowManager interior;
        internal static VoteFlowManager Interior
        {
            get
            {
                lock (locker)
                {
                    if (interior == null)
                    {
                        lock (locker)
                        {
                            interior = new VoteFlowManager();
                        }
                    }
                }

                return interior;
            }
        }
    }

    /// <summary>
    /// 审批流
    /// </summary>
    public class MyVoteFlow
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }
        public ApplicationType Type { get; internal set; }

        internal MyVoteFlow() { }

        public decimal? LowerLimit { get; internal set; }
        public decimal? UpperLimit { get; internal set; }
        public Step[] Steps { get; internal set; }
    }

    /// <summary>
    /// 审批步骤
    /// </summary>
    public class Step
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }
        public string AdminID { get; internal set; }
        public int OrderIndex { get; internal set; }
    }
}
