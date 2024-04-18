using NtErp.Crm.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Problems
{
    public partial class Show : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string StandardID = Request.QueryString["StandardID"];
            string id = Request.QueryString["ID"];
            //var data = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[StandardID].Problems[id];
            //this.Model.AllData = data.Json();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var contact = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts;
            this.Model.ContactData = contact.Where(c => c.Status != Status.Delete).Select(c => new { value = c.ID, text = c.Name }).Json();
        }
    }
}