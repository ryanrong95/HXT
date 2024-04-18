using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class Deduction : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();

            string OrderIDs = Request.QueryString["OrderIDs"];
            string[] arrOrderID = OrderIDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
            var addedValueTaxTotalModels = new Needs.Ccs.Services.Views.AddedValueTaxTotalView().GetAddedValueTaxTotal(arrOrderID);

            decimal addedValueTaxTotal = addedValueTaxTotalModels.Sum(t => t.TotalAmount);
            decimal orderCount = addedValueTaxTotalModels.Count();

            this.Model.AddedValueTaxTotal = addedValueTaxTotal;
            this.Model.OrderCount = orderCount;
        }

        /// <summary>
        /// 确认抵扣
        /// </summary>
        protected void Submit()
        {
            try
            {
                string DeductionTime = Request.Form["DeductionTime"];
                string IDs = Request.Form["IDs"];
                string[] arrId = IDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                UnDeductionDecTax decTax = new UnDeductionDecTax(arrId, Convert.ToDateTime(DeductionTime));
                decTax.Deduction();
                Response.Write((new { success = true, message = "抵扣成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "抵扣失败：" + ex.Message }).Json());
            }
        }
    }
}