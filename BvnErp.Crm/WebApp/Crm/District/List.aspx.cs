using Needs.Utils.Serializers;
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
    /// 区域管理展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.Model.TreeData = new NtErp.Crm.Services.Views.DistrictTree().Tree;
                this.Model.SalesM = "".Json();
                this.Model.Sales = "".Json();
                this.Model.MarketM = "".Json();
                this.Model.MarketA = "".Json();
            }
        }
        
        /// <summary>
        /// 根据区域加载区域内管理员架构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            string Districtid = this.hidDistrict.Value;
            this.Model.TreeData = new NtErp.Crm.Services.Views.DistrictTree().Tree;
            //获取所有员工
            var admins = new NtErp.Crm.Services.Views.AdminTopView();

            this.Model.SalesM = admins.GetLead(Districtid,DistrictType.Sales).Select(item => new { id = item.ID, text = item.RealName }).Json();
            this.Model.Sales = admins.GetAdmin(Districtid, DistrictType.Sales).Select(item => new { id = item.ID, text = item.RealName }).Json();

            this.Model.MarketM = admins.GetLead(Districtid,DistrictType.Market).Select(item => new { id = item.ID, text = item.RealName }).Json();
            this.Model.MarketA = admins.GetAdmin(Districtid, DistrictType.Market).Select(item => new { id = item.ID, text = item.RealName }).Json();
        }
    }
}