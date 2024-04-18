using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class ReceivingImportItemsQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string ImportID = Request.QueryString["ImportID"];

            var InvoiceXmlIDItems = new MKDeclareImportItemQueryView()
             .Where(item => item.ImportID == ImportID)
             .AsQueryable();

            Func<Needs.Ccs.Services.Models.ReImportItemModel, object> convert = xml => new
            {
                xml.ID,
                xml.ImportID,
                xml.FinanceRepID,
                xml.Seq,
                xml.USD,
                xml.RMB,
                xml.DeclareRate,
                xml.Currency
            };

            Response.Write(new
            {
                rows = InvoiceXmlIDItems.Select(convert).ToArray(),
                total = InvoiceXmlIDItems.Count()
            }.Json());

        }
    }
}