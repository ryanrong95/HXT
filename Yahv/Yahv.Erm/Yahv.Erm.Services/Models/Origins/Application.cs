using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using System;
using Yahv.Utils.Converters.Contents;
using Layers.Data;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using System.Collections.Generic;
using Yahv.Erm.Services.Views;
using Yahv.Services.Models;
using Yahv.Erm.Services.Interfaces;
using System.Configuration;
using NPOI.OpenXmlFormats.Dml.Diagram;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Common;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 申请
    /// </summary>
    public class Application : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string VoteFlowID { get; set; }

        public string Title { get; set; }

        public string Context { get; set; }

        public string ApplicantID { get; set; }

        public string CreatorID { get; set; }

        public DateTime CreateDate { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }

        #endregion

        #region 扩展属性

        public Admin Applicant { get; set; }

        public VoteFlow VoteFlow { get; set; }

        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public Staff StaffOrganization
        {
            get
            {
                return new StaffAlls().FirstOrDefault(item => item.Admin.ID == this.ApplicantID);
            }
        }

        /// <summary>
        /// 负责人
        /// </summary>
        public Staff ManagerOrganization
        {
            get
            {
                if (this.StaffOrganization == null || string.IsNullOrEmpty(this.StaffOrganization.DepartmentCode))
                {
                    return null;
                }
                else
                {
                    return new StaffAlls()
                        .Where(item => item.DepartmentCode == this.StaffOrganization.DepartmentCode)
                        .Where(item => item.PostionCode == PostType.Manager.ToString() || item.PostionCode == PostType.President.ToString())
                        .FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 加班申请内容
        /// </summary>
        public OverTimeContext OverTimeContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.Overtime ? this.Context.JsonTo<OverTimeContext>() : null;
            }
        }

        /// <summary>
        /// 请假申请内容
        /// </summary>
        public OffTimeContext OffTimeContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.Offtime ? this.Context.JsonTo<OffTimeContext>() : null;
            }
        }

        /// <summary>
        /// 补签申请内容
        /// </summary>
        public ReSignContext ReSignContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.ReSign ? this.Context.JsonTo<ReSignContext>() : null;
            }
        }

        /// <summary>
        /// 离职申请内容
        /// </summary>
        public Resignation Resignation
        {
            get
            {
                return this.ApplicationType == ApplicationType.Leave ? this.Context.JsonTo<Resignation>() : null;
            }
        }

        /// <summary>
        /// 招聘申请内容
        /// </summary>
        public RecruitContext RecruitContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.Recruit ? this.Context.JsonTo<RecruitContext>() : null;
            }
        }

        /// <summary>
        /// 印章借用申请内容
        /// </summary>
        public SealBorrowContext SealBorrowContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.SealBorrow ? this.Context.JsonTo<SealBorrowContext>() : null;
            }
        }

        /// <summary>
        /// 印章刻制申请内容
        /// </summary>
        public SealEngraveContext SealEngraveContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.SealEngrave ? this.Context.JsonTo<SealEngraveContext>() : null;
            }
        }

        /// <summary>
        /// 员工奖惩申请内容
        /// </summary>
        public RewardAndPunishContext RewardAndPunishContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.RewardAndPunish ? this.Context.JsonTo<RewardAndPunishContext>() : null;
            }
        }

        /// <summary>
        /// 工牌补办申请内容
        /// </summary>
        public WorkCardContext WorkCardContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.WorkCard ? this.Context.JsonTo<WorkCardContext>() : null;
            }
        }

        /// <summary>
        /// 培训申请内容
        /// </summary>
        public InternalTrainingContext InternalTrainingContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.InternalTraining ? this.Context.JsonTo<InternalTrainingContext>() : null;
            }
        }

        /// <summary>
        /// 外训申请内容
        /// </summary>
        public ExternalTrainingContext ExternalTrainingContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.ExternalTraining ? this.Context.JsonTo<ExternalTrainingContext>() : null;
            }
        }

        /// <summary>
        /// 单证档案借阅申请内容
        /// </summary>
        public ArchiveBorrowContext ArchiveBorrowContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.ArchiveBorrow ? this.Context.JsonTo<ArchiveBorrowContext>() : null;
            }
        }

        /// <summary>
        /// 单证档案外借申请内容
        /// </summary>
        public ArchiveLendingContext ArchiveLendingContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.ArchiveLending ? this.Context.JsonTo<ArchiveLendingContext>() : null;
            }
        }

        /// <summary>
        /// 单证档案销毁申请内容
        /// </summary>
        public ArchiveDestroyContext ArchiveDestroyContext
        {
            get
            {
                return this.ApplicationType == ApplicationType.ArchiveDestroy ? this.Context.JsonTo<ArchiveDestroyContext>() : null;
            }
        }

        /// <summary>
        /// 申请的当前审批步骤
        /// </summary>
        public ApplyVoteStep CurrentVoteStep { get; set; }

        /// <summary>
        /// 申请的所有审批步骤
        /// </summary>
        public IEnumerable<ApplyVoteStep> Steps { get; set; }

        /// <summary>
        /// 申请的附件
        /// </summary>
        IEnumerable<CenterFileDescription> fileitems;
        public IEnumerable<CenterFileDescription> Fileitems
        {
            get
            {
                if (this.fileitems == null)
                {
                    this.fileitems = new Views.ApplicationFileAlls(this.ID);
                }
                return this.fileitems;
            }
            set
            {
                this.fileitems = value;
            }
        }

        #endregion

        #region 事件
        public event SuccessHanlder ApprovalCompleted;

        private void ApplicationApproval_Completed(object sender, SuccessEventArgs e)
        {
            var apply = sender as Application;
            using (var repository = new PvbErmReponsitory())
            {
                #region 加班审批完成
                if (apply.ApplicationType == ApplicationType.Overtime)
                {
                    if (apply.OverTimeContext.OvertimeExchangeType == OvertimeExchangeType.PayDay)
                    {
                        //新增一天员工的调休假期
                        var staff = new Views.StaffAlls().Single(item => item.Admin.ID == apply.ApplicantID);
                        if (apply.OverTimeContext.OvertimeExchangeType == OvertimeExchangeType.PayDay)
                        {
                            var vocation = new Views.Origins.VacationsOrigin()
                            .SingleOrDefault(item => item.StaffID == staff.ID && item.Type == VacationType.OffDay);
                            if (vocation == null)
                            {
                                throw new Exception("该员工假期未初始化");
                            }
                            repository.Update<Vacations>(new
                            {
                                Lefts = vocation.Lefts + 1,
                            }, item => item.ID == vocation.ID);
                        }
                    }
                }
                #endregion

                #region 离职审批完成
                if (apply.ApplicationType == ApplicationType.Leave)
                {
                    var admins = new AdminsAll(repository);
                    var admin = admins[apply.ApplicantID];
                    var resignation = apply.Context.JsonTo<Resignation>();

                    #region 停用账号
                    //停用账号
                    admins.Disable(admin.ID);
                    #endregion

                    #region 更新离职日期
                    //更新离职日期
                    repository.Update<Layers.Data.Sqls.PvbErm.Labours>(new { LeaveDate = resignation.ResignationDate }, item => item.ID == admin.StaffID);
                    repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new { Status = (int)StaffStatus.Departure }, item => item.ID == admin.StaffID);
                    #endregion

                    #region 工作承接
                    //若是跟单员 调用芯达通接口
                    if (admin.RoleID == "FRole011")
                    {
                        var swap = resignation.HandoverID;
                        var swapAdmin = admins[swap];
                        //todo 判断承接人是否为跟单员，跟单员只能交接为跟单员
                        if (swapAdmin.RoleID == "FRole011")
                        {
                            var apisetting = new WlAdminApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.HandOver;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
                            {
                                AdminLeave = admin.ID,
                                AdminWork = swapAdmin.ID,
                                ApplyID = apply.ID,
                            });
                            if (!result.success)
                            {
                                throw new Exception(result.Json());
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region 请假审批完成
                if (apply.ApplicationType == ApplicationType.Offtime)
                {
                    if (apply.OffTimeContext.Days >= 10)
                    {
                        var admins = new AdminsAll(repository);
                        var admin = admins[this.ApplicantID];
                        admins.Disable(admin.ID);//停用账号

                        #region 工作承接
                        var swap = apply.OffTimeContext.SwapStaff;
                        var swapAdmin = admins[swap];
                        //若是跟单员 调用芯达通接口
                        if (admin.RoleID == "FRole011" && swapAdmin.RoleID == "FRole011")
                        {
                            var apisetting = new WlAdminApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.HandOver;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
                            {
                                AdminLeave = admin.ID,
                                AdminWork = swapAdmin.ID,
                                ApplyID = apply.ID,
                            });
                            if (!result.success)
                            {
                                throw new Exception(result.Json());
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                //生成私有日程数据
                ProductSchedulePrivate(apply);

                //变更申请状态为已完成
                repository.Update<Applications>(new
                {
                    Status = (int)ApplicationStatus.Complete,
                }, item => item.ID == this.ID);
            }
        }

        #endregion

        public Application()
        {
            this.CreateDate = DateTime.Now;
            this.ApplicationStatus = ApplicationStatus.Draft;
            this.ApprovalCompleted += ApplicationApproval_Completed;
        }

        #region 持久化

        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = new PvbErmReponsitory())
            {
                #region 保存申请
                if (!repository.ReadTable<Applications>().Any(t => t.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Erm_Application);
                    this.VoteFlowID = GetVoteFlowID(this.ApplicationType, this.OffTimeContext?.Days);
                    repository.Insert(new Applications
                    {
                        ID = this.ID,
                        VoteFlowID = this.VoteFlowID,
                        Title = this.Title,
                        Context = this.Context,
                        ApplicantID = this.ApplicantID,
                        CreatorID = this.CreatorID,
                        CreateDate = this.CreateDate,
                        Status = (int)this.ApplicationStatus,
                    });
                    //初始化审批步骤
                    this.InitApplyVoteSteps(this);
                }
                else
                {
                    var voteFlowID = GetVoteFlowID(this.ApplicationType, this.OffTimeContext?.Days);
                    var isVoteFlowChange = this.VoteFlowID == voteFlowID ? false : true;

                    repository.Update<Applications>(new
                    {
                        VoteFlowID = voteFlowID,
                        Title = this.Title,
                        Context = this.Context,
                        ApplicantID = this.ApplicantID,
                        CreatorID = this.CreatorID,
                        Status = (int)this.ApplicationStatus,
                    }, item => item.ID == this.ID);

                    if (isVoteFlowChange)
                    {
                        this.VoteFlowID = voteFlowID;
                        //重新初始化审批步骤
                        this.InitApplyVoteSteps(this);
                    }
                }
                #endregion

                #region 保存文件
                //保存订单项
                var oldItems = new Views.ApplicationFileAlls(this.ID).ToList();
                SaveFiles(this.Fileitems, oldItems);
                #endregion
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = new PvbErmReponsitory())
            {
                repository.Update<Applications>(new
                {
                    Status = (int)ApplicationStatus.Delete,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 申请审批
        /// </summary>
        /// <param name="isAgress"></param>
        public void Approval(bool isAgress)
        {
            //审批意见
            string summary = this.CurrentVoteStep.Summary;
            if (isAgress)
            {
                //审批通过，调用下一步
                var result = Services.VoteFlowManager.Current.For(this).Next(summary);
                if (result)
                {
                    //TODO:审批完成后逻辑
                    if (this != null && ApprovalCompleted != null)
                    {
                        this.ApprovalCompleted(this, new SuccessEventArgs(this));
                    }
                }
            }
            else
            {
                //审批驳回
                Services.VoteFlowManager.Current.For(this).Reject(summary);

                using (var repository = new PvbErmReponsitory())
                {
                    repository.Update<Applications>(new
                    {
                        Status = (int)ApplicationStatus.Reject,
                    }, item => item.ID == this.ID);
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取审批流ID
        /// </summary>
        /// <param name="Type">申请类型</param>
        /// <param name="days">天数（请假专用）</param>
        /// <returns></returns>
        private string GetVoteFlowID(ApplicationType Type, decimal? days = null)
        {
            var voteFlow = Services.VoteFlowManager.Current[Type, days];
            return voteFlow.ID;
        }

        /// <summary>
        /// 初始化审批步骤
        /// </summary>
        /// <param name="application"></param>
        private void InitApplyVoteSteps(Application application)
        {
            Services.VoteFlowManager.Current.For(application).Init();
        }

        /// <summary>
        /// 保存申请文件
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveFiles(IEnumerable<CenterFileDescription> newFiles, IEnumerable<CenterFileDescription> oldFiles)
        {
            string[] newids = newFiles.Select(item => item.ID).ToArray();
            string[] oldids = oldFiles.Select(item => item.ID).ToArray();
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //删除原文件
                foreach (var id in oldids)
                {
                    if (!newids.Contains(id))
                    {
                        //删除原来的项
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            ErmApplicationID = "",
                            Status = FileDescriptionStatus.Delete,
                        }, item => item.ID == id && item.ErmApplicationID == this.ID);
                    }
                }
                //订单绑定新文件
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    ErmApplicationID = this.ID,
                }, item => newids.Contains(item.ID));
            }
        }

        /// <summary>
        /// 生成私有日程数据
        /// </summary>
        /// <param name="application"></param>
        private void ProductSchedulePrivate(Application application)
        {
            using (var repository = new PvbErmReponsitory())
            {
                //加班申请
                if (application.ApplicationType == ApplicationType.Overtime)
                {
                    //上午
                    var ID = PKeySigner.Pick(PKeyType.Sched);
                    repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                    {
                        ID = ID,
                        Date = application.OverTimeContext.Date,
                        Type = (int)Underly.Enums.ScheduleType.Private,
                        CreateDate = DateTime.Now,
                        CreatorID = application.CurrentVoteStep.AdminID,
                        ModifyDate = DateTime.Now,
                    });
                    repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                    {
                        ID = ID,
                        ApplicationID = application.ID,
                        Type = (int)Underly.Enums.SchedulePrivateType.Overtime,
                        AmOrPm = Underly.Enums.AmOrPm.Am.ToString(),
                        StaffID = application.Applicant.StaffID,
                        CreateDate = DateTime.Now,
                    });
                    //下午
                    var ID2 = PKeySigner.Pick(PKeyType.Sched);
                    repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                    {
                        ID = ID2,
                        Date = application.OverTimeContext.Date,
                        Type = (int)Underly.Enums.ScheduleType.Private,
                        CreateDate = DateTime.Now,
                        CreatorID = application.CurrentVoteStep.AdminID,
                        ModifyDate = DateTime.Now,
                    });
                    repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                    {
                        ID = ID2,
                        ApplicationID = application.ID,
                        Type = (int)Underly.Enums.SchedulePrivateType.Overtime,
                        AmOrPm = Underly.Enums.AmOrPm.Pm.ToString(),
                        StaffID = application.Applicant.StaffID,
                        CreateDate = DateTime.Now,
                    });
                }
                //补签申请
                if (application.ApplicationType == ApplicationType.ReSign)
                {
                    //上午
                    if (application.ReSignContext.AmOn == true || application.ReSignContext.AmOff == true)
                    {
                        var ID = PKeySigner.Pick(PKeyType.Sched);
                        repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                        {
                            ID = ID,
                            Date = application.ReSignContext.Date,
                            Type = (int)Underly.Enums.ScheduleType.Private,
                            CreateDate = DateTime.Now,
                            CreatorID = application.CurrentVoteStep.AdminID,
                            ModifyDate = DateTime.Now,
                        });
                        repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                        {
                            ID = ID,
                            ApplicationID = application.ID,
                            Type = (int)Underly.Enums.SchedulePrivateType.ReSign,
                            AmOrPm = Underly.Enums.AmOrPm.Am.ToString(),
                            StaffID = application.Applicant.StaffID,
                            OnWorkRemedy = application.ReSignContext.AmOn,
                            OffWorkRemedy = application.ReSignContext.AmOff,
                            CreateDate = DateTime.Now,
                        });
                    }
                    if (application.ReSignContext.PmOn == true || application.ReSignContext.PmOff == true)
                    {
                        //下午
                        var ID2 = PKeySigner.Pick(PKeyType.Sched);
                        repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                        {
                            ID = ID2,
                            Date = application.ReSignContext.Date,
                            Type = (int)Underly.Enums.ScheduleType.Private,
                            CreateDate = DateTime.Now,
                            CreatorID = application.CurrentVoteStep.AdminID,
                            ModifyDate = DateTime.Now,
                        });
                        repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                        {
                            ID = ID2,
                            ApplicationID = application.ID,
                            Type = (int)Underly.Enums.SchedulePrivateType.ReSign,
                            AmOrPm = Underly.Enums.AmOrPm.Pm.ToString(),
                            StaffID = application.Applicant.StaffID,
                            OnWorkRemedy = application.ReSignContext.PmOn,
                            OffWorkRemedy = application.ReSignContext.PmOff,
                            CreateDate = DateTime.Now,
                        });
                    }
                }
                //请假申请
                if (application.ApplicationType == ApplicationType.Offtime)
                {
                    var DateItems = application.OffTimeContext.DateItems;
                    foreach (var date in DateItems)
                    {
                        if (date.Type == DateLengthType.AllDay)
                        {
                            //上午
                            var ID = PKeySigner.Pick(PKeyType.Sched);
                            repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                            {
                                ID = ID,
                                Date = date.Date,
                                Type = (int)Underly.Enums.ScheduleType.Private,
                                CreateDate = DateTime.Now,
                                CreatorID = application.CurrentVoteStep.AdminID,
                                ModifyDate = DateTime.Now,
                            });
                            repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                            {
                                ID = ID,
                                ApplicationID = application.ID,
                                Type = (int)application.OffTimeContext.LeaveType,
                                AmOrPm = Underly.Enums.AmOrPm.Am.ToString(),
                                StaffID = application.Applicant.StaffID,
                                CreateDate = DateTime.Now,
                            });
                            //下午
                            var ID2 = PKeySigner.Pick(PKeyType.Sched);
                            repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                            {
                                ID = ID2,
                                Date = date.Date,
                                Type = (int)Underly.Enums.ScheduleType.Private,
                                CreateDate = DateTime.Now,
                                CreatorID = application.CurrentVoteStep.AdminID,
                                ModifyDate = DateTime.Now,
                            });
                            repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                            {
                                ID = ID2,
                                ApplicationID = application.ID,
                                Type = (int)application.OffTimeContext.LeaveType,
                                AmOrPm = Underly.Enums.AmOrPm.Pm.ToString(),
                                StaffID = application.Applicant.StaffID,
                                CreateDate = DateTime.Now,
                            });
                        }
                        else if (date.Type == DateLengthType.Morning)
                        {
                            //上午
                            var ID = PKeySigner.Pick(PKeyType.Sched);
                            repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                            {
                                ID = ID,
                                Date = date.Date,
                                Type = (int)Underly.Enums.ScheduleType.Private,
                                CreateDate = DateTime.Now,
                                CreatorID = application.CurrentVoteStep.AdminID,
                                ModifyDate = DateTime.Now,
                            });
                            repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                            {
                                ID = ID,
                                ApplicationID = application.ID,
                                Type = (int)application.OffTimeContext.LeaveType,
                                AmOrPm = Underly.Enums.AmOrPm.Am.ToString(),
                                StaffID = application.Applicant.StaffID,
                                CreateDate = DateTime.Now,
                            });
                        }
                        else
                        {
                            //下午
                            var ID2 = PKeySigner.Pick(PKeyType.Sched);
                            repository.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                            {
                                ID = ID2,
                                Date = date.Date,
                                Type = (int)Underly.Enums.ScheduleType.Private,
                                CreateDate = DateTime.Now,
                                CreatorID = application.CurrentVoteStep.AdminID,
                                ModifyDate = DateTime.Now,
                            });
                            repository.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                            {
                                ID = ID2,
                                ApplicationID = application.ID,
                                Type = (int)application.OffTimeContext.LeaveType,
                                AmOrPm = Underly.Enums.AmOrPm.Pm.ToString(),
                                StaffID = application.Applicant.StaffID,
                                CreateDate = DateTime.Now,
                            });
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 申请的审批日志
    /// </summary>
    public class Logs_ApplyVoteStep : IUnique
    {
        public string ID { get; set; }

        public string ApplicationID { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public string VoteStepID { get; set; }

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

        /// <summary>
        /// 审批人扩展
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string VoteStepName { get; set; }
    }
}