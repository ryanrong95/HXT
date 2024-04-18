using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class XmlItems : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            string InvoiceXmlID = Request.QueryString["InvoiceXmlID"];

            var InvoiceXmlIDItems = new InvoiceNoticeXmlItemView()
             .Where(item => item.InvoiceNoticeXmlID == InvoiceXmlID).OrderBy(item => item.Xh)
             .AsQueryable();

            Func<Needs.Ccs.Services.Models.InvoiceNoticeXmlItem, object> convert = xml => new
            {
                xml.Spmc,
                xml.Ggxh,
                xml.Jldw,
                xml.Sl,
                xml.Dj,
                xml.Je,
                xml.Slv,
                xml.Se
            };

            Response.Write(new
            {
                rows = InvoiceXmlIDItems.Select(convert).ToArray(),
                total = InvoiceXmlIDItems.Count()
            }.Json());

        }
    }
}