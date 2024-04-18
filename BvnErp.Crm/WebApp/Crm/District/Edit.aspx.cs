using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.District
{
    /// <summary>
    /// 区域管理设置页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 加载人员数据
        /// </summary>
        private void LoadData()
        {
            string districtid = Request.QueryString["ID"];
            DistrictType Type = (DistrictType)int.Parse(Request.QueryString["Type"]);

            var admins = new NtErp.Crm.Services.Views.AdminTopView();

            this.Model.Managers = admins.GetLead(districtid, Type);
            this.Model.Members = admins.GetAdmin(districtid, Type);
            if (Type == DistrictType.Sales)
            {
                this.Model.Admins = admins.Where(item => item.JobType == JobType.Sales || item.JobType == JobType.Sales_PME);
            }
            else if (Type == DistrictType.Market)
            {
                this.Model.Admins = admins.Where(item => item.JobType == JobType.PME || item.JobType == JobType.FAE || item.JobType == JobType.Sales_PME);
            }
           
        }

        /// <summary>
        /// 人员架构保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string districtid = Request.QueryString["ID"];
            string[] managerids = this.hidManaids.Value.Split(',');
            string[] memberids = this.hidMemids.Value.Split(',');
            int Type = int.Parse(Request.QueryString["Type"].ToString());

            if (string.IsNullOrWhiteSpace(districtid))
            {
                throw new Exception("区域id 为空！！！");
            }

            //删除绑定
            Needs.Erp.ErpPlot.Current.ClientSolutions.Districts.DeleteBinding(districtid,Type);

            foreach (string manaid in managerids)
            {
                var leader = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(manaid);

                foreach(string memid in memberids)
                {
                    var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(memid);
                    Needs.Erp.ErpPlot.Current.ClientSolutions.Districts.Binding(districtid, leader, admin, Type);
                }
            }

            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
    }
}