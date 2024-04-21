using System;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class PastsHistory : ErpParticlePage
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
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            var pastwageitems = Alls.Current.PastsWageItems.Where(item => item.StaffID == id).OrderByDescending(item=>item.CreateDate).ToList();
            var data = from past in pastwageitems
                       group new { past.WageItemName, past.DefaultValue, past.CreateDate, past.AdminName }
                       by new { past.AdminID, past.CreateDate } into pasts
                       select new
                       {
                           WageItem = string.Join(",", pasts.Select(item => item.WageItemName + ":" + (item.DefaultValue?.ToString("#0.00")) ).ToArray()),
                           CreateDate = pasts.FirstOrDefault().CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           pasts.FirstOrDefault().AdminName,
                       };
            return data;
        }
    }
}