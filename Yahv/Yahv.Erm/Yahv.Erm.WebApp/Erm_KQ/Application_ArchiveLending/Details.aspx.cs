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
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_ArchiveLending
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
            var staffs = Erp.Current.Erm.XdtStaffs.ToArray();
            //员工
            this.Model.StaffData = staffs.OrderBy(item => item.DepartmentCode).Select(item => new
            {
                Value = item.Admin?.ID,
                Text = item.Name,
                SelCode = item.SelCode,
                DepartmentCode = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                PostionName = item.Postion?.Name,
                EntryDate = item.Labour?.EntryDate.ToString("yyyy-MM-dd"),
            });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
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
                    BorrowDate = application.ArchiveLendingContext.BorrowDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ReturnDate = application.ArchiveLendingContext.ReturnDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Reason = application.ArchiveLendingContext.Reason,
                    ArchiveName = application.ArchiveLendingContext.ArchiveName,
                    Keeper = application.ArchiveLendingContext.Keeper,
                    Count = application.ArchiveLendingContext.Count,

                    ApproveID = application.ArchiveLendingContext.ApproveID,
                    DepartmentName = application.ArchiveLendingContext.DepartmentName,
                };
            }
        }

        protected void StaffChange()
        {
            try
            {
                var staffs = Erp.Current.Erm.XdtStaffs.Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
                //员工姓名
                string Name = Request.Form["Name"];
                var staff = staffs.Single(item => item.Admin.ID == Name);
                //员工部门的负责人
                var manager = staffs.Where(item => item.DepartmentCode == staff.DepartmentCode)
                    .Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString()).FirstOrDefault();
                var data = new
                {
                    department = (int)((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)),
                    manager = manager?.Admin.ID,
                    selcode = staff.SelCode,
                };
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["ID"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).AsEnumerable();
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
    }
}