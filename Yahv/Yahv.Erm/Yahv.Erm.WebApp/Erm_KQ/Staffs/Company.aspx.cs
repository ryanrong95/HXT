using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class Company : ErpParticlePage
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
            this.Model.PostionData = Erp.Current.Erm.XdtPostions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.CityData = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity).Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.Status = ExtendsEnum.ToDictionary<StaffStatus>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.SchedulingData = new Services.Views.Origins.SchedulingsOrigin().Where(item => item.IsMain == true).Select(item => new { Value = item.ID, Text = item.Name });
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

        protected void Submit()
        {
            try
            {
                #region 界面数据
                string StaffID = Request.Form["StaffID"];
                string StaffCode = Request.Form["StaffCode"];
                string StaffStatus = Request.Form["StaffStatus"];
                string UserName = Request.Form["UserName"];
                string Password = Request.Form["Password"];
                string SelCode = Request.Form["SelCode"];
                string WorkCity = Request.Form["WorkCity"];
                string Postion = Request.Form["Postion"];
                string WorkingClass = Request.Form["WorkingClass"];
                string Region = Request.Form["Region"];
                string DepartmentType = Request.Form["DepartmentType"];
                string PostType = Request.Form["PostType"];
                #endregion
                if (!string.IsNullOrEmpty(DepartmentType) && !string.IsNullOrEmpty(PostType))
                {
                    var staffs = Erp.Current.Erm.XdtStaffs
                        .Where(item => item.Status == Yahv.Erm.Services.StaffStatus.Normal || item.Status == Yahv.Erm.Services.StaffStatus.Period)
                        .Where(item => item.ID != StaffID);
                    if (PostType == ((int)Services.Common.PostType.Manager).ToString())
                    {
                        var count = staffs.Count(item => item.DepartmentCode == DepartmentType && item.PostionCode == PostType);
                        if (count > 0)
                        {
                            throw new Exception("该部门已有负责人，不能重复设置.");
                        }
                    }
                    if (PostType == ((int)Services.Common.PostType.President).ToString())
                    {
                        var count = staffs.Count(item => item.PostionCode == PostType);
                        if (count > 0)
                        {
                            throw new Exception("公司已有总经理，不能重复设置.");
                        }
                    }
                }

                Staff staff = Erp.Current.Erm.XdtStaffs.Single(item => item.ID == StaffID);
                //职位是否变更
                bool postionChange = staff.PostionID == Postion ? false : true;

                //保存员工信息
                staff.SelCode = SelCode;
                staff.WorkCity = WorkCity;
                staff.PostionID = Postion;
                staff.SchedulingID = WorkingClass;
                staff.RegionID = Region;
                staff.DepartmentCode = DepartmentType;
                staff.PostionCode = PostType;
                staff.Enter();

                //职位变更-》更新工作项
                if (postionChange && !string.IsNullOrEmpty(Postion))
                {
                    var wage = Alls.Current.MyWageItems;
                    //初始化工资项
                    wage.InitWageItems(StaffID, staff.PostionID);
                }

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"公司信息编辑", staff.Json());

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
    }
}