using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksOther
{
    public partial class Show : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }
        void PageInit()
        {
            string id = Request.QueryString["id"];
            var work = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther[id];
            var reply = new ReplyAlls().Where(item => item.WorksOtherID == id);
            var files = new FileAlls().Where(item => item.WorksOtherID == id && item.Status == Status.Normal);
            this.AdminName.Text = work.Admin.RealName;
            this.Model.AllData = new
            {
                work.StartDate,
                work.Subject,
                IsOwner = work.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                //isLeader = GetStaffs(Needs.Erp.ErpPlot.Current.ID).Contains(work.Admin.ID), //是否为上级人员
                work.Context,
                Reply = reply.OrderBy(c => c.UpdateDate).Select(item => new
                {
                    RealName = item.Admin.RealName,
                    UpdateDate = item.UpdateDate,
                    Context = item.Context
                }).ToArray(),
                files = files.Select(item => new
                {
                    URL = item.Url,
                    Name = item.Name
                }).ToArray(),
            }.Json();
        }

        /// <summary>
        /// 获取员工集合
        /// </summary>
        /// <param name="AdminID">人员ID</param>
        /// <returns></returns>
        private IEnumerable<string> GetStaffs(string AdminID)
        {
            List<string> list = new List<string>() { AdminID };

            var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(AdminID);
            if (admin.JobType == NtErp.Crm.Services.Enums.JobType.TPM)
            {
                return new string[0];
            }
            else
            {
                //获取所有员工
                var Mystaffids = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => item.ID).ToArray();
                return Mystaffids.Except(list);
            }
        }
    }
}