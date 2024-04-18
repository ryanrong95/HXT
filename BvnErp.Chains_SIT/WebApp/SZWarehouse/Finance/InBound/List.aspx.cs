using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Finance.InBound
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string ContrNO = Request.QueryString["ContrNo"];
            string EntryId = Request.QueryString["EntryId"];
            string OwnerName = Request.QueryString["OwnerName"];
            string GoodsModel = Request.QueryString["GoodsModel"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNotice.AsQueryable();


            if (!string.IsNullOrEmpty(ContrNO))
            {
                ContrNO = ContrNO.Trim();
                notice = notice.Where(t => t.DecHead.ContrNo == ContrNO);
            }
            if (!string.IsNullOrEmpty(EntryId))
            {
                EntryId = EntryId.Trim();
                notice = notice.Where(t => t.DecHead.EntryId == EntryId);
            }
            if (!string.IsNullOrEmpty(OwnerName))
            {
                OwnerName = OwnerName.Trim();
                notice = notice.Where(t => t.DecHead.OwnerName.Contains(OwnerName));
            }
            if (!string.IsNullOrEmpty(GoodsModel))
            {
                notice = notice.Where(t => t.SZItems.Any(item => item.DecList.GoodsModel == GoodsModel));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                notice = notice.Where(t => t.UpdateDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate).AddDays(1);
                notice = notice.Where(t => t.UpdateDate <= to);
            }

            Func<Needs.Ccs.Services.Models.SZEntryNotice, object> convert = decnotice => new
            {
                ID = decnotice.ID,
                ContrNO = decnotice.DecHead.ContrNo,
                EntryId = decnotice.DecHead.EntryId,
                OwnerName = decnotice.DecHead.OwnerName,
                GoodsModel = decnotice.DecHead.Lists.Select(item=>item.GoodsModel).FirstOrDefault(),
                DDate = decnotice.UpdateDate.ToString("yyyy-MM-dd"),
                TotalAmount = decnotice.DecHead.Lists.Sum(item=>item.GQty),
                DeclTotal = decnotice.DecHead.Lists.Sum(item => item.DeclTotal),
            };
            this.Paging(notice, convert);
        }
    }
}