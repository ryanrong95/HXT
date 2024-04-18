using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class AddWaybill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //快递公司
            this.Model.ExpressCompanyData = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(t => t.Code == "SF" || t.Code == "EMS").
                Select(item => new { Value = item.ID, Text = item.Name }).Json();
        }

        /// <summary>
        /// 用于手动维护发票运单号
        /// </summary>
        protected void Save() 
        {

            string InvoiceNotice = Request["InvoiceNotice"];
            string[] InvoiceNotices = InvoiceNotice.Split(',');
            string ExpressName = Request["ExpressName"];
            string WaybillCode = Request["WaybillCode"];

            try {

                foreach (var invoiceid in InvoiceNotices)
                {
                    var entity = new Needs.Ccs.Services.Models.InvoiceWaybill();
                    entity.InvoiceNotice = new Needs.Ccs.Services.Models.InvoiceNotice() { ID = invoiceid };
                    entity.CompanyName = ExpressName;
                    entity.WaybillCode = WaybillCode;

                    entity.Enter();
                }

                Response.Write((new { success = "true", message = "保存成功"}).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "保存失败：" + ex.Message}).Json());
            }
        }
    }
}