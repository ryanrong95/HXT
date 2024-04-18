using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;

namespace Yahv.AttendanceData.Import.Services
{
    /// <summary>
    /// 审批流基础数据导入
    /// </summary>
    public class VoteFlowService : IDataService
    {
        #region 需要保存的数据

        //审批流
        Layers.Data.Sqls.PvbErm.VoteFlows[] voteFlows;
        Layers.Data.Sqls.PvbErm.VoteSteps[] voteSteps;

        #endregion

        /// <summary>
        /// 数据读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IDataService Read(string path = null)
        {
            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public IDataService Encapsule()
        {
            voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[6]
            {
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "RZVF",
                    Name = "入职审批",
                    Type = (int)ApplicationType.Entry,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "LZVF",
                    Name = "离职审批",
                    Type = (int)ApplicationType.Leave,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "JBVF",
                    Name = "加班审批",
                    Type = (int)ApplicationType.Overtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "QJBelow3VF",
                    Name = "3天以内请假审批",
                    Type = (int)ApplicationType.Offtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "QJAbove3VF",
                    Name = "3天以上请假审批",
                    Type = (int)ApplicationType.Offtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "BQVF",
                    Name = "补签审批",
                    Type = (int)ApplicationType.ReSign,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };

            var adminID = DataManager.Current.XdtStaffs.Single(item => item.Name == "张庆永").AdminID;
            voteSteps = new Layers.Data.Sqls.PvbErm.VoteSteps[17]
            {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "RZVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "RZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "RZVS2",
                    Name = "总经理核准",
                    VoteFlowID = "RZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "RZVS3",
                    Name = "行政部记录",
                    VoteFlowID = "RZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "LZVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "LZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "LZVS2",
                    Name = "总经理批准",
                    VoteFlowID = "LZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "LZVS3",
                    Name = "行政部审批",
                    VoteFlowID = "LZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JBVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "JBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JBVS2",
                    Name = "总经理核准",
                    VoteFlowID = "JBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JBVS3",
                    Name = "行政部记录",
                    VoteFlowID = "JBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJBelow3VS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "QJBelow3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJBelow3VS2",
                    Name = "行政部记录",
                    VoteFlowID = "QJBelow3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJAbove3VS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "QJAbove3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJAbove3VS2",
                    Name = "总经理核准",
                    VoteFlowID = "QJAbove3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJAbove3VS3",
                    Name = "行政部记录",
                    VoteFlowID = "QJAbove3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "BQVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "BQVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "BQVS2",
                    Name = "总经理核准",
                    VoteFlowID = "BQVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "BQVS3",
                    Name = "行政部记录",
                    VoteFlowID = "BQVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID,
                },
            };

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            using (var conn = ConnManager.Current.PvbErm)
            {
                //审批流
                conn.BulkInsert(voteFlows);
                //审批步骤
                conn.BulkInsert(voteSteps);
            }
        }
    }
}
