using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payee.PayeeApply
{
    public partial class DetailList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var leftID = Request.QueryString["id"];
            var query = new PayeeRightsView().Where(item => item.PayeeLeftID == leftID).ToArray();

            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.PayeeLeftID,
                item.AccountCatalogName,
                item.CreatorName,
                Currency = item.Currency.GetDescription(),
                item.LeftPrice,
                item.RightPrice,
                item.SenderName,
                item.PayerName,
            });
        }
    }
}