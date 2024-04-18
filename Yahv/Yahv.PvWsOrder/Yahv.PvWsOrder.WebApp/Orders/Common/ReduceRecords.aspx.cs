using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class ReduceRecords : ErpParticlePage
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
            var Currency = Request.QueryString["Currency"];
            var query = Erp.Current.WsOrder.Receiveds
                .Where(item => item.ReceivableID == receivableID && item.AccountType == AccountType.Reduction).AsEnumerable();
            var rowList = query.Select(t => new
            {
                ID = t.ID,
                AccountType = t.AccountType.GetDescription(),
                Price = t.Price,
                Currency = Currency,
                PaidPrice = t.PaidPrice,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                FlowID = t.FlowID,
                AccountCode = t.AccountCode,
              // AdminID = t.AdminID,
                AdminName=t.AdminName,
                Summay = t.Summay,
            }).ToList();
            return new
            {
                rows = rowList,
                total = rowList.Count(),
            }.Json();
        }

        /// <summary>
        /// 撤销减免记录
        /// </summary>
        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];
                PaymentManager.Erp(Erp.Current.ID).Received.ReductionCancel(ID);
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}