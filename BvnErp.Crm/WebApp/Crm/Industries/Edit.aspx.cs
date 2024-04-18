using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Industries
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
            }
        }

        void PageInit()
        {
            string id = Request.QueryString["id"];
            string fatherID = Request.QueryString["fatherID"];
            var industry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries[id];
            this.Model.AllData = industry.Json();
        }



        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string fatherID = Request.QueryString["fatherID"];
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