using Layers.Data.Sqls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using System.Collections;

namespace Yahv.Erm.Services
{


    //public class MyEventArgs : EventArgs
    //{

    //}

    //public class MyClass1
    //{

    //    public MyClass1()
    //    {
    //        Application application = null;

    //        VoteFlowManager_chenhan.Current.For(application).Init();
    //        VoteFlowManager_chenhan.Current.For(application).Next();
    //        VoteFlowManager_chenhan.Current.For(application).Reject();
    //        var flow = VoteFlowManager_chenhan.Current[ApplicationType.Entry, 1];

    //        var sessioin = VoteFlowManager_chenhan.Current.For(application).Next();
    //        //response.redirct


    //        //foreach (var item in collection)
    //        //{

    //        //}


    //        //VoteFlowManager_chenhan.Current.For(application)

    //    }
    //}

    ///// <summary>
    ///// 管理要求
    ///// </summary>
    //public class ForVoteFlow_chenhan
    //{



    //    Application application;

    //    /// <summary>
    //    /// 构造器
    //    /// </summary>
    //    internal ForVoteFlow_chenhan(Application application)
    //    {
    //        this.application = application;
    //    }

    //    #region 生成申请的审批步骤

    //    /// <summary>
    //    /// 创建申请审批步骤
    //    /// </summary>
    //    public void Init()
    //    {
    //        using (var reponsitory = new PvbErmReponsitory())
    //        {
    //            //删除已经生成的申请审批步骤
    //            reponsitory.Delete<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(item => item.ApplicationID == application.ID);

    //            //获取审批流的审批步骤
    //            var voteSteps = VoteFlowManager_chenhan.Interior.Single(item => item.ID == application.VoteFlowID).Steps;
    //            var ids = Underly.PKeyType.ApplyVoteStep.Pick(voteSteps.Length);

    //            //根据芯达通组织架构获取部门负责人
    //            var staff = XDTOrganization.StaffList.Single(item => item.AdminID == application.ApplicantID);
    //            var manager = XDTOrganization.StaffList.Single(item => item.DepartmentType == staff.DepartmentType && item.PostType != PostType.Staff);

    //            //生成申请审批步骤
    //            var applyVoteSteps = voteSteps.Select((item, index) => new Layers.Data.Sqls.PvbErm.ApplyVoteSteps()
    //            {
    //                ID = ids[index],
    //                ApplicationID = application.ID,
    //                VoteStepID = item.ID,
    //                IsCurrent = item.OrderIndex == 1 ? true : false,
    //                AdminID = item.OrderIndex == 1 ? manager.AdminID : item.AdminID,
    //                Status = (int)ApprovalStatus.Waiting,
    //                CreateDate = DateTime.Now,
    //                ModifyDate = DateTime.Now
    //            }).ToArray();

    //            reponsitory.Insert(applyVoteSteps);
    //        }
    //    }

    //    #endregion

    //    #region 审批通过

    //    /// <summary>
    //    /// 更新申请审批步骤的状态
    //    /// </summary>
    //    /// <param name="allow">
    //    /// 真：allow
    //    /// 假：Veto
    //    /// </param>
    //    /// <returns>是否为最后一步</returns>
    //    public bool Next(bool allow = true)
    //    {
    //        bool isLastStep = false;

    //        using (var reponsitory = new PvbErmReponsitory())
    //        {
    //            //申请步骤
    //            var voteSteps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>().Where(item => item.ApplicationID == application.ID).ToArray();
    //            var currentStep = voteSteps.Single(item => item.IsCurrent);

    //            // --- 实际的数据变更

    //            //审批步骤
    //            var voteFlow = VoteFlowManager_chenhan.Interior.Single(item => item.ID == application.VoteFlowID);

    //            //当前的逻辑
    //            var currentVoteStep = voteFlow.Steps.Single(item => item.ID == currentStep.VoteStepID);

    //            var nextVoteStep = voteFlow.Steps.FirstOrDefault(item => item.OrderIndex == (currentVoteStep.OrderIndex + 1));

    //            //如果没有下一步审批，则当前步骤为最后一步
    //            if (nextVoteStep == null)
    //            {
    //                isLastStep = true;
    //            }

    //            #region 建议保留：


    //            if (allow)
    //            {
    //                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //                {
    //                    IsCurrent = false,
    //                    Status = (int)ApprovalStatus.Agree,
    //                }, item => item.ApplicationID == this.application.ID);

    //                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //                {
    //                    IsCurrent = true
    //                }, item => item.ApplicationID == this.application.ID && item.VoteStepID == nextVoteStep.ID);

    //            }
    //            else
    //            {
    //                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //                {
    //                    IsCurrent = false,
    //                    Status = (int)ApprovalStatus.Waiting
    //                }, item => item.ApplicationID == application.ID);

    //                reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //                {
    //                    IsCurrent = true,
    //                    Status = (int)ApprovalStatus.Waiting
    //                }, item => item.ApplicationID == application.ID && item.ID == voteSteps.First().ID);
    //            }

    //            reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps()
    //            {
    //                ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.ApplyVoteStepLog),
    //                ApplicationID = currentStep.ApplicationID,
    //                VoteStepID = currentStep.VoteStepID,
    //                AdminID = currentStep.AdminID,
    //                Status = (int)(allow ? ApprovalStatus.Agree : ApprovalStatus.Reject),
    //                Summary = currentStep.Summary,
    //                CreateDate = DateTime.Now
    //            });

    //            #endregion

    //            #region _bak

    //            //while (voteFlow.MoveNext())
    //            //{
    //            //    var step = voteFlow.Current;

    //            //    if (allow)
    //            //    {
    //            //        //更新当前申请审批步骤的IsCurrent为false, 审批状态为赞同
    //            //        if (step.ID == currentVoteStep.ID)
    //            //        {
    //            //            reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //            //            {
    //            //                IsCurrent = false,
    //            //                Status = (int)ApprovalStatus.Agree,
    //            //            }, item => item.ID == step.ID);
    //            //        }

    //            //        //更新下一个Step的IsCurrent为true
    //            //        if (step.ID == nextVoteStep.ID)
    //            //        {
    //            //            reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //            //            {
    //            //                IsCurrent = true
    //            //            }, item => item.ApplicationID == application.ID && item.VoteStepID == step.ID);
    //            //        }
    //            //    }
    //            //    else
    //            //    {
    //            //        //TODO: 申请否决的业务流程还需要再讨论，如果有变化则需要修改以下逻辑

    //            //        //审批否决，申请回到初始状态，更新申请审批的第一步的IsCurrent为true，审批状态为待审批
    //            //        if (step.OrderIndex == 1)
    //            //        {
    //            //            reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //            //            {
    //            //                IsCurrent = true,
    //            //                Status = (int)ApprovalStatus.Waiting
    //            //            }, item => item.ApplicationID == application.ID && item.VoteStepID == step.ID);
    //            //        }
    //            //        //更新申请审批的其他步骤的IsCurrent为false，审批状态为待审批
    //            //        else
    //            //        {
    //            //            reponsitory.Update<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>(new
    //            //            {
    //            //                IsCurrent = false,
    //            //                Status = (int)ApprovalStatus.Waiting
    //            //            }, item => item.ApplicationID == application.ID && item.VoteStepID == step.ID);
    //            //        }
    //            //    }

    //            //    //记录审批日志
    //            //    if (step.ID == currentVoteStep.ID)
    //            //    {
    //            //        reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps()
    //            //        {
    //            //            ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.ApplyVoteStepLog),
    //            //            ApplicationID = currentStep.ApplicationID,
    //            //            VoteStepID = currentStep.VoteStepID,
    //            //            AdminID = currentStep.AdminID,
    //            //            Status = (int)(allow ? ApprovalStatus.Agree : ApprovalStatus.Reject),
    //            //            Summary = currentStep.Summary,
    //            //            CreateDate = DateTime.Now
    //            //        });
    //            //    }
    //            //}

    //            //voteFlow.Reset();

    //            #endregion
    //        }

    //        return isLastStep;
    //    }

    //    #endregion

    //    #region 审批否决

    //    /// <summary>
    //    /// 审批否决
    //    /// </summary>
    //    public void Reject()
    //    {
    //        this.Next(false);
    //    }

    //    #endregion

    //}

    ///// <summary>
    ///// 审批流管理
    ///// </summary>
    //public class VoteFlowManager_chenhan : IEnumerable<MyVoteFlow_chenhan>
    //{
    //    static object elocker = new object();
    //    public event EventHandler<MyEventArgs> Initializing
    //    {
    //        add
    //        {
    //            lock (elocker)
    //            {
    //                var nds = Thread.GetNamedDataSlot($"event_{Thread.CurrentThread.ManagedThreadId}_{nameof(EventHandler<MyEventArgs>)}");
    //                EventHandler<MyEventArgs> sender = Thread.GetData(nds) as EventHandler<MyEventArgs>;

    //                if (sender == null)
    //                {
    //                    Thread.SetData(nds, value);
    //                }
    //            }
    //        }
    //        remove
    //        {
    //            lock (elocker)
    //            {
    //                var nds = Thread.GetNamedDataSlot($"event_{Thread.CurrentThread.ManagedThreadId}_{nameof(EventHandler<MyEventArgs>)}");
    //                EventHandler<MyEventArgs> sender = Thread.GetData(nds) as EventHandler<MyEventArgs>;
    //                if (sender != null)
    //                {
    //                    Thread.SetData(nds, null);
    //                }
    //            }
    //        }
    //    }

    //    IEnumerable<MyVoteFlow_chenhan> data;

    //    #region 构造器

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <remarks>
    //    /// 
    //    /// </remarks>
    //    VoteFlowManager_chenhan()
    //    {
    //        this.InitData();
    //    }

    //    #endregion

    //    #region 索引器

    //    /// <summary>
    //    /// 获取审批流
    //    /// </summary>
    //    /// <param name="type">申请审批的类型</param>
    //    /// <param name="days">请假天数(请假申请时填写)</param>
    //    /// <returns></returns>
    //    public MyVoteFlow_chenhan this[ApplicationType type, decimal? days = null]
    //    {
    //        get
    //        {
    //            MyVoteFlow_chenhan my;
    //            if (days == null)
    //            {
    //                my = Interior.Single(item => item.Type == type);
    //            }
    //            else
    //            {
    //                my = Interior.Single(item => item.Type == type
    //                    && item.LowerLimit < days
    //                    && (item.UpperLimit == null || item.UpperLimit >= days));
    //            }
    //            return my;
    //        }
    //    }

    //    #endregion

    //    public void InitData()
    //    {
    //        if (this != null)
    //        {
    //            var nds = Thread.GetNamedDataSlot($"event_{Thread.CurrentThread.ManagedThreadId}_{nameof(EventHandler<MyEventArgs>)}");
    //            EventHandler<MyEventArgs> events = Thread.GetData(nds) as EventHandler<MyEventArgs>;

    //            if (events != null)
    //            {
    //                events(this, new MyEventArgs());
    //            }
    //        }

    //        var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "voteflows.json");
    //        using (System.IO.StreamReader file = System.IO.File.OpenText(json))
    //        using (JsonTextReader reader = new JsonTextReader(file))
    //        using (var reponsitory = new PvbErmReponsitory())
    //        {
    //            JArray jArray = (JArray)JToken.ReadFrom(reader);
    //            var voteFlows = jArray.ToObject<List<VoteFlow>>();

    //            var flows = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>().ToArray();
    //            var steps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().ToArray();

    //            var linqs = from flow in flows
    //                        join step in steps on flow.ID equals step.VoteFlowID into _steps
    //                        let config = voteFlows.SingleOrDefault(item => item.ID == flow.ID)
    //                        select new MyVoteFlow_chenhan
    //                        {
    //                            ID = flow.ID,
    //                            Name = flow.Name,
    //                            Type = config.Type,
    //                            Steps = _steps.Select(item => new Step_chenhan
    //                            {
    //                                ID = item.ID,
    //                                Name = item.Name,
    //                                AdminID = item.AdminID,
    //                                OrderIndex = item.OrderIndex
    //                            }).OrderBy(item => item.OrderIndex).ToArray(),
    //                            LowerLimit = config.LowerLimit,
    //                            UpperLimit = config.UpperLimit
    //                        };
    //            this.data = linqs.ToList();
    //        }
    //    }

    //    /// <summary>
    //    /// 为申请服务
    //    /// </summary>
    //    /// <param name="application">申请对象</param>
    //    /// <returns>返回 申请管理</returns>
    //    public ForVoteFlow_chenhan For(Application application)
    //    {
    //        if (application == null || application.ID == null)
    //            throw new Exception($"{nameof(Application)}对象不能为空!");

    //        var nds = Thread.GetNamedDataSlot($"oject_{application.ID}_{nameof(Application)}");
    //        ForVoteFlow_chenhan fvf = Thread.GetData(nds) as ForVoteFlow_chenhan;

    //        if (fvf == null)
    //        {
    //            Thread.SetData(nds, fvf = new ForVoteFlow_chenhan(application));
    //        }

    //        return fvf;
    //    }

    //    public IEnumerator<MyVoteFlow_chenhan> GetEnumerator()
    //    {
    //        return this.data.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }

    //    public static VoteFlowManager_chenhan Current
    //    {
    //        get
    //        {
    //            return Interior;
    //        }
    //    }

    //    private static readonly object locker = new object();
    //    private static VoteFlowManager_chenhan interior;
    //    internal static VoteFlowManager_chenhan Interior
    //    {
    //        get
    //        {
    //            lock (locker)
    //            {
    //                if (interior == null)
    //                {
    //                    lock (locker)
    //                    {
    //                        interior = new VoteFlowManager_chenhan();
    //                    }
    //                }
    //            }

    //            return interior;
    //        }
    //    }

    //}

    //public class MyVoteFlow_chenhan : IEnumerator<Step_chenhan>
    //{
    //    public string ID { get; internal set; }
    //    public string Name { get; internal set; }
    //    public ApplicationType Type { get; internal set; }

    //    internal MyVoteFlow_chenhan() { }

    //    public decimal? LowerLimit { get; internal set; }
    //    public decimal? UpperLimit { get; internal set; }
    //    public Step_chenhan[] Steps { get; internal set; }

    //    int position = -1;

    //    public Step_chenhan Current
    //    {
    //        get
    //        {
    //            return Steps[position];
    //        }
    //    }

    //    object IEnumerator.Current
    //    {
    //        get
    //        {
    //            return Current;
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool MoveNext()
    //    {
    //        position++;
    //        return position < Steps.Length;
    //    }

    //    public void Reset()
    //    {
    //        position = -1;
    //    }
    //}

    //public class Step_chenhan
    //{
    //    public string ID { get; internal set; }
    //    public string Name { get; internal set; }
    //    public string AdminID { get; internal set; }
    //    public int OrderIndex { get; internal set; }
    //}
}
