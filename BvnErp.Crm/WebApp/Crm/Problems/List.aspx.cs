using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Problems
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
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string StandardID = Request.QueryString["StandardID"];
            //var problem = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[StandardID].Problems.Where(c=>c.Status!=Status.Delete);
            //this.Paging(problem);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            string StandardID = Request.Form["StandardID"];
            string id = Request.Form["ID"];
            //var del = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[StandardID].Problems[id];
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