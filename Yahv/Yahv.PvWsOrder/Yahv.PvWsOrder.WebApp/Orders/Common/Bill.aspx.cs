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
    public partial class Bill : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 收款
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.VouchersCnyStatistics
                .Where(item => item.OrderID == orderid)
                .Where(item => item.Business == Payments.ConductConsts.供应链)
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

        protected object PaymentData()
        {
            var orderid = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.Alls.PaymentsStatisticsRoll(orderid).AsEnumerable();
            return query.Select(t => new
            {
                t.PayableID,
                t.OrderID,
                Date = t.LeftDate.ToString("yyyy-MM-dd"),
                t.PayeeName,
                t.PayerName,
                t.Catalog,
                t.Subject,
                t.LeftPrice,
                t.RightPrice,
                t.Remains,
                Currency = t.Currency.GetCurrency().ShortName,
                OriginDate = t.LeftDate.ToString("yyyy-MM-dd"),
                t.AdminName,
            }).ToList();

        }
    }
}