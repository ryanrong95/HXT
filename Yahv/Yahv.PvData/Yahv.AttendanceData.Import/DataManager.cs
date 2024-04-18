using Layers.Data.Sqls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Models;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 数据管理类
    /// </summary>
    public class DataManager
    {
        private static readonly object locker = new object();
        private static DataManager instance;
        public static DataManager Current
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        lock (locker)
                        {
                            instance = new DataManager();
                        }
                    }
                }

                return instance;
            }
        }

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
                    my = Current.voteFlows.Single(item => item.Type == type);
                }
                else
                {
                    my = Current.voteFlows.Single(item => item.Type == type
                        && item.LowerLimit < days
                        && (item.UpperLimit == null || item.UpperLimit >= days));
                }
                return my;
            }
        }

        #endregion

        #region 数据配置

        /// <summary>
        /// 审批流配置
        /// </summary>
        private List<MyVoteFlow> voteFlows;
        public List<MyVoteFlow> VoteFlows
        {
            get
            {
                if (this.voteFlows == null)
                {
                    var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\jsons", "voteflows.json");
                    var voteFlowsConfig = JsonTo<List<MyVoteFlow>>(json);

                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        var flows = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>().ToArray();
                        var steps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().ToArray();

                        var linqs = from flow in flows
                                    join step in steps on flow.ID equals step.VoteFlowID into _steps
                                    let config = voteFlowsConfig.SingleOrDefault(item => item.ID == flow.ID)
                                    select new MyVoteFlow
                                    {
                                        ID = flow.ID,
                                        Name = flow.Name,
                                        Type = config.Type,
                                        Steps = _steps.Select(item => new MyVoteFlow.Step
                                        {
                                            ID = item.ID,
                                            Name = item.Name,
                                            AdminID = item.AdminID,
                                            OrderIndex = item.OrderIndex
                                        }).OrderBy(item => item.OrderIndex).ToArray(),
                                        LowerLimit = config.LowerLimit,
                                        UpperLimit = config.UpperLimit
                                    };
                        this.voteFlows = linqs.ToList();
                    }
                }

                return this.voteFlows;
            }
        }

        /// <summary>
        /// 芯达通组织架构
        /// </summary>
        private List<Models.XdtStaff> xdtStaffs;
        public List<Models.XdtStaff> XdtStaffs
        {
            get
            {
                if (this.xdtStaffs == null)
                {
                    var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\jsons", "xdt.organization.json");
                    var xdtStaffsConfig = JsonTo<List<Models.XdtStaff>>(json);

                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        var names = xdtStaffsConfig.Select(item => item.Name).ToArray();
                        var staffs = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>().Where(item => names.Contains(item.Name)).ToArray();

                        var linq = from staff in staffs
                                   join config in xdtStaffsConfig on staff.Name equals config.Name
                                   select new XdtStaff
                                   {
                                       ID = staff.ID,
                                       DyjCode = staff.DyjCode,
                                       DyjID = config.DyjID,
                                       Name = config.Name,
                                       DepartmentType = config.DepartmentType,
                                       PostType = config.PostType,
                                       AdminID = config.AdminID,
                                       HireDate = config.HireDate,
                                       LeaveDate = config.LeaveDate,
                                       RegionID = staff.RegionID,
                                       SchedulingID = staff.SchedulingID ?? config.SchedulingID
                                   };

                        this.xdtStaffs = linq.ToList();
                    }
                }
                return this.xdtStaffs;
            }
        }

        /// <summary>
        /// 班别管理
        /// </summary>
        private List<Models.Scheduling> schedulings;
        public List<Models.Scheduling> Schedulings
        {
            get
            {
                if (this.schedulings == null)
                {
                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        var linq = from entity in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedulings>()
                                   select new Models.Scheduling
                                   {
                                       ID = entity.ID,
                                       Name = entity.Name,
                                       PostionID = entity.PostionID,
                                       AmStartTime = entity.AmStartTime,
                                       AmEndTime = entity.AmEndTime,
                                       PmStartTime = entity.PmStartTime,
                                       PmEndTime = entity.PmEndTime,
                                       DomainValue = entity.DomainValue,
                                       Summary = entity.Summary
                                   };
                        this.schedulings = linq.ToList();
                    }
                }
                return this.schedulings;
            }
        }

        /// <summary>
        /// 员工哺乳假管理
        /// </summary>
        private List<Models.BreastfeedingLeave> breastfeedingLeaves;
        public List<Models.BreastfeedingLeave> BreastfeedingLeaves
        {
            get
            {
                if (this.breastfeedingLeaves == null)
                {
                    var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\jsons", "staff.breastfeedingLeave.json");
                    this.breastfeedingLeaves = JsonTo<List<Models.BreastfeedingLeave>>(json);
                }
                return this.breastfeedingLeaves;
            }
        }

        #endregion

        private static T JsonTo<T>(string json)
        {
            T data;
            using (System.IO.StreamReader file = System.IO.File.OpenText(json))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JArray jArray = (JArray)JToken.ReadFrom(reader);
                    data = jArray.ToObject<T>();
                }
            }

            return data;
        }
    }
}
