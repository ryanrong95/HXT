using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail
{
    public partial class CompanyDetails : ErpParticlePage
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
            this.Model.PostionData = Alls.Current.Postions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.CityData = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity).Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.Status = ExtendsEnum.ToDictionary<StaffStatus>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.SchedulingData = new Services.Views.Origins.SchedulingsOrigin().Where(item => item.Name != "C班").Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.RegionData = new Services.Views.Origins.RegionsAcOrigin().Select(item => new { Value = item.ID, Text = item.Name });

            this.Model.DepartmentType = ExtendsEnum.ToDictionary<Services.Common.DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.PostType = ExtendsEnum.ToDictionary<Services.Common.PostType>().Select(item => new { Value = item.Key, Text = item.Value });
        }

        protected void LoadData()
        {
            string StaffID = Request.QueryString["ID"];
            Staff staff = Alls.Current.Staffs.Single(item => item.ID == StaffID);
            this.Model.StaffData = new
            {
                CompanyName = staff.Labour?.EntryCompany,
                Code = staff.Code,
                SelCode = staff.SelCode,
                Status = staff.Status,
                UserName = staff.Admin?.UserName,
                Password = staff.Admin?.Password,
                DyjCompanyCode = staff.DyjCompanyCode,
                DyjDepartmentCode = staff.DyjDepartmentCode,
                DyjCode = staff.DyjCode,
                WorkCity = staff.WorkCity,
                PostionID = staff.PostionID,
                WorkClassID = staff.SchedulingID,
                RegionID = staff.RegionID,
                DepartmentCode = staff.DepartmentCode,
                PostionCode = staff.PostionCode,
            };

            this.Model.VocationData = new
            {
                YearsDay = staff.YearsDay,
                OffDay = staff.OffDay,
                SickDay = staff.SickDay,
                ProductionInspectionDay = staff.ProductionInspectionDay,
            };
        }
    }
}