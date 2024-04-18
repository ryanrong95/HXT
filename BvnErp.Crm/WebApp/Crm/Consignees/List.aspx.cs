using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Consignees
{
    /// <summary>
    /// 收发地址展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        protected void data()
        {
            string clientid = Request.QueryString["ClientID"];           
            var Consigneedata = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees;
            var data = Consigneedata.Where(item => item.ClientID == clientid);
            Func<Consignee, object> linq = item => new
            {
                ID = item.ID,
                CompanyName = item.CompanyID,
                ContactName = item.Contact.Name,            
                Address = item.Address,
                Zipcode = item.Zipcode,
                Status = item.Status.GetDescription(),
            };
            this.Paging(data, linq);
        }
        
        /// <summary>
        /// 数据删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string id = this.hidID.Value;
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees[id];
            if (del != null)
            {
                del.AbandonError += Del_AbandonForbidden;
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonForbidden(object sender, Needs.Linq.ErrorEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert(e.Message, url, true);
        }

        /// <summary>
        /// 删除成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("删除成功！", url, false);
        }
    }
}