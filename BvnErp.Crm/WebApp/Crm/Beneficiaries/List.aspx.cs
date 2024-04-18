using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Beneficiaries
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            var beneift = Needs.Erp.ErpPlot.Current.ClientSolutions.Beneficiaries;
            Func<NtErp.Crm.Services.Models.Beneficiaries, object> linq = c => new
            {
                ID = c.ID,
                Bank = c.Bank,
                BankCode=c.BankCode,
                Address=c.Address,
                CompanyName=c.Company.Name
            };
            this.Paging(beneift, linq);
        }

        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Beneficiaries[id];
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