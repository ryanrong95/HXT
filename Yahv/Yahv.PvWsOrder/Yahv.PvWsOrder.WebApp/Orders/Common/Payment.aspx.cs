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
    public partial class Payment : ErpParticlePage
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
            var paymentID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.Payments.GetIQueryableEx().Where(item => item.PayableID == paymentID).AsEnumerable();
            var rowList = query.Select(t => new
            {
                ID = t.ID,
                AccountType = t.AccountType.GetDescription(),
                Price = t.Price,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                FlowID = t.FlowID,
                FormCode = t.FormCode,
                AccountCode = t.AccountCode,
                AdminID = t.AdminID,
                AdminName=t.AdminName,
                Summay = t.Summay,
            }).ToList();
            return new
            {
                rows = rowList,
                total = rowList.Count(),
            }.Json();
        }
    }
}