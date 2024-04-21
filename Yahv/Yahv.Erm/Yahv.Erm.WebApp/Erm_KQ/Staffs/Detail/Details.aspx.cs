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

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail
{
    public partial class Details : ErpParticlePage
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
    }
}