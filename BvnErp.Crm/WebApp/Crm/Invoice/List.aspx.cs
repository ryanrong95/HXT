using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Invoice
{
    /// <summary>
    /// 发票信息展示页面
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
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string clientid = Request.QueryString["ClientID"];
            var client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(clientid);
            string clientName = client.Name;
            var Invoices = Needs.Erp.ErpPlot.Current.ClientSolutions.MyInvoices;
            var data = Invoices.Where(item => item.ClientID == clientid);
            Func<NtErp.Crm.Services.Models.Invoice, object> linq = item => new
            {
                ID = item.ID,
                Type = item.InvoiceTypes.GetDescription(),
                CompanyName = clientName,
                CompanyCode = item.CompanyCode,
                Phone = item.Phone,
                Bank = item.BankName,
                Account = item.Account,
                Contact = item.Contacts.Name,
            };
            this.Paging(data, linq);
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.MyInvoices[id];
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}