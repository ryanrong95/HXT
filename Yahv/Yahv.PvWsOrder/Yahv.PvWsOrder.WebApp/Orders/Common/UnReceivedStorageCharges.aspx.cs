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
    public partial class UnReceivedStorageCharges : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = new PvWsOrder.Services.Views.Alls.UnReceivedStorageChargesView().AsEnumerable();
            var rowList = query.Select(t => new
            {
                ID = t.ReceivableID,
                OrderID = t.OrderID,
                PayeeName = t.PayeeName,
                PayerName = t.PayerName,
                Catalog = t.Catalog,
                Subject = t.Subject,
                LeftPrice = t.LeftPrice,
                RightPrice = t.RightPrice,
                ReducePrice=t.ReduceTotalPrice,
                Remains = t.Remains,
                Currency = t.Currency.GetCurrency().ShortName,
                LeftDate = t.LeftDate.ToString("yyyy-MM-dd"),
                AdminName = t.AdminName,
            }).ToList();
            return new
            {
                rows = rowList,
                total = rowList.Count(),
            }.Json();
        }
    }
}