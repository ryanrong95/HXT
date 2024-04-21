using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.AutoQuotes
{
    public partial class Edit : BasePage
    {
        protected string SupplierID
        {
            get
            {
                return Request.QueryString["supplierid"];
            }
        }
        protected string AutoQuoteID
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var entity = new SuppliersRoll()[SupplierID].AutoQuotes[AutoQuoteID];
                if (entity != null)
                {
                    this.Model = new
                    {
                        entity.ID,
                        entity.Name,
                        entity.Supplier,
                        entity.Manufacturer,
                        entity.PackageCase,
                        entity.Packaging,
                        entity.DateCode,
                        entity.Deadline,
                        entity.Quantity,
                        entity.UnitPrice
                    };
                }
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new SuppliersRoll()[SupplierID].AutoQuotes[AutoQuoteID] ?? new AutoQuote();
            if (entity != null && !string.IsNullOrWhiteSpace(SupplierID))
            {
                entity.SupplierID = SupplierID;
                entity.Name = Request.Form["Name"];
                entity.Supplier = Request.Form["Supplier"];
                entity.Manufacturer = Request.Form["Manufacturer"];
                entity.PackageCase = Request.Form["PackageCase"];
                entity.Packaging = Request.Form["Packaging"];
                entity.DateCode = Request.Form["DateCode"];
                entity.Deadline = DateTime.Parse(Request.Form["Deadline"]);
                entity.Quantity = Request.Form["Quantity"];
                entity.UnitPrice = Request.Form["UnitPrice"];
                entity.ReporterID = Yahv.Erp.Current.ID;//报价人
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Reload("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info);
        }
    }
}