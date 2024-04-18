using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class BillNew : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.VouchersCnyStatistics
                .Where(item => item.OrderID == orderid)
                .Where(item => item.PayeeID == PvWsOrder.Services.Common.Helper.XdtCompanyID)
                .Where(item => item.Business == Payments.ConductConsts.供应链)
                .Where(t => t.Subject != Payments.SubjectConsts.代付货款 && t.Subject != Payments.SubjectConsts.代收货款)
                .AsEnumerable();
            var linq = query.Select(t => new
            {
                t.ReceivableID,
                t.OrderID,
                t.Catalog,
                t.Subject,
                PayeeName = t.Payee.Name,
                PayerName = t.Payer.Name,
                PayeeID = t.Payee.ID,
                PayerID = t.Payer.ID,

                OriginCurrency = t.OriginCurrency.GetCurrency().ShortName,
                OriginPrice = t.OriginPrice.ToString("f2"),
                Rate = t.Rate.ToString("f4"),
                Currency = t.Currency.GetCurrency().ShortName,
                LeftPrice = t.LeftPrice.ToString("f2"),
                RightPrice = (t.RightPrice ?? 0).ToString("f2"),
                ReducePrice = (t.ReducePrice ?? 0).ToString("f2"),
                Remains = t.Remains.ToString("f2"),
                OriginDate = t.LeftDate.ToString("yyyy-MM-dd"),

                ConfirmPrice = t.LeftPrice.ToString("f2"),
                AdminName = t.Admin.RealName,
            });
            return new
            {
                rows = linq.OrderBy(item => item.OriginDate).ToArray(),
                total = linq.Count()
            };
        }
    }
}