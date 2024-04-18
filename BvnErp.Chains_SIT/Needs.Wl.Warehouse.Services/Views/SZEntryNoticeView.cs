using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.Views
{
    /// <summary>
    /// 深圳库房入库通知视图
    /// </summary>
    public class SZEntryNoticeView : View<PageModels.SZWarehouseEntryListModel, ScCustomsReponsitory>
    {
        public SZEntryNoticeView()
        {
        }

        internal SZEntryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PageModels.SZWarehouseEntryListModel> GetIQueryable()
        {
            return from entryNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on entryNotice.OrderID equals order.ID
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on entryNotice.DecHeadID equals decHead.ID into decHeads
                   from decHead in decHeads.DefaultIfEmpty()
                   where entryNotice.WarehouseType == (int)Needs.Wl.Models.Enums.WarehouseType.ShenZhen
                   orderby entryNotice.CreateDate descending
                   select new PageModels.SZWarehouseEntryListModel
                   {
                       ID = entryNotice.ID,
                       OrderID = order.ID,
                       DecHeadID = decHead.ID,
                       PackNo = decHead.PackNo,
                       CompanyName = company.Name,
                       ClientCode = entryNotice.ClientCode,
                       WarehouseType = (Needs.Wl.Models.Enums.WarehouseType)entryNotice.WarehouseType,
                       EntryNoticeStatus = (Needs.Wl.Models.Enums.EntryNoticeStatus)entryNotice.EntryNoticeStatus,
                       Status = entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                   };
        }
    }
}