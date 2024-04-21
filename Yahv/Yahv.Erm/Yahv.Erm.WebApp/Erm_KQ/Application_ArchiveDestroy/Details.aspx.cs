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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_ArchiveDestroy
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
                    Supervisor = application.ArchiveDestroyContext.Supervisor,

                    ApproveID = application.ArchiveDestroyContext.ApproveID,
                    DepartmentName = application.ArchiveDestroyContext.DepartmentName,
                };
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

        /// <summary>
        /// 销毁档案内容
        /// </summary>
        /// <returns></returns>
        protected object LoadArchives()
        {
            var id = Request.QueryString["ID"];
            var dates = Erp.Current.Erm.Applications[id]?.ArchiveDestroyContext?.Archives ?? new List<ArchiveContext>();
            var data = dates.Select(item => new
            {
                Year = item.Year,
                Type = item.Type,
                Code = item.Code,
                Context = item.Context,
                Count = item.Count,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }
    }
}