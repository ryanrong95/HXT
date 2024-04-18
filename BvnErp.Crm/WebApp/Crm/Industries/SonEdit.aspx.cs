using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Industries
{
    /// <summary>
    /// 行业子类编辑页面
    /// </summary>
    public partial class SonEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropDownList();
                this.PageInit();
            }
        }

        void PageInit()
        {
            string id = Request.QueryString["id"];
            string fatherID = Request.QueryString["fatherID"];
            string Type = Request.QueryString["Type"];
            var industry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries[id];
            this.Model.AllData = industry.Json();
        }

        private void BindDropDownList()
        {
            string fatherID = Request.QueryString["fatherID"]; //最大的分类
            string Type = Request.QueryString["Type"];
            if (Type == "1") //终端市场分组
            {
                this.Model.FatherIndustryData = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.Where(c => c.ID == fatherID).Select(c => new { value = c.ID, text = c.Name }).Json();
            }
            else if (Type == "2")//终端市场分组子类
            {
                this.Model.FatherIndustryData = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.Where(c => c.FatherID == fatherID).Select(c => new { value = c.ID, text = c.Name }).Json();
            }
            else if (Type == "3")//终端市场分组子子类
            {
                var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.AsQueryable();
                var datas = from ins in data
                            join ins2 in data on ins.ID equals ins2.FatherID
                            join ins3 in data on ins2.ID equals ins3.FatherID
                            where ins.ID == fatherID
                            select new
                            {
                                value = ins3.ID,
                                text = ins3.Name
                            };
                this.Model.FatherIndustryData = datas.ToList().Json();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string fatherID = Request.Form["FatherIndustry"];
            var ins = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries[id] as
       NtErp.Crm.Services.Models.Industry ?? new NtErp.Crm.Services.Models.Industry();
            ins.FatherID = fatherID;
            ins.Name = Request.Form["Name"];
            ins.EnglishName = Request.Form["EnglishName"];
            ins.EnterSuccess += Contact_EnterSuccess;
            ins.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
    }
}