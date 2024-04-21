using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Offtime
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        protected void LoadComboBoxData()
        {
            var staffs = Erp.Current.Erm.XdtStaffs
               .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);

            var staff = staffs.SingleOrDefault(item => item.ID == Erp.Current.StaffID);
            if (staff != null && staff.PostionCode != ((int)Services.Common.PostType.President).ToString())
            {
                staffs = staffs.Where(item => item.DepartmentCode == staff.DepartmentCode);
            }
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
            //负责人员
            this.Model.ManageData = staffs.Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString())
                .Select(item => new
                {
                    Value = item.Admin.ID,
                    Text = item.Name
                });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //假期类型
            this.Model.SchedulePrivateType = ExtendsEnum.ToDictionary<Underly.Enums.LeaveType>().Select(item => new
            {
                Value = item.Key,
                Text = item.Value
            });
            //请假时长
            this.Model.DateType = ExtendsEnum.ToDictionary<DateLengthType>().Select(item => new { Value = item.Key, Text = item.Value });
            //出差原因
            this.Model.BusinessTripReason = ExtendsEnum.ToArray<BusinessTripReason>().Select(item => new
            {
                Value = (int)item,
                Text = item.GetDescription()
            });
            //是否借款
            this.Model.LoanOrNot = ExtendsEnum.ToArray<LoanOrNot>().Select(item => new
            {
                Value = (int)item,
                Text = item.GetDescription()
            });
        }

        protected void LoadData()
        {
            //申请信息
            string ApplicationID = Request.QueryString["ID"];
            this.Model.ApplicationData = null;
            if (!string.IsNullOrEmpty(ApplicationID))
            {
                var application = Erp.Current.Erm.Applications.Single(item => item.ID == ApplicationID);
                this.Model.ApplicationData = new
                {
                    ApplicantID = application.ApplicantID,
                    Type = (int)application.OffTimeContext.LeaveType,
                    SwapStaff = application.OffTimeContext.SwapStaff,
                    WorkContext = application.OffTimeContext.WorkContext,
                    Reason = application.OffTimeContext.Reason,
                    BusinessReason = application.OffTimeContext.BusinessReason,
                    Entourage = application.OffTimeContext.Entourage,
                    LoanOrNot = application.OffTimeContext.LoanOrNot
                };
            }
        }

        protected void StaffChange()
        {
            try
            {
                var staffs = Erp.Current.Erm.XdtStaffs
                    .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
                //员工姓名
                string Name = Request.Form["Name"];
                var staff = staffs.Single(item => item.Admin.ID == Name);
                var YearsDay = staff?.VacationItems.Where(item => item.Type == VacationType.YearsDay).Sum(item => item.Lefts);
                var OffDay = staff?.VacationItems.Where(item => item.Type == VacationType.OffDay).Sum(item => item.Lefts);
                //唯一负责人
                var manager = staffs.Where(item => item.DepartmentCode == staff.DepartmentCode)
                    .Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString()).FirstOrDefault();
                var data = new
                {
                    department = (int)((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)),
                    manager = manager?.Admin.ID,
                    code = staff?.Code,
                    YearsDay = YearsDay,
                    OffDay = OffDay,
                };
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.OfftimeApplication);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
                                FileType = dic.FileType,
                                FileTypeDec = dic.FileType.GetDescription(),
                                Url = dic.uploadResult.Url,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["ID"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).Where(item => item.Type == (int)FileType.OfftimeApplication).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileType = ((FileType)Enum.Parse(typeof(FileType), t.Type.ToString())).GetDescription(),
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }

        /// <summary>
        /// 请假日期
        /// </summary>
        /// <returns></returns>
        protected object LoadDate()
        {
            var id = Request.QueryString["ID"];
            var dates = Erp.Current.Erm.Applications[id]?.OffTimeContext?.DateItems;
            var data = dates.Select(item => new
            {
                Date = item.Date.ToString("yyyy-MM-dd"),
                Type = item.Type,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            var id = Request.QueryString["ID"];
            var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.ApplicationID == id);

            var data = logs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.Admin.RealName,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                item.VoteStepName,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }

        /// <summary>
        /// 行程信息
        /// </summary>
        /// <returns></returns>
        protected object LoadSchedule()
        {
            var id = Request.QueryString["ID"];
            var dates = Erp.Current.Erm.Applications[id]?.OffTimeContext?.ScheduleItems;
            var data = dates.Select(item => new
            {
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                StartPlace = item.StartPlace,
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                EndPlace = item.EndPlace,
                Vehicle = item.Vehicle,
                VehicleCost = item.VehicleCost,
                StayDay = item.StayDay,
                CompanyName = item.CompanyName,
                Person = item.Person,
                Department = item.Department,
                Position = item.Position,
                Phone = item.Phone,
                BusinessReason = item.BusinessReason,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }

        /// <summary>
        /// 提交
        /// </summary>
        protected void Submit()
        {
            try
            {
                #region 界面数据
                string id = Request.Form["ID"];
                string AdminID = Request.Form["Staff"];
                string StaffCode = Request.Form["StaffCode"];
                string Manager = Request.Form["Manager"];
                string ManagerName = Request.Form["ManagerName"];
                string SwapStaff = Request.Form["SwapStaff"];
                string WorkContext = Request.Form["WorkContext"];
                string Reason = Request.Form["Reason"];
                var businessReason = Request.Form["BusinessReason"];
                BusinessTripReason? BusinessReason = string.IsNullOrEmpty(businessReason) ? null : (BusinessTripReason?)Enum.Parse(typeof(BusinessTripReason), businessReason);
                string Entourage = Request.Form["Entourage"];
                var loanOrNot = Request.Form["LoanOrNot"];
                LoanOrNot LoanOrNot = string.IsNullOrEmpty(loanOrNot) ? LoanOrNot.Not : (LoanOrNot)Enum.Parse(typeof(LoanOrNot), loanOrNot);
                LeaveType Type = (LeaveType)Enum.Parse(typeof(LeaveType), Request.Form["Type"]);
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                //请假日期
                var dates = Request.Form["dates"].Replace("&quot;", "'").Replace("amp;", "");
                var dateList = dates.JsonTo<List<OffTimeDateItem>>();
                //日程安排
                var schedules = Request.Form["schedules"].Replace("&quot;", "'").Replace("amp;", "");
                var scheduleList = schedules.JsonTo<List<OffTimeScheduleItem>>();
                #endregion

                #region 验证是否可以请假
                if (Type == LeaveType.OfficialBusiness || Type == LeaveType.BusinessTrip || Type == LeaveType.AnnualLeave
                    || Type == LeaveType.CasualLeave || Type == LeaveType.SickLeave || Type == LeaveType.LeaveInLieu)
                {
                    var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == AdminID);
                    var vocationDate = Erp.Current.Erm.SchedulesPublic
                        .Where(item => item.ShiftID == staff.SchedulingID && item.RegionID == staff.RegionID)
                        .Where(item => item.Method != Underly.Enums.ScheduleMethod.Work)
                        .Select(item => item.Date).ToArray();
                    var applyDate = dateList.Select(item => item.Date).ToArray();
                    foreach (var date in applyDate)
                    {
                        if (vocationDate.Contains(date))
                        {
                            throw new Exception(date + "为公休日时间，不能请假!");
                        }
                    }
                }
                #endregion

                #region 验证员工的假期是否足够
                decimal day1 = dateList.Where(item => item.Type == DateLengthType.AllDay).Count();
                decimal day2 = dateList.Where(item => item.Type != DateLengthType.AllDay).Count();
                var days = day1 + day2 / 2;

                var Staff = Alls.Current.Staffs.SingleOrDefault(item => item.Admin.ID == AdminID);
                if (Type == LeaveType.AnnualLeave)
                {
                    var YearsDay = Staff?.VacationItems.SingleOrDefault(item => item.Type == VacationType.YearsDay)?.Lefts;
                    if (YearsDay == null || days > YearsDay)
                    {
                        throw new Exception("请假失败，年假天数不足");
                    }
                }
                if (Type == LeaveType.LeaveInLieu)
                {
                    var OffDay = Staff?.VacationItems.SingleOrDefault(item => item.Type == VacationType.OffDay)?.Lefts;
                    if (OffDay == null || days > OffDay)
                    {
                        throw new Exception("请假失败，调休假天数不足");
                    }
                }
                if (Type == LeaveType.SickLeave)
                {
                    var SickDay = Staff?.VacationItems.SingleOrDefault(item => item.Type == VacationType.SickDay)?.Lefts;
                    if (SickDay == null || days > SickDay)
                    {
                        throw new Exception("请假失败，病假天数不足");
                    }
                }
                //if (Type == LeaveType.ProductionInspectionLeave)
                //{
                //    var ProductionInspectionDay = Staff?.VacationItems
                //        .SingleOrDefault(item => item.Type == VacationType.ProductionInspectionDay && item.StartTime >= DateTime.Now && item.EndTime <= DateTime.Now)?.Lefts;
                //    if (ProductionInspectionDay == null || days > ProductionInspectionDay)
                //    {
                //        throw new Exception("请假失败，产检假天数不足");
                //    }
                //}
                #endregion

                #region 验证是否已经请过假
                var contexts = Erp.Current.Erm.Applications
                    .Where(item => item.ApplicantID == AdminID && item.ApplicationType == Services.ApplicationType.Offtime && item.ID != id).ToArray()
                    .Select(item => item.OffTimeContext);
                foreach (var date in dateList)
                {
                    foreach (var context in contexts)
                    {
                        foreach (var dateitem in context.DateItems)
                        {
                            if (date.Type == DateLengthType.AllDay)
                            {
                                if (dateitem.Date == date.Date)
                                {
                                    throw new Exception(date.Date.ToShortDateString() + "已经请假，不能重复申请");
                                }
                            }
                            else if (date.Type == DateLengthType.Morning)
                            {
                                if (dateitem.Date == date.Date && dateitem.Type != DateLengthType.Afternoon)
                                {
                                    throw new Exception(date.Date.ToShortDateString() + "上午已经请假，不能重复申请");
                                }
                            }
                            else
                            {
                                if (dateitem.Date == date.Date && dateitem.Type != DateLengthType.Morning)
                                {
                                    throw new Exception(date.Date.ToShortDateString() + "下午已经请假，不能重复申请");
                                }
                            }
                        }
                    }
                }
                #endregion

                Application application = Erp.Current.Erm.Applications[id] ?? new Application();
                application.Title = "请假申请";
                application.Context = new OffTimeContext()
                {
                    LeaveType = Type,
                    ApproveID = Manager,
                    ApproveName = ManagerName,
                    DepartmentCode = Staff.DepartmentCode,
                    SwapStaff = SwapStaff,
                    WorkContext = WorkContext,
                    Reason = Reason,
                    BusinessReason = BusinessReason,
                    Entourage = Entourage,
                    LoanOrNot = LoanOrNot,
                    DateItems = dateList,
                    ScheduleItems = scheduleList,
                }.Json();
                application.ApplicantID = AdminID;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.UnderApproval;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.Offtime;

                application.Enter();

                #region 扣除员工假期

                Staff.DeductVacation(Type, days);

                #endregion

                //如果是部门负责人
                if (Staff.PostionCode == ((int)Services.Common.PostType.Manager).ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[application.ID];
                    apply.Approval(true);
                }
                //如果是总经理
                if (Staff.PostionCode == ((int)Services.Common.PostType.President).ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[application.ID];
                    if (days > 3)
                    {
                        apply.Approval(true);
                        apply.Approval(true);
                    }
                    else
                    {
                        apply.Approval(true);
                    }
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}