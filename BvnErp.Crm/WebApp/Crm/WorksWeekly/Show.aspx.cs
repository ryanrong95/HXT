using NtErp.Crm.Services.Enums;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksWeekly
{
    /// <summary>
    /// 工作周报详情展示页面
    /// </summary>
    public partial class Show : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WeekOfYear.Text = Get_WeekOfYear(DateTime.Now, new CultureInfo("zh-CN")).ToString();
                PageInit();
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        void PageInit()
        {
            string id = Request.QueryString["id"];
            var work = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly[id];
            var reply = new ReplyAlls().Where(item => item.WorksWeeklyID == id);
            var files = new FileAlls().Where(item => item.WorksWeeklyID == id && item.Status == Status.Normal);
            this.Model.AllData = new
            {
                work.WeekOfYear,  //当前周
                IsOwner = work.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                //isLeader = GetStaffs(Needs.Erp.ErpPlot.Current.ID).Contains(work.Admin.ID), //是否为上级人员
                work.Context,
                Reply = reply.OrderBy(c => c.UpdateDate).Select(item => new
                {
                    item.Admin.RealName,
                    UpdateDate = item.UpdateDate,
                    item.Context
                }).ToArray(),
                files = files.Select(item => new
                {
                    URL = item.Url,
                    Name = item.Name
                }).ToArray(),
            }.Json();
            if (work != null)
            {
                AdminName.Text = work.Admin.RealName;
                CreateDate.Text = work.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 获取当前周
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ci"></param>
        /// <returns></returns>
        public static int Get_WeekOfYear(DateTime dt, CultureInfo ci)
        {
            return ci.Calendar.GetWeekOfYear(dt, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
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