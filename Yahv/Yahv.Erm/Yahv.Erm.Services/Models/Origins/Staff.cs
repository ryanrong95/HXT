using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using Layers.Data;
using Yahv.Underly;
using System.Collections.Generic;
using Yahv.Services.Models;
using Yahv.Services.Views;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using Yahv.Erm.Services.Common;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class Staff : IUnique
    {
        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 员工编码,全局唯一（例如：0001）
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 统一编码,全局唯一(自定义编码)
        /// </summary>
        public string SelCode { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 是内部公司分类的方法 采用大赢家的编号
        /// </summary>
        public string DyjCompanyCode { get; set; }

        /// <summary>
        /// 是内部公司下部门分类的方法 采用大赢家的编号
        /// </summary>
        public string DyjDepartmentCode { get; set; }

        /// <summary>
        /// 组织ID （只做合同关系的）
        /// </summary>
        public string LeagueID { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 员工部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 员工职务编码
        /// </summary>
        public string PostionCode { get; set; }

        /// <summary>
        /// 执行考核办法（把字段列在页面中，并标注：待开发）
        /// </summary>
        public string AssessmentMethod { get; set; }

        /// <summary>
        /// 执行考核时间（把字段列在页面中，并标注：待开发）
        /// </summary>
        public string AssessmentTime { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 正常、离职、停用
        /// </summary>
        public StaffStatus Status { get; set; }

        public string RegionID { get; set; }

        public string SchedulingID { get; set; }

        #endregion

        #region 扩展属性

        public Personal Personal { get; set; }

        public Admin Admin { get; set; }

        public Labour Labour { get; set; }

        public Postion Postion { get; set; }

        public League City { get; set; }

        /// <summary>
        /// 员工假期
        /// </summary>
        IEnumerable<Vacation> vacationItems;
        public IEnumerable<Vacation> VacationItems
        {
            get
            {
                if (this.vacationItems == null)
                {
                    this.vacationItems = new Views.Origins.VacationsOrigin().Where(item => item.StaffID == this.ID);
                }
                return this.vacationItems;
            }
            set
            {
                this.vacationItems = value;
            }
        }

        public decimal? YearsDay
        {
            get
            {
                return this.VacationItems.Where(item => item.Type == VacationType.YearsDay).Sum(item => item.Lefts);
            }
        }
        public decimal? OffDay
        {
            get
            {
                return this.VacationItems.Where(item => item.Type == VacationType.OffDay).Sum(item => item.Lefts);
            }
        }
        public decimal? SickDay
        {
            get
            {
                return this.VacationItems.Where(item => item.Type == VacationType.SickDay).Sum(item => item.Lefts);
            }
        }
        public decimal? ProductionInspectionDay
        {
            get
            {
                return this.VacationItems.Where(item => item.Type == VacationType.ProductionInspectionDay
                && DateTime.Now > item.StartTime
                && DateTime.Now < item.EndTime).Sum(item => item.Lefts);
            }
        }

        /// <summary>
        /// 员工附件
        /// </summary>
        IEnumerable<CenterFileDescription> fileitems;
        public IEnumerable<CenterFileDescription> Fileitems
        {
            get
            {
                if (this.fileitems == null)
                {
                    this.fileitems = new CenterFilesTopView()
                        .Where(item => item.StaffID == this.ID)
                        .Where(item => item.Status != FileDescriptionStatus.Delete);
                }
                return this.fileitems;
            }
            set
            {
                this.fileitems = value;
            }
        }

        /// <summary>
        /// 最近调查时间
        /// </summary>
        public DateTime? LastBackgroundCheck { get; set; }

        /// <summary>
        /// 班别ID
        /// </summary>
        public string WorkingClassID { get; set; }

        /// <summary>
        /// 登入名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 登入密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? EntryDate { get; set; }

        /// <summary>
        /// 离职时间 可空 空表示未离职，有代表离职
        /// </summary>
        public DateTime? LeaveDate { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string WorkCity { get; set; }

        /// <summary>
        /// 城市名
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 所属企业（合同所属企业，Crm中的内部公司）
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 所属企业名（合同所属企业，Crm中的内部公司）
        /// </summary>
        public string EnterpriseCompany { get; set; }

        /// <summary>
        /// 岗位名
        /// </summary>
        public string PostionName { get; set; }

        /// <summary>
        /// 银行卡账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        #endregion

        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        public Staff()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = StaffStatus.UnApplied;
            this.DepartmentCode = DepartmentType.关务部.GetHashCode().ToString();
            this.PostionCode = PostType.Staff.GetHashCode().ToString();
        }

        #region 持久化

        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<Staffs>().Any(t => t.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Staff);
                    //添加
                    repository.Insert(new Staffs()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Code = this.ID.Replace("Staff", ""),
                        SelCode = this.ID.Replace("Staff", ""),
                        Gender = (int)this.Gender,
                        DyjCompanyCode = this.DyjCompanyCode,
                        DyjDepartmentCode = this.DyjDepartmentCode,
                        DyjCode = this.DyjCode,
                        WorkCity = this.WorkCity,
                        LeagueID = this.LeagueID,
                        PostionID = this.PostionID,
                        DepartmentCode = this.DepartmentCode,
                        PostionCode = this.PostionCode,
                        AssessmentMethod = this.AssessmentMethod,
                        AssessmentTime = this.AssessmentTime,
                        AdminID = this.AdminID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Status = (int)this.Status,
                        RegionID = this.RegionID,
                        SchedulingID = this.SchedulingID,
                    });
                }
                else
                {
                    //修改
                    repository.Update<Staffs>(new
                    {
                        SelCode = this.SelCode,
                        Name = this.Name,
                        Gender = (int)this.Gender,
                        DyjCompanyCode = this.DyjCompanyCode,
                        DyjDepartmentCode = this.DyjDepartmentCode,
                        DyjCode = this.DyjCode,
                        WorkCity = this.WorkCity,
                        LeagueID = this.LeagueID,
                        PostionID = this.PostionID,
                        DepartmentCode = this.DepartmentCode,
                        PostionCode = this.PostionCode,
                        AssessmentMethod = this.AssessmentMethod,
                        AssessmentTime = this.AssessmentTime,
                        AdminID = this.AdminID,
                        UpdateDate = DateTime.Now,
                        Status = (int)this.Status,
                        RegionID = this.RegionID,
                        SchedulingID = this.SchedulingID,
                    }, t => t.ID == this.ID);
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                {
                    Status = StaffStatus.Delete
                }, item => item.ID == this.ID);
                //删除日志
                repository.Delete<Layers.Data.Sqls.PvbErm.Logs_StaffApprovals>(item => item.StaffID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Cancel()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                {
                    Status = StaffStatus.Cancel
                }, item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 面试结果
        /// </summary>
        public void Interview(bool result)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (result)
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                    {
                        Status = StaffStatus.InterviewPass
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                    {
                        Status = StaffStatus.InterviewFail
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 申请入职
        /// </summary>
        public void Apply(string AdminID = "")
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //更新入职日期为申请入职的当前日期
                repository.Update<Layers.Data.Sqls.PvbErm.Labours>(new
                {
                    EntryDate = DateTime.Now.Date,
                    ContractPeriod = DateTime.Now.Date.AddYears(3),
                    ProbationMonths = "3"
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 转正
        /// </summary>
        public void TurnNormal()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                {
                    Status = StaffStatus.Normal,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 入职审批
        /// </summary>
        /// <param name="IsPass">是否通过</param>
        public void Approval(bool IsPass, string adminID = "")
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (IsPass)
                {
                    //判断试用期时长
                    var Duration = this.Labour?.ProbationMonths;
                    if (!string.IsNullOrEmpty(Duration) && decimal.Parse(Duration) > 0m)
                    {
                        repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                        {
                            //试用期
                            Status = StaffStatus.Period,
                        }, item => item.ID == this.ID);
                    }
                    else
                    {
                        repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                        {
                            //在职
                            Status = StaffStatus.Normal,
                        }, item => item.ID == this.ID);
                    }
                }

                //更新入职日志
                var log = new Views.Alls.Logs_StaffApprovalAll().Single(item => item.ApprovalStep == StaffApprovalStep.Entry && item.StaffID == this.ID);
                log.EntryReportStatus = IsPass ? StaffEntryReportStatus.Entry : StaffEntryReportStatus.WaitingReport;
                log.AdminID = adminID;
                log.Enter();
            }
        }

        /// <summary>
        /// 离职
        /// </summary>
        public void Departure(DateTime date)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                {
                    Status = StaffStatus.Departure
                }, item => item.ID == this.ID);

                repository.Update<Layers.Data.Sqls.PvbErm.Labours>(new
                {
                    LeaveDate = date
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 员工假期初始化
        /// </summary>
        public void InitVacation()
        {
            int years = DateTime.Now.Year - ((DateTime)this.Personal.BeginWorkDate).Year;
            int month = ((DateTime)this.Personal.BeginWorkDate).Month;//开始工作的月份
            double rate = 5.0 / 12.0;
            double yearsday;
            if (years == 0)
            {
                yearsday = 0;
            }
            else if (years == 1)
            {
                if ((DateTime.Now.Month - month) >= 0)
                {
                    //刚满一年的按比例分配年假天数
                    yearsday = rate * (12 - month + 1);
                }
                else
                {
                    yearsday = 0;
                }
            }
            else if (years > 1 && years < 10)
            {
                yearsday = 5;
            }
            else if (years == 10)
            {
                if ((DateTime.Now.Month - month) >= 0)
                {
                    //刚满10年的按比例分配年假天数
                    yearsday = rate * (12 - month + 1) + 5;
                }
                else
                {
                    yearsday = 5;
                }
            }
            else if (years > 10 && years < 20)
            {
                yearsday = 10;
            }
            else if (years == 20)
            {
                if ((DateTime.Now.Month - month) >= 0)
                {
                    //刚满20年的按比例分配年假天数
                    yearsday = rate * (12 - month + 1) + 10;
                }
                else
                {
                    yearsday = 10;
                }
            }
            else
            {
                yearsday = 15;
            }

            using (var repository = new PvbErmReponsitory())
            {
                List<Vacations> list = new List<Vacations>();

                var origins = new Views.Origins.VacationsOrigin().Where(item => item.StaffID == this.ID);
                var year = origins.SingleOrDefault(item => item.Type == VacationType.YearsDay);
                if (year == null)
                {
                    list.Add(new Vacations
                    {
                        ID = PKeySigner.Pick(PKeyType.Vacation),
                        StaffID = this.ID,
                        Type = (int)VacationType.YearsDay,
                        Lefts = (decimal)Math.Floor(yearsday),
                        Total = (decimal)Math.Floor(yearsday),
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Vacations>(new
                    {
                        Lefts = (decimal)Math.Floor(yearsday),
                        Total = (decimal)Math.Floor(yearsday),
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == year.ID);
                }
                var sick = origins.SingleOrDefault(item => item.Type == VacationType.SickDay);
                if (sick == null)
                {
                    list.Add(new Vacations
                    {
                        ID = PKeySigner.Pick(PKeyType.Vacation),
                        StaffID = this.ID,
                        Type = (int)VacationType.SickDay,
                        Lefts = 30m,
                        Total = 30m,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Vacations>(new
                    {
                        Lefts = 30m,
                        Total = 30m,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == sick.ID);
                }

                var off = origins.SingleOrDefault(item => item.Type == VacationType.OffDay);
                if (off == null)
                {
                    list.Add(new Vacations
                    {
                        ID = PKeySigner.Pick(PKeyType.Vacation),
                        StaffID = this.ID,
                        Type = (int)VacationType.OffDay,
                        Lefts = 0m,
                        Total = 0m,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Vacations>(new
                    {
                        Lefts = 0m,
                        Total = 0m,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == off.ID);
                }

                var productions = origins.Where(item => item.Type == VacationType.ProductionInspectionDay);
                if (productions.Count() == 0)
                {
                    list.Add(new Vacations
                    {
                        ID = PKeySigner.Pick(PKeyType.Vacation),
                        StaffID = this.ID,
                        Type = (int)VacationType.ProductionInspectionDay,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        Lefts = 0m,
                        Total = 0m,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    var ids = productions.Select(item => item.ID).ToArray();
                    repository.Update<Vacations>(new
                    {
                        Lefts = 0m,
                        Total = 0m,
                        UpdateDate = DateTime.Now,
                    }, item => ids.Contains(item.ID));
                }
                repository.Insert(list.ToArray());
            }
        }

        /// <summary>
        /// 扣除员工假期（请假申请提交时调用）
        /// </summary>
        /// <param name="type">请假类型</param>
        /// <param name="days">假期天数</param>
        public void DeductVacation(Underly.Enums.LeaveType type, decimal days)
        {
            using (var repository = new PvbErmReponsitory())
            {
                Vacation vacation = new Vacation();
                if (type == Underly.Enums.LeaveType.AnnualLeave)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.YearsDay);
                }
                if (type == Underly.Enums.LeaveType.LeaveInLieu)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.OffDay);
                }
                if (type == Underly.Enums.LeaveType.SickLeave)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.SickDay);
                }
                //if (type == Underly.Enums.LeaveType.ProductionInspectionLeave)
                //{
                //    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.ProductionInspectionDay &&
                //    item.StartTime >= DateTime.Now && item.EndTime < DateTime.Now);
                //}
                if (vacation != null)
                {
                    decimal lefts = vacation.Lefts - days <= 0m ? 0m : vacation.Lefts - days;
                    //更新假期
                    repository.Update<Layers.Data.Sqls.PvbErm.Vacations>(new
                    {
                        Lefts = lefts,
                    }, item => item.ID == vacation.ID);
                }
            }
        }

        /// <summary>
        /// 还回员工假期（请假审批驳回时调用）
        /// </summary>
        /// <param name="type">请假类型</param>
        /// <param name="days">假期天数</param>
        public void ReturnVacation(Underly.Enums.LeaveType type, decimal days)
        {
            using (var repository = new PvbErmReponsitory())
            {
                Vacation vacation = new Vacation();
                if (type == Underly.Enums.LeaveType.AnnualLeave)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.YearsDay);
                }
                if (type == Underly.Enums.LeaveType.LeaveInLieu)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.OffDay);
                }
                if (type == Underly.Enums.LeaveType.SickLeave)
                {
                    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.SickDay);
                }
                //if (type == Underly.Enums.LeaveType.ProductionInspectionLeave)
                //{
                //    vacation = this.VacationItems.SingleOrDefault(item => item.Type == VacationType.ProductionInspectionDay);
                //}
                //更新假期
                repository.Update<Layers.Data.Sqls.PvbErm.Vacations>(new
                {
                    Lefts = vacation.Lefts + days,
                }, item => item.ID == vacation.ID);
            }
        }

        /// <summary>
        /// 导出应聘人员登记表
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="TempletePath">模板路径</param>
        public void ExportEmploymentForm(string filePath, string TempletePath)
        {
            //创建数据字典
            var info = new Dictionary<object, int[]>();
            info.Add("  填表日期：" + this.CreateDate.Year + " 年 " + this.CreateDate.Month + " 月 " + this.CreateDate.Day + " 日 " + "2=0", new int[] { 2, 0 });
            info.Add(this.Name + "3=1", new int[] { 3, 1 });
            info.Add(this.Gender.GetDescription() + "3=3", new int[] { 3, 3 });
            info.Add(this.Personal.IDCard + "3=5", new int[] { 3, 5 });

            info.Add(this.Personal.Volk + "4=1", new int[] { 4, 1 });
            info.Add(this.Personal.PoliticalOutlook + "4=3", new int[] { 4, 3 });
            info.Add(this.Personal.IsMarry == true ? "是" : "否" + "4=5", new int[] { 4, 5 });
            info.Add(this.Personal.Age + "4=7", new int[] { 4, 7 });

            info.Add(this.Personal.Height + "5=1", new int[] { 5, 1 });
            info.Add(this.Personal.Weight + "5=3", new int[] { 5, 3 });
            info.Add(this.Personal.Healthy + "5=5", new int[] { 5, 5 });
            info.Add(this.Personal.NativePlace + "5=7", new int[] { 5, 7 });

            info.Add("  " + this.Personal.PassAddress + "6=1", new int[] { 6, 1 });
            info.Add("  " + this.Personal.HomeAddress + "7=1", new int[] { 7, 1 });
            info.Add(this.Personal.GraduatInstitutions + "8=1", new int[] { 8, 1 });
            info.Add(this.Personal.GraduationDate?.ToString("yyyy-MM-dd") + "8=6", new int[] { 8, 6 });

            info.Add(this.Personal.Education + "9=1", new int[] { 9, 1 });
            info.Add(this.Personal.Major + "9=3", new int[] { 9, 3 });
            info.Add(this.Personal.LanguageLevel + "9=6", new int[] { 9, 6 });

            info.Add(this.Personal.ComputerLevel + "10=1", new int[] { 10, 1 });
            info.Add(this.Personal.Email + "10=3", new int[] { 10, 3 });
            info.Add(this.Personal.Mobile + "10=6", new int[] { 10, 6 });

            info.Add("  " + this.Personal.SelfEvaluation + "11=1", new int[] { 11, 1 });
            info.Add(this.Personal.PositionName + "12=1", new int[] { 12, 1 });
            info.Add(this.Personal.Treatment + "12=3", new int[] { 12, 3 });
            info.Add(this.Personal.BeginWorkDate?.ToString("yyyy-MM-dd") + "12=6", new int[] { 12, 6 });

            var works = this.Personal.Workitems.ToArray();
            for (int i = 15; i < 19 && i < works.Length + 15; i++)
            {
                var work = works[i - 15];
                info.Add(work.StartTime.ToShortDateString().Replace("-", "") + "-" + work.EndTime.ToShortDateString().Replace("-", "") + i + "=0", new int[] { i, 0 });
                info.Add(work.Company + i + "=1", new int[] { i, 1 });
                info.Add(work.Position + i + "=4", new int[] { i, 4 });
                info.Add(work.Salary?.ToString("0.00") + i + "=5", new int[] { i, 5 });
                info.Add(work.LeaveReason + i + "=6", new int[] { i, 6 });
            }

            var familys = this.Personal.Familyitems.ToArray();
            for (int i = 21; i < 25 && i < familys.Length + 21; i++)
            {
                var family = familys[i - 21];
                info.Add(family.Name + i + "=0", new int[] { i, 0 });
                info.Add(family.Relation + i + "=1", new int[] { i, 1 });
                info.Add(family.Age.ToString() + i + "=2", new int[] { i, 2 });
                info.Add(family.Company + i + "=3", new int[] { i, 3 });
                info.Add(family.Phone + i + "=6", new int[] { i, 6 });
            }

            //获取模板
            FileStream file = new FileStream(TempletePath, FileMode.Open, FileAccess.Read);
            //用模板创建对象
            IWorkbook xssfworkbook = new XSSFWorkbook(file);
            //获取模板中的Sheet
            ISheet sheet1 = xssfworkbook.GetSheet("应聘人员登记表");

            #region 创建样式（目前没用到）
            //XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrap.WrapText = true;

            //IFont font1 = xssfworkbook.CreateFont();//字体
            //font1.FontHeightInPoints = 9;
            //XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrapandfont.WrapText = true;
            //stylewrapandfont.SetFont(font1);

            //XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            //stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));
            #endregion

            //填充数据到表格
            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //保存新文件
            using (FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xssfworkbook.Write(newFile);
                newFile.Close();
            }
        }

        /// <summary>
        /// 导出面试情况评估表
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="TempletePath">模板路径</param>
        public void ExportInterviewEvaluation(string filePath, string TempletePath)
        {
            //创建数据字典
            var info = new Dictionary<object, int[]>();
            info.Add(this.Name + "1=1", new int[] { 1, 1 });
            info.Add(this.Personal.PositionName + "1=3", new int[] { 1, 3 });
            info.Add(this.Personal.Treatment + "1=5", new int[] { 1, 5 });
            info.Add(this.CreateDate.ToShortDateString() + "1=7", new int[] { 1, 7 });

            //获取模板
            FileStream file = new FileStream(TempletePath, FileMode.Open, FileAccess.Read);
            //用模板创建对象
            IWorkbook xssfworkbook = new XSSFWorkbook(file);
            //获取模板中的Sheet
            ISheet sheet1 = xssfworkbook.GetSheet("面试情况评估表");

            #region 创建样式（目前没用到）
            //XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrap.WrapText = true;

            //IFont font1 = xssfworkbook.CreateFont();//字体
            //font1.FontHeightInPoints = 9;
            //XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrapandfont.WrapText = true;
            //stylewrapandfont.SetFont(font1);

            //XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            //stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));
            #endregion

            //填充数据到表格
            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //保存新文件
            using (FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xssfworkbook.Write(newFile);
                newFile.Close();
            }
        }

        /// <summary>
        /// 员工背景调查报告
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="TempletePath">模板路径</param>
        public void ExportBackgroundInvestigation(string filePath, string TempletePath)
        {
            //创建数据字典
            var info = new Dictionary<object, int[]>();
            info.Add(this.Name + "1=1", new int[] { 1, 1 });
            info.Add(this.Gender.GetDescription() + "1=3", new int[] { 1, 3 });
            info.Add(this.Personal.Age + "1=5", new int[] { 1, 5 });
            info.Add(this.Personal.BirthDate?.ToString("yyyy-MM-dd") + "1=7", new int[] { 1, 7 });

            info.Add(this.Personal.NativePlace + "2=1", new int[] { 2, 1 });
            info.Add(this.Personal.Volk + "2=3", new int[] { 2, 3 });
            info.Add(this.Personal.IsMarry == true ? "是" : "否" + "2=5", new int[] { 2, 5 });
            info.Add(this.Personal.Education + "2=7", new int[] { 2, 7 });

            info.Add(string.IsNullOrEmpty(this.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription() + "3=1", new int[] { 3, 1 });
            info.Add(string.IsNullOrEmpty(this.PostionCode) ? "" : ((PostType)Enum.Parse(typeof(PostType), this.PostionCode)).GetDescription() + "3=3", new int[] { 3, 3 });
            info.Add(this.Labour.EntryDate.ToString("yyyy-MM-dd") + "3=5", new int[] { 3, 5 });
            info.Add(DateTime.Now.ToString("yyyy-MM-dd") + "3=7", new int[] { 3, 7 });

            info.Add(this.Personal.IDCard + "4=1", new int[] { 4, 1 });
            info.Add(this.Personal.Mobile + "4=5", new int[] { 4, 5 });

            info.Add(this.Personal.PassAddress + "5=1", new int[] { 5, 1 });
            info.Add(this.Personal.HomeAddress + "5=5", new int[] { 5, 5 });

            //获取模板
            FileStream file = new FileStream(TempletePath, FileMode.Open, FileAccess.Read);
            //用模板创建对象
            IWorkbook xssfworkbook = new XSSFWorkbook(file);
            //获取模板中的Sheet
            ISheet sheet1 = xssfworkbook.GetSheet("员工背景调查报告");

            #region 创建样式（目前没用到）
            //XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrap.WrapText = true;

            //IFont font1 = xssfworkbook.CreateFont();//字体
            //font1.FontHeightInPoints = 9;
            //XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrapandfont.WrapText = true;
            //stylewrapandfont.SetFont(font1);

            //XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            //stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));
            #endregion

            //填充数据到表格
            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //保存新文件
            using (FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xssfworkbook.Write(newFile);
                newFile.Close();
            }
        }

        /// <summary>
        /// 员工入职登记表
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="TempletePath">模板路径</param>
        public void ExportInductionRegistration(string filePath, string TempletePath)
        {
            //创建数据字典
            var info = new Dictionary<object, int[]>();
            info.Add("填表日期：" + this.CreateDate.Year + " 年 " + this.CreateDate.Month + " 月 " + this.CreateDate.Day + " 日 " + "2=0", new int[] { 2, 0 });
            info.Add(this.Name + "3=1", new int[] { 3, 1 });
            info.Add(this.Gender.GetDescription() + "3=3", new int[] { 3, 3 });
            info.Add(this.Personal.IDCard + "3=5", new int[] { 3, 5 });

            info.Add(this.Personal.Volk + "4=1", new int[] { 4, 1 });
            info.Add(this.Personal.PoliticalOutlook + "4=3", new int[] { 4, 3 });
            info.Add(this.Personal.Age + "4=5", new int[] { 4, 5 });

            info.Add(this.Personal.Height + "5=1", new int[] { 5, 1 });
            info.Add(this.Personal.Weight + "5=3", new int[] { 5, 3 });
            info.Add(this.Personal.Healthy + "5=5", new int[] { 5, 5 });

            info.Add(this.Personal.IsMarry == true ? "是" : "否" + "6=1", new int[] { 6, 1 });
            info.Add(this.Personal.NativePlace + "6=3", new int[] { 6, 3 });

            info.Add("  " + this.Personal.PassAddress + "7=1", new int[] { 7, 1 });
            info.Add("  " + this.Personal.HomeAddress + "8=1", new int[] { 8, 1 });
            info.Add(this.Personal.GraduatInstitutions + "9=1", new int[] { 9, 1 });
            info.Add(this.Personal.GraduationDate?.ToString("yyyy-MM-dd") + "9=5", new int[] { 9, 5 });

            info.Add(this.Personal.Major + "10=1", new int[] { 10, 1 });
            info.Add(this.Personal.Education + "10=3", new int[] { 10, 3 });
            info.Add(this.Personal.Email + "10=5", new int[] { 10, 5 });

            info.Add(this.Personal.LanguageLevel + "11=1", new int[] { 11, 1 });
            info.Add(this.Personal.ComputerLevel + "11=3", new int[] { 11, 3 });
            info.Add(this.Personal.Mobile + "11=5", new int[] { 11, 5 });

            var works = this.Personal.Workitems.ToArray();
            for (int i = 14; i < 18 && i < works.Length + 14; i++)
            {
                var work = works[i - 14];
                info.Add(work.StartTime.ToShortDateString().Replace("-", "") + "-" + work.EndTime.ToShortDateString().Replace("-", "") + i + "=0", new int[] { i, 0 });
                info.Add(work.Company + i + "=1", new int[] { i, 1 });
                info.Add(work.Position + i + "=4", new int[] { i, 4 });
                info.Add(work.LeaveReason + i + "=5", new int[] { i, 5 });
            }

            var familys = this.Personal.Familyitems.ToArray();
            for (int i = 20; i < 24 && i < familys.Length + 20; i++)
            {
                var family = familys[i - 20];
                info.Add(family.Name + i + "=0", new int[] { i, 0 });
                info.Add(family.Relation + i + "=1", new int[] { i, 1 });
                info.Add(family.Age.ToString() + i + "=2", new int[] { i, 2 });
                info.Add(family.Company + i + "=3", new int[] { i, 3 });
                info.Add(family.Phone + i + "=6", new int[] { i, 6 });
            }

            //info.Add(this.Personal.BeginWorkDate?.ToShortDateString() + "29=1", new int[] { 29, 1 });
            if (this.Status.GetHashCode() < StaffStatus.Period.GetHashCode())
            {
                info.Add(DateTime.Now.ToString("yyyy-MM-dd") + "29=1", new int[] { 29, 1 });
            }
            else
            {
                //入职登记表中的参加工作时间是用我们公司的入职时间
                info.Add(this.Labour.EntryDate.ToString("yyyy-MM-dd") + "29=1", new int[] { 29, 1 });
            }
            info.Add(string.IsNullOrEmpty(this.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription() + "29=3", new int[] { 29, 3 });
            info.Add(string.IsNullOrEmpty(this.PostionCode) ? "" : ((PostType)Enum.Parse(typeof(PostType), this.PostionCode)).GetDescription() + "29=5", new int[] { 29, 5 });
            info.Add(this.Labour.ProbationMonths.ToString() + "30=1", new int[] { 30, 1 });

            //获取模板
            FileStream file = new FileStream(TempletePath, FileMode.Open, FileAccess.Read);
            //用模板创建对象
            IWorkbook xssfworkbook = new XSSFWorkbook(file);
            //获取模板中的Sheet
            ISheet sheet1 = xssfworkbook.GetSheet("入职登记表");

            #region 创建样式（目前没用到）
            //XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrap.WrapText = true;

            //IFont font1 = xssfworkbook.CreateFont();//字体
            //font1.FontHeightInPoints = 9;
            //XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //stylewrapandfont.WrapText = true;
            //stylewrapandfont.SetFont(font1);

            //XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            //IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            //stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));
            #endregion

            //填充数据到表格
            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //保存新文件
            using (FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xssfworkbook.Write(newFile);
                newFile.Close();
            }
        }

        #endregion
    }
}