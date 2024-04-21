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
    public partial class ListHistory : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
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

            var staffs = Erp.Current.Erm.XdtStaffs.Where(expression);

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
                LeaveDate = item.Labour.LeaveDate?.ToShortDateString(),
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

            string Date = Request.QueryString["Date"];
            DateTime date = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(Date))
            {
                date = Convert.ToDateTime(Date);
            }
            predicate = predicate.And(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period || item.Status == StaffStatus.Departure || item.Status == StaffStatus.Cancel);
            predicate = predicate.And(item => item.Labour.EntryDate <= date);
            return predicate;
        }

        /// <summary>
        /// 导出员工花名册
        /// </summary>
        protected void ExportRoster()
        {
            try
            {
                string Date = Request.Form["Date"];
                DateTime date = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(Date))
                {
                    date = Convert.ToDateTime(Date);
                }

                var fileName = FileType.Roster.GetDescription() + "-" + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.Roster);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                string TempletePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Template\\files\\芯达通公司人员花名册.xlsx");
                StaffHelper.ExportRoster(filePath, TempletePath, date);

                var fileUrl = @"../../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "导出成功", fileUrl }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }
    }
}