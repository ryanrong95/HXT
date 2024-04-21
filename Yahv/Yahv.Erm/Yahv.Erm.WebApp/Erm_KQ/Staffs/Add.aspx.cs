using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.Gender = ExtendsEnum.ToDictionary<Gender>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.PoliticType = ExtendsEnum.ToDictionary<PoliticType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.ChineseNationType = ExtendsEnum.ToDictionary<ChineseNationType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.HealthyType = ExtendsEnum.ToDictionary<HealthyType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.BloodType = ExtendsEnum.ToDictionary<BloodType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.EducationType = ExtendsEnum.ToDictionary<EducationType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.MaritalStatus = ExtendsEnum.ToDictionary<MaritalStatus>().Select(item => new { Value = item.Value, Text = item.Value });
        }

        /// <summary>
        /// 工作经历
        /// </summary>
        /// <returns></returns>
        protected object LoadWork()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

        /// <summary>
        /// 家庭成员
        /// </summary>
        /// <returns></returns>
        protected object LoadFamily()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

        protected void Submit()
        {
            var staff = new Staff();
            try
            {
                #region 界面数据
                string Name = Request.Form["Name"];
                string Gender = Request.Form["Gender"];
                string BirthDate = Request.Form["BirthDate"];
                string Volk = Request.Form["Volk"];
                string PoliticalOutlook = Request.Form["PoliticalOutlook"];
                string IsMarry = Request.Form["IsMarry"];
                string Healthy = Request.Form["Healthy"];
                string IDCard = Request.Form["IDCard"];
                string NativePlace = Request.Form["NativePlace"];
                string PassAddress = Request.Form["PassAddress"];

                string Blood = Request.Form["Blood"];
                string Height = Request.Form["Height"];
                string Weight = Request.Form["Weight"];
                string Education = Request.Form["Education"];
                string Major = Request.Form["Major"];
                string GraduationDate = Request.Form["GraduationDate"];
                string GraduatInstitutions = Request.Form["GraduatInstitutions"];
                string HomeAddress = Request.Form["HomeAddress"];
                string Mobile = Request.Form["Mobile"];
                string Email = Request.Form["Email"];
                string BeginWorkDate = Request.Form["BeginWorkDate"];
                string EmergencyContact = Request.Form["EmergencyContact"];
                string EmergencyMobile = Request.Form["EmergencyMobile"];
                string LanguageLevel = Request.Form["LanguageLevel"];
                string ComputerLevel = Request.Form["ComputerLevel"];
                string SelfEvaluation = Request.Form["SelfEvaluation"];
                string PositionName = Request.Form["PositionName"];
                string Treatment = Request.Form["Treatment"];

                //工作经历
                var works = Request.Form["works"].Replace("&quot;", "'").Replace("amp;", "");
                var workList = works.JsonTo<List<PersonalWorkExperience>>();
                //家庭成员
                var familys = Request.Form["familys"].Replace("&quot;", "'").Replace("amp;", "");
                var familyList = familys.JsonTo<List<PersonalFamilyMember>>();
                #endregion

                if (Alls.Current.Personals.Where(item => item.IDCard == IDCard).Count() != 0)
                {
                    throw new Exception("身份证ID重复，保存失败");
                }

                //保存员工信息
                staff.Name = Name;
                staff.Gender = (Gender)Enum.Parse(typeof(Gender), Gender);
                staff.AdminID = Erp.Current.ID;
                staff.Enter();
                //保存员工日志
                var log = new Logs_StaffApproval();
                log.StaffID = staff.ID;
                log.ApprovalStep = StaffApprovalStep.Interview;
                log.ApprovalStatus = StaffApprovalStatus.Waiting;
                log.Enter();

                //保存账号信息
                var admin = new Admin();
                admin.StaffID = staff.ID;
                admin.UserName = GetAccountByName(staff.Name);
                admin.Password = "123456";
                admin.RealName = staff.Name;
                admin.Status = AdminStatus.Closed;
                admin.Enter();

                //劳务信息
                var labour = new Services.Models.Origins.Labour();
                labour.ID = staff.ID;
                labour.EntryDate = DateTime.Now;
                labour.SigningTime = labour.EntryDate;
                labour.ContractPeriod = labour.EntryDate.AddYears(3);
                labour.EnterpriseID = ErmConfig.LabourEnterpriseID;
                labour.EntryCompany = ErmConfig.LabourEnterpriseName;
                labour.ProbationMonths = "3";
                labour.SocialSecurityAccount = "";
                labour.Enter();

                //保存人
                var personal = new Personal();
                personal.ID = staff.ID;
                personal.IDCard = IDCard;
                personal.NativePlace = NativePlace;
                personal.HomeAddress = HomeAddress;
                personal.PassAddress = PassAddress;
                personal.Volk = Volk;
                personal.PoliticalOutlook = PoliticalOutlook;
                personal.Height = double.Parse(Height);
                personal.Weight = double.Parse(Weight);
                personal.Blood = Blood;
                personal.Education = Education;
                personal.Major = Major;
                personal.GraduatInstitutions = GraduatInstitutions;
                personal.GraduationDate = Convert.ToDateTime(GraduationDate);
                personal.Mobile = Mobile;
                personal.Email = Email;
                personal.BirthDate = Convert.ToDateTime(BirthDate);
                personal.BeginWorkDate = Convert.ToDateTime(BeginWorkDate);
                personal.Healthy = Healthy;
                personal.IsMarry = IsMarry == MaritalStatus.Married.GetDescription() ? true : false;
                personal.EmergencyContact = EmergencyContact;
                personal.EmergencyMobile = EmergencyMobile;
                personal.LanguageLevel = LanguageLevel;
                personal.ComputerLevel = ComputerLevel;
                personal.SelfEvaluation = SelfEvaluation;
                personal.PositionName = PositionName;
                personal.Treatment = Treatment;

                personal.Workitems = workList;
                personal.Familyitems = familyList;
                personal.Enter();

                //员工假期初始化
                staff.Personal = personal;
                staff.InitVacation();

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"员工新增", staff.Json());
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                //删除保存的信息
                staff.Abandon();

                string message = "空间名：" + ex.Source + "；" + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("\\") + 1, ex.StackTrace.Length - ex.StackTrace.LastIndexOf("\\") - 1) + '\n' +
                    "错误提示：" + ex.Message;
                Response.Write((new { success = false, message = "保存失败：" + message }).Json());
            }
        }

        /// <summary>
        /// 根据姓名获取账号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetAccountByName(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(name)) return result;

            result = Utils.PinyinHelper.GetPinyin(name);
            int num = 1;
            while (Alls.Current.Admins.Where(item => item.UserName == result).Count() >= 1)
            {
                if (num == 1)
                {
                    result = result + num.ToString();
                }
                else
                {
                    result = result.Replace((num - 1).ToString(), num.ToString());
                }
                num++;
            }

            return result;
        }
    }
}