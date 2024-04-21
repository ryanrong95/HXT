using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Underly;
using Yahv.Linq.Extends;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using Yahv.Erm.Services.Models.Origins;
using Layers.Data;
using Yahv.Utils;
using System.Data;
using Yahv.Utils.Serializers;
using System.Linq.Expressions;
using Yahv.Erm.Services.Common;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class ListNormal : ErpParticlePage
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
            this.Model.PostionData = Erp.Current.Erm.XdtPostions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.PostType = ExtendsEnum.ToDictionary<PostType>().Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Staff, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var staffs = Erp.Current.Erm.XdtStaffs.Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).Where(expression);

            var data = staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Code,
                item.SelCode,
                item.Name,
                DepartmentCode = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                PostionName = item.Postion?.Name,
                Gender = item.Gender.GetDescription(),
                BirthDate = item.Personal.BirthDate?.ToString("yyyy-MM-dd"),
                BeginWorkDate = item.Personal.BeginWorkDate?.ToString("yyyy-MM-dd"),
                Volk = item.Personal.Volk,
                IsMarry = item.Personal.IsMarry == true ? "已婚" : "未婚",
                PassAddress = item.Personal.PassAddress,
                HomeAddress = item.Personal.HomeAddress,
                IDCard = item.Personal.IDCard,
                Education = item.Personal.Education,
                GraduatInstitutions = item.Personal.GraduatInstitutions,
                Major = item.Personal.Major,
                Mobile = item.Personal.Mobile,
                EmergencyContact = item.Personal.EmergencyContact,
                EmergencyMobile = item.Personal.EmergencyMobile,
                EntryDate = item.Labour.EntryDate.ToShortDateString(),
                ContractPeriod = item.Labour?.ContractPeriod?.ToShortDateString(),
                Summary = item.Personal.Summary,
                Status = item.Status,
                StatusDec = item.Status.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }

        Expression<Func<Staff, bool>> Predicate()
        {
            Expression<Func<Staff, bool>> predicate = item => true;

            //查询参数
            var Name = Request.QueryString["Name"];
            var PositionID = Request.QueryString["Position"];
            var DepartmentType = Request.QueryString["DepartmentType"];
            var PostType = Request.QueryString["PostType"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Name.Contains(Name) || item.Code.Contains(Name)|| item.SelCode.Contains(Name));
            }
            if (!string.IsNullOrWhiteSpace(PositionID))
            {
                predicate = predicate.And(item => item.PostionID == PositionID);
            }
            if (!string.IsNullOrWhiteSpace(DepartmentType))
            {
                predicate = predicate.And(item => item.DepartmentCode == DepartmentType);
            }
            if (!string.IsNullOrWhiteSpace(PostType))
            {
                predicate = predicate.And(item => item.PostionCode == PostType);
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                var start = Convert.ToDateTime(StartDate);
                predicate = predicate.And(item => item.Labour.EntryDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                var end = Convert.ToDateTime(EndDate).AddDays(1);
                predicate = predicate.And(item => item.Labour.EntryDate < end);
            }
            return predicate;
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void TurnNormal()
        {
            try
            {
                string ID = Request.Form["ID"];
                var files = new Services.Views.StaffFileAlls(ID).Where(item => item.Type == (int)FileType.TurnNormal);
                if (files.Count() == 0)
                {
                    throw new Exception("请上传转正申请附件");
                }
                var staff = Alls.Current.Staffs[ID];
                var flag = DateTime.Now < staff.Labour.ProbationEndDate;
                if (flag)
                {
                    throw new Exception("实习期还未结束，不能转正");
                }
                staff.TurnNormal();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }
    }
}