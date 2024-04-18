using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Entry
{
    public partial class Entrying : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 深圳入库通知
        /// </summary>
        protected void data()
        {
            string EntryNoticeID = Request.QueryString["ID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNoticeItems.Where(x=>x.EntryNoticeID==EntryNoticeID).OrderBy(x=>x.DecList.CaseNo);
            //var data = entryNotice.SZItems;
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

        /// <summary>
        /// 入库
        /// </summary>
        protected void Entry()
        {
            try
            {
                string ID = Request.Form["ID"];
                string StockCode = Request.Form["StockCode"];

                var EntryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNotice[ID];
                EntryNotice.EntryNoticeStatus = Needs.Ccs.Services.Enums.EntryNoticeStatus.Boxed;
                //暂时以uodatetime作为通知的实际入库时间 ryan
                EntryNotice.UpdateDate = DateTime.Now;
                //入库
                EntryNotice.SetAdmin(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                EntryNotice.Entry(StockCode);
                Response.Write((new { success = true, message = "入库成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "入库失败" + ex.Message }).Json());
            }
        }
    }
}