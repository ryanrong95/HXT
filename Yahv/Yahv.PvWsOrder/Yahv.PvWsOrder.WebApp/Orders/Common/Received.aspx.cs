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
    public partial class Received : ErpParticlePage
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
            var receivableID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.Receiveds.GetIQueryableEx().Where(item => item.ReceivableID == receivableID).AsEnumerable();
            var rowList = query.Select(t => new
            {
                ID = t.ID,
                AccountType = t.AccountType.GetDescription(),
                Price = t.Price,
                PaidPrice = t.PaidPrice,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                FlowID = t.FlowID,
                FormCode = t.FormCode,
                AccountCode = t.AccountCode,
                AdminID = t.AdminID,
                AdminName = t.AdminName,
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