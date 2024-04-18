using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Entry
{
    public partial class Details : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 深圳入库通知（已入库）
        /// </summary>
        protected void data()
        {
            string entryNoticeID = Request.QueryString["ID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNoticeItems.Where(x=>x.EntryNoticeID==entryNoticeID).OrderBy(x => x.DecList.CaseNo);
            Func<SZEntryNoticeItem, object> convert = item => new
            {
                CaseNumber = item.DecList.CaseNo,
                NetWeight = item.DecList.NetWt,
                GrossWeight = item.DecList.GrossWt,
                ProductName = item.DecList.GName,
                Model = item.DecList.GoodsModel,
                Manufactor = item.DecList.GoodsBrand,
                Quantity = item.DecList.GQty,
                PackingDate = item.DecList.DeclarationNoticeItem.Sorting.CreateDate.ToString("yyyy-MM-dd")
            };
            this.Paging(data, convert);
        }
    }
}