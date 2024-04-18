using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Problems
{
    public partial class Edit : Uc.PageBase
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
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string StandardID = Request.QueryString["StandardID"];
            string id = Request.QueryString["ID"];
            //var problem = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[StandardID].Problems[id] as
            //    NtErp.Crm.Services.Models.Problem ?? new NtErp.Crm.Services.Models.Problem();
            //problem.StandardID = StandardID;
            //problem.ContactID = Request.Form["ContactID"];
            //problem.Answer= Request.Form["Answer"];
            //problem.Context = Request.Form["Context"];
            //problem.AdminID = Needs.Erp.ErpPlot.Current.ID;
            //problem.EnterSuccess += Problem_EnterSuccess;
            //problem.Enter();
        }


        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Problem_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Uri url = Request.UrlReferrer ?? Request.Url;
            Alert("保存成功", url, true);
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