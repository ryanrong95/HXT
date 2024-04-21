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
using System.Web;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class ListBackgroundCheck : ErpParticlePage
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
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray();

            var fileView = new Yahv.Services.Views.CenterFilesTopView()
                .Where(item => item.Type == (int)FileType.BackgroundInvestigation && item.Status == Yahv.Services.Models.FileDescriptionStatus.Normal).ToArray();

            var datas = from staff in staffs
                        join file in fileView on staff.ID equals file.StaffID into files
                        select new
                        {
                            staff.ID,
                            staff.Code,
                            staff.SelCode,
                            staff.Name,
                            DepartmentCode = string.IsNullOrEmpty(staff.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)).GetDescription(),
                            PostionName = staff.Postion?.Name,
                            Gender = staff.Gender.GetDescription(),
                            BirthDate = staff.Personal.BirthDate?.ToString("yyyy-MM-dd"),
                            Volk = staff.Personal.Volk,
                            IsMarry = staff.Personal.IsMarry == true ? "已婚" : "未婚",
                            PassAddress = staff.Personal.PassAddress,
                            HomeAddress = staff.Personal.HomeAddress,
                            IDCard = staff.Personal.IDCard,
                            Education = staff.Personal.Education,
                            GraduatInstitutions = staff.Personal.GraduatInstitutions,
                            Major = staff.Personal.Major,
                            Mobile = staff.Personal.Mobile,
                            EmergencyContact = staff.Personal.EmergencyContact,
                            EmergencyMobile = staff.Personal.EmergencyMobile,
                            EntryDate = staff.Labour.EntryDate.ToShortDateString(),
                            ContractPeriod = staff.Labour?.ContractPeriod?.ToShortDateString(),
                            Summary = staff.Personal.Summary,
                            Status = staff.Status,
                            StatusDec = staff.Status.GetDescription(),
                            CreateDate = staff.CreateDate.ToString("yyyy-MM-dd"),

                            LastBackgroundCheck = files.Max(item => item.CreateDate),
                        };
            //背景调查需求修改
            var date1 = new DateTime(DateTime.Now.Year, 1, 1);
            var date2 = new DateTime(DateTime.Now.Year, 2, 1);
            var data = datas.Where(item => (item.LastBackgroundCheck == null || item.LastBackgroundCheck < date1) && DateTime.Now >= date2);

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }

        /// <summary>
        /// 员工背景调查报告
        /// </summary>
        protected void ExportBackgroundInvestigation()
        {
            try
            {
                string StaffID = Request.Form["ID"];
                var staff = Alls.Current.Staffs[StaffID];

                var fileName = FileType.BackgroundInvestigation.GetDescription() + "-" + staff.Name + staff.Code + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.BackgroundInvestigation);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                string TempletePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Template\\files\\员工背景调查报告.xlsx");
                staff.ExportBackgroundInvestigation(filePath, TempletePath);

                var fileUrl = @"../../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "导出成功", fileUrl }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                string StaffID = Request.Form["StaffID"];
                var staff = Alls.Current.Staffs[StaffID];
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            if (!(file.FileName.Contains(staff.Name) && file.FileName.Contains(staff.Code)))
                            {
                                throw new Exception("文件名称与员工不匹配,请用导出时的名称");
                            }
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.BackgroundInvestigation);
                            dic.AdminID = Erp.Current.ID;
                            dic.StaffID = StaffID;
                            dic.Save(file);
                        }
                    }
                }
                Response.Write((new { success = true, message = "导入成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }
    }
}