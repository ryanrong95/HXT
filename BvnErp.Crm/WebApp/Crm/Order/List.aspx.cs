using Needs.Underly;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Order
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            var order = Needs.Erp.ErpPlot.Current.ClientSolutions.Order;
            Func<NtErp.Crm.Services.Models.Order, object> linq = c => new
            {
                ID = c.ID,
                ClientName = c.Client.Name,
                AdminName = c.Admin.RealName,
                CurrencyName = c.Currency.GetDescription(),
                BeneficiaryName = c.Beneficiaries.BankCode,
                DeliveryAddress = c.DeliveryAddress,
                Address = c.Address,
                ConsigneeName = c.Contact.Name
            };
            this.Paging(order, linq);
        }

        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Order[id];
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