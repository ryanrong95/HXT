using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Preminum
{
    public partial class List : Uc.PageBase
    { 
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string catalogueID = Request.QueryString["ID"];
            var catalogue = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID];
            //var Preminums = catalogue.Preminums;
            //this.Paging(Preminums);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            string catalogueID = Request.QueryString["ID"];
            string id = Request.Form["ID"];
            //var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].Preminums[id];
            //if (del != null)
            //{
            //    del.AbandonSuccess += Del_AbandonSuccess;
            //    del.Abandon();
            //}
        }

        /// <summary>
        /// 删除触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}