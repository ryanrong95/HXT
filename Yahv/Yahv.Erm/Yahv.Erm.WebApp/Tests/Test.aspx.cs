using Layers.Data.Sqls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.WebApp.Tests
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = string.Concat("Staff01570", StaffApprovalStep.Interview.ToString()).MD5();
            //DYJRSAPI.Instance.GetWorkDate("4820", new DateTime(2020, 9, 10));
        }

        public void LoadData()
        {
            string connectString1 = "Data Source=172.30.10.199,5311;Initial Catalog=PvbErm;Persist Security Info=True;User ID=udata;Password=Turing2019";
            string connectString2 = "Data Source=172.30.10.51,6522;Initial Catalog=PvbErm;Persist Security Info=True;User ID=u_v0;Password=V6e1W9MiA84t6FJ05TXCU9uRY9THg3EpT60v9X1qwMbLg895Oc";

            SqlConnection sqlCnt = new SqlConnection(connectString1);
            sqlCnt.Open();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter("select * from PvbErm.dbo.VoteFlows", sqlCnt);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "PvbErm.dbo.VoteFlows");


            using (SqlConnection destinationConnection = new SqlConnection(connectString2))
            {
                destinationConnection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    try
                    {
                        bulkCopy.DestinationTableName = "PvbErm.dbo.VoteFlows";//要插入的表的表名                        
                        bulkCopy.BatchSize = myDataSet.Tables[0].Rows.Count;
                        bulkCopy.ColumnMappings.Add("ID", "ID");//映射字段名 DataTable列名 ,数据库 对应的列名                          
                        bulkCopy.ColumnMappings.Add("Name", "Name");
                        bulkCopy.ColumnMappings.Add("Type", "Type");
                        bulkCopy.ColumnMappings.Add("CreatorID", "CreatorID");
                        bulkCopy.ColumnMappings.Add("ModifyID", "ModifyID");
                        bulkCopy.ColumnMappings.Add("CreateDate", "CreateDate");
                        bulkCopy.ColumnMappings.Add("ModifyDate", "ModifyDate");
                        bulkCopy.WriteToServer(myDataSet.Tables[0]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                    }
                }
            }
            sqlCnt.Close();
        }

        void VoteFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[6]
            {
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "RZVF",
                    Name = "入职审批",
                    Type = (int)Services.ApplicationType.Entry,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "LZVF",
                    Name = "离职审批",
                    Type = (int)Services.ApplicationType.Leave,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "JBVF",
                    Name = "加班审批",
                    Type = (int)Services.ApplicationType.Overtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "QJBelow3VF",
                    Name = "3天以内请假审批",
                    Type = (int)Services.ApplicationType.Offtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "QJAbove3VF",
                    Name = "3天以上请假审批",
                    Type = (int)Services.ApplicationType.Offtime,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
                new Layers.Data.Sqls.PvbErm.VoteFlows()
                {
                    ID = "BQVF",
                    Name = "补签审批",
                    Type = (int)Services.ApplicationType.ReSign,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };

            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>
            {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "RZVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "RZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = "",
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
                    Name = "行政部审批",
                    VoteFlowID = "RZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "LZVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "LZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = "",
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
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JBVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "JBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = "",
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JBVS2",
                    Name = "行政部审批",
                    VoteFlowID = "JBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJBelow3VS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "QJBelow3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = "",
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJBelow3VS2",
                    Name = "行政部审批",
                    VoteFlowID = "QJBelow3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "QJAbove3VS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "QJAbove3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = "",
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
                    Name = "行政部审批",
                    VoteFlowID = "QJAbove3VF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "BQVS1",
                    Name = "行政部审批",
                    VoteFlowID = "BQVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
            };

            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void RecruitVoteFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "ZPVF",
                    Name = "招聘审批",
                    Type = (int)Services.ApplicationType.Recruit,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "ZPVS1",
                    Name = "行政部审核",
                    VoteFlowID = "ZPVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "ZPVS2",
                    Name = "总经理审批",
                    VoteFlowID = "ZPVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "ZPVS3",
                    Name = "行政部招聘",
                    VoteFlowID = "ZPVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void SealBorrowVoteFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "YZJYVF",
                    Name = "印章借用审批",
                    Type = (int)Services.ApplicationType.SealBorrow,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "YZJYVS1",
                    Name = "总经理审批",
                    VoteFlowID = "YZJYVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "YZJYVS2",
                    Name = "行政部安排盖章",
                    VoteFlowID = "YZJYVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "YZJYVS3",
                    Name = "盖章人执行盖章",
                    VoteFlowID = "YZJYVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                }
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void WorkCardVoteFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "GPBBVF",
                    Name = "工牌补办审批",
                    Type = (int)Services.ApplicationType.WorkCard,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "GPBBVS1",
                    Name = "部门负责人审核",
                    VoteFlowID = "GPBBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "GPBBVS2",
                    Name = "行政部受理",
                    VoteFlowID = "GPBBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "GPBBVS3",
                    Name = "行政部通知工牌领取",
                    VoteFlowID = "GPBBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "GPBBVS4",
                    Name = "申请人领取新工牌",
                    VoteFlowID = "GPBBVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 4,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void InternalTrainingFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "PSVF",
                    Name = "培训申请审批",
                    Type = (int)Services.ApplicationType.InternalTraining,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "PSVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "PSVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "PSVS2",
                    Name = "行政部审批",
                    VoteFlowID = "PSVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void ExternalTrainingFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "WSVF",
                    Name = "外训申请审批",
                    Type = (int)Services.ApplicationType.ExternalTraining,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "WSVS1",
                    Name = "行政部审批",
                    VoteFlowID = "WSVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "WSVS2",
                    Name = "总经理审批",
                    VoteFlowID = "WSVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void ArchiveBorrowFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "DZDAJYVF",
                    Name = "单证档案借阅申请",
                    Type = (int)Services.ApplicationType.ArchiveBorrow,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDAJYVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "DZDAJYVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDAJYVS2",
                    Name = "行政部审批并执行",
                    VoteFlowID = "DZDAJYVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void ArchiveLendingFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "DZDAWJVF",
                    Name = "单证档案外借申请",
                    Type = (int)Services.ApplicationType.ArchiveLending,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDAWJVS1",
                    Name = "部门负责人审批",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDAWJVS2",
                    Name = "总经理审批",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDAWJVS3",
                    Name = "行政部审批并执行",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void ArchiveDestroyFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "DZDASHVF",
                    Name = "单证档案销毁申请",
                    Type = (int)Services.ApplicationType.ArchiveLending,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDASHVS1",
                    Name = "审计部审批",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDASHVS2",
                    Name = "总经理审批",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "DZDASHVS3",
                    Name = "行政部审批并执行",
                    VoteFlowID = "DZDAWJVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void SealEngraveFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "YZKZVF",
                    Name = "印章刻制申请",
                    Type = (int)Services.ApplicationType.SealEngrave,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "YZKZVS1",
                    Name = "总经理审批",
                    VoteFlowID = "YZKZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "YZKZVS2",
                    Name = "行政部安排刻制",
                    VoteFlowID = "YZKZVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID2,
                }
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void RewardAndPunishFlowEnter()
        {
            var voteFlows = new Layers.Data.Sqls.PvbErm.VoteFlows[1]
            {
                 new Layers.Data.Sqls.PvbErm.VoteFlows()
                 {
                    ID = "JCVF",
                    Name = "员工奖惩申请",
                    Type = (int)Services.ApplicationType.RewardAndPunish,
                    CreatorID = "NPC",
                    CreateDate =DateTime.Now,
                    ModifyDate = DateTime.Now
                },
            };
            var adminID = XDTOrganization.StaffList.Single(item => item.Name == "张庆永").AdminID;
            var adminID2 = XDTOrganization.StaffList.Single(item => item.Name == "鲁亚慧").AdminID;
            var voteSteps = new List<Layers.Data.Sqls.PvbErm.VoteSteps>() {
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JCVS1",
                    Name = "行政部审核",
                    VoteFlowID = "JCVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 1,
                    AdminID = adminID2,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JCVS2",
                    Name = "总经理审批",
                    VoteFlowID = "JCVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 2,
                    AdminID = adminID,
                },
                new Layers.Data.Sqls.PvbErm.VoteSteps()
                {
                    ID = "JCVS3",
                    Name = "行政部执行",
                    VoteFlowID = "JCVF",
                    CreateDate = DateTime.Now,
                    OrderIndex = 3,
                    AdminID = adminID2,
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(voteFlows);
                reponsitory.Insert(voteSteps.ToArray());
            }
        }

        void TestVoteFlowManager()
        {
            /*
            var json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "voteflows.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(json))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JArray jArray = (JArray)JToken.ReadFrom(reader);
                    var voteFlows = jArray.ToObject<List<Services.Models.Origins.VoteFlow>>();
                }
            }
            */

            //获取VoteFlow
            var rzVF = Services.VoteFlowManager.Current[Services.ApplicationType.Entry];
            var lzVF = Services.VoteFlowManager.Current[Services.ApplicationType.Leave];
            var jbVF = Services.VoteFlowManager.Current[Services.ApplicationType.Overtime];
            var qjVFBelow3 = Services.VoteFlowManager.Current[Services.ApplicationType.Offtime, 2.5m];
            var qjVFAbove3 = Services.VoteFlowManager.Current[Services.ApplicationType.Offtime, 4m];
            var bqVF = Services.VoteFlowManager.Current[Services.ApplicationType.ReSign];

            //生成申请的审批步骤
            var application = new Services.Models.Origins.Application()
            {
                ID = "",
                VoteFlowID = "",
                ApplicantID = ""
            };
            Services.VoteFlowManager.Current.For(application).Init();
            Services.VoteFlowManager.Current.For(application).Next("");
        }

        void TestPvDataApi()
        {
            //var result= Yahv.Utils.Http.ApiHelper.Current.JPost<Underly.JMessage>("http://hv.erp.b1b.com/PvDataApi/Classify/Lock", new
            //{
            //    itemId = "OrderItem20191015000010",
            //    creatorId = "SA01",
            //    creatorName = "超级管理员",
            //    step = "1"
            //});

            var result1 = Yahv.Utils.Http.ApiHelper.Current.Get<Underly.JMessage>("http://hv.erp.b1b.com/PvDataApi/Order/GetProductIdByInfo", new
            {
                partNumber = "型号一",
                manufacturer = "品牌一"
            });

            var result2 = Yahv.Utils.Http.ApiHelper.Current.Get<Underly.JSingle<dynamic>>("http://hv.erp.b1b.com/PvDataApi/Classify/GetAutoClassified", new
            {
                partNumber = "型号一",
                manufacturer = "品牌一"
            });
        }

        void Logs_Attend()
        {
            var attend = new List<Layers.Data.Sqls.PvbErm.Logs_Attend>()
            {
                new Layers.Data.Sqls.PvbErm.Logs_Attend() {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.AttendLog),
                    Date = new DateTime(2020,5,7),
                    StaffID = "Staff01082",
                    CreateDate = new DateTime(2020,5,7,9,5,0)
                },
                new Layers.Data.Sqls.PvbErm.Logs_Attend() {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.AttendLog),
                    Date = new DateTime(2020,5,7),
                    StaffID = "Staff01082",
                    CreateDate = new DateTime(2020,5,7,18,5,0)
                },

                new Layers.Data.Sqls.PvbErm.Logs_Attend() {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.AttendLog),
                    Date = new DateTime(2020,5,8),
                    StaffID = "Staff01082",
                    CreateDate = new DateTime(2020,5,8,18,5,0)
                },

                new Layers.Data.Sqls.PvbErm.Logs_Attend() {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.AttendLog),
                    Date = new DateTime(2020,5,9),
                    StaffID = "Staff01082",
                    CreateDate = new DateTime(2020,5,9,9,5,0)
                },

                new Layers.Data.Sqls.PvbErm.Logs_Attend() {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.AttendLog),
                    Date = new DateTime(2020,5,10),
                    StaffID = "Staff01082",
                    CreateDate = new DateTime(2020,5,10,9,5,0)
                },
            };
            using (var reponsitory = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                reponsitory.Insert(attend.ToArray());
            }
        }

        void read()
        {
            using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "xdt_positions.json")))
            {
                string json = reader.ReadToEnd();
                xdtinfos infos = Utils.Serializers.JsonSerializerExtend.JsonTo<xdtinfos>(json);

                xdtinfos info = new xdtinfos();
                info.positions = new List<string>() { "总经理", "负责人" };
                string infoss = info.Json();
            }
        }
    }

    public class xdtinfos
    {
        public List<string> positions { get; set; }
    }
}