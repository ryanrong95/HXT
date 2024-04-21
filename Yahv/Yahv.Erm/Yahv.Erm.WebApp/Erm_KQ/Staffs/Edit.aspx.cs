using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
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
            this.Model.Gender = ExtendsEnum.ToDictionary<Gender>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.PoliticType = ExtendsEnum.ToDictionary<PoliticType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.ChineseNationType = ExtendsEnum.ToDictionary<ChineseNationType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.HealthyType = ExtendsEnum.ToDictionary<HealthyType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.BloodType = ExtendsEnum.ToDictionary<BloodType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.EducationType = ExtendsEnum.ToDictionary<EducationType>().Select(item => new { Value = item.Value, Text = item.Value });
            this.Model.MaritalStatus = ExtendsEnum.ToDictionary<MaritalStatus>().Select(item => new { Value = item.Value, Text = item.Value });
        }

        protected void LoadData()
        {
            string StaffID = Request.QueryString["ID"];
            Staff staff = Alls.Current.Staffs.Single(item => item.ID == StaffID);
            this.Model.StaffData = new
            {
                Name = staff.Name,
                Gender = staff.Gender,
                BirthDate = staff.Personal.BirthDate,
                Volk = staff.Personal.Volk,
                PoliticalOutlook = staff.Personal.PoliticalOutlook,
                IsMarry = staff.Personal.IsMarry == true ? MaritalStatus.Married.GetDescription() : MaritalStatus.Unmarried.GetDescription(),
                Healthy = staff.Personal.Healthy,
                IDCard = staff.Personal.IDCard,
                NativePlace = staff.Personal.NativePlace,
                PassAddress = staff.Personal.PassAddress,

                Blood = staff.Personal.Blood,
                Height = staff.Personal.Height,
                Weight = staff.Personal.Weight,
                Education = staff.Personal.Education,
                Major = staff.Personal.Major,
                GraduationDate = staff.Personal.GraduationDate,
                GraduatInstitutions = staff.Personal.GraduatInstitutions,
                HomeAddress = staff.Personal.HomeAddress,
                Mobile = staff.Personal.Mobile,
                Email = staff.Personal.Email,
                BeginWorkDate = staff.Personal.BeginWorkDate,
                EmergencyContact = staff.Personal.EmergencyContact,
                EmergencyMobile = staff.Personal.EmergencyMobile,

                SelfEvaluation = staff.Personal.SelfEvaluation,
                LanguageLevel = staff.Personal.LanguageLevel,
                ComputerLevel = staff.Personal.ComputerLevel,
                Treatment = staff.Personal.Treatment,
                PositionName = staff.Personal.PositionName,
            };
        }

        /// <summary>
        /// 工作经历
        /// </summary>
        /// <returns></returns>
        protected object LoadWork()
        {
            string StaffID = Request.QueryString["ID"];
            var works = new PersonalWorkExperiencesOrigin().Where(item => item.StaffID == StaffID).AsEnumerable();
            var linq = works.Select(t => new
            {
                t.ID,
                t.StaffID,
                StartTime = t.StartTime.ToString("yyyy-MM-dd"),
                EndTime = t.EndTime.ToString("yyyy-MM-dd"),
                t.Company,
                t.Position,
                t.Salary,
                t.LeaveReason,
                t.Phone,
            });
            return linq;
        }

        /// <summary>
        /// 家庭成员
        /// </summary>
        /// <returns></returns>
        protected object LoadFamily()
        {
            string StaffID = Request.QueryString["ID"];
            var familys = new PersonalFamilyMembersOrigin().Where(item => item.StaffID == StaffID);
            var linq = familys.Select(t => new
            {
                t.ID,
                t.StaffID,
                t.Name,
                t.Relation,
                t.Company,
                t.Position,
                t.Age,
                t.Phone,
            });
            return linq;
        }

        protected void Submit()
        {
            try
            {
                #region 界面数据
                string StaffID = Request.Form["StaffID"];
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

                //保存员工信息
                Staff staff = Alls.Current.Staffs.Single(item => item.ID == StaffID);
                staff.Name = Name;
                staff.Gender = (Gender)Enum.Parse(typeof(Gender), Gender);
                staff.AdminID = Erp.Current.ID;
                staff.Enter();

                //保存人
                Personal personal = staff.Personal ?? new Personal();
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
                bool isChange = personal.BeginWorkDate != Convert.ToDateTime(BeginWorkDate);//开始工作时间是否变更
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
                if (isChange)
                {
                    //重新初始化假期
                    staff.InitVacation();
                }

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"基本信息编辑", staff.Json());
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
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