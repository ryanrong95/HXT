using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 打印出库单项目(产品)
    /// </summary>
    public class ExitPrintItemsView : UniqueView<Models.HKExitPrintItem, ScCustomsReponsitory>
    {
        public ExitPrintItemsView()
        {
        }

        internal ExitPrintItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKExitPrintItem> GetIQueryable()
        {
            var hKExitNoticeView = new Views.HKExitNoticeView(this.Reponsitory);
            var result = from exitNoticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on exitNoticeItem.DecListID equals decList.ID
                         join declarationNoticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>() on decList.DeclarationNoticeItemID equals declarationNoticeItem.ID
                         join packingItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>() on declarationNoticeItem.SortingID equals packingItem.SortingID
                         join storage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>() on packingItem.SortingID equals storage.SortingID
                         join hKExitNotice in hKExitNoticeView on exitNoticeItem.ExitNoticeID equals hKExitNotice.ID
                         where packingItem.Status == (int)Enums.Status.Normal 
                                && (new Enums.OrderType[] { Enums.OrderType.Inside, Enums.OrderType.Icgoo }).Contains(hKExitNotice.Order.Type)
                                && hKExitNotice.Order.Client.ClientType == Enums.ClientType.Internal
                         select new Models.HKExitPrintItem
                         {
                             ID = exitNoticeItem.ID,
                             ExitNoticeID = exitNoticeItem.ExitNoticeID,
                             DecList = new Models.DecList
                             {
                                 ID = decList.ID,
                                 CaseNo = decList.CaseNo,
                                 GoodsModel = decList.GoodsModel,
                                 GQty = decList.GQty,
                             },
                             StoreStorage = new Models.StoreStorage
                             {
                                 ID = storage.ID,
                                 StockCode = storage.StockCode,
                             },
                             HKExitNotice = hKExitNotice,
                         };

            return result;
        }
    }
}
