using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Contacts
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
        /// 数据查询
        /// </summary>
        protected void data()
        {
            string clientid = Request.QueryString["ClientID"];
            var Contacts = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts;
            var data = Contacts.Where(item => item.ClientID == clientid);
            Func<Contact, object> linq = item => new
            {
                item.ID,
                item.Name,
                ClientName = item.Clients.Name,
                item.Position,
                item.Email,
                item.Mobile,
                item.Tel,
            };
            this.Paging(data, linq);
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string id = this.hidID.Value;
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts[id];
            if (del != null)
            {
                del.AbandonError += Del_AbandonError;
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert(e.Message, url, true);
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("删除成功", url, true);
        }
    }
}