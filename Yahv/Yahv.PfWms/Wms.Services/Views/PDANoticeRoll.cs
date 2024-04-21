using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services.Models;
using Yahv.Utils.Serializers;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    public class PDANoticeRoll : QueryView<Models.PDANotices, PvWmsRepository>
    {
        protected override IQueryable<PDANotices> GetIQueryable()
        {
            var productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   join product in productView on notice.ProductID equals product.ID into pro
                   from product in pro.DefaultIfEmpty()
                   join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID into inputs
                   from input in inputs.DefaultIfEmpty()
                   select new Models.PDANotices
                   {
                       ID = notice.ID,
                       Type = (Yahv.Services.Enums.CgNoticeType)notice.Type,
                       WareHouseID = notice.WareHouseID,
                       WaybillID = notice.WaybillID,
                       InputID = notice.InputID,
                       OutputID = notice.OutputID,
                       ProductID = notice.ProductID,
                       Supplier = notice.Supplier,
                       DateCode = notice.DateCode,
                       Quantity = notice.Quantity,
                       Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                       CreateDate = notice.CreateDate,
                       Status = (Yahv.Services.Enums.NoticesStatus)notice.Status,
                       Source = (CgNoticeSource)notice.Source,
                       Target = (NoticesTarget)notice.Target,
                       BoxCode = notice.BoxCode,
                       Weight = notice.Weight,
                       NetWeight = notice.NetWeight,
                       Volume = notice.Volume,
                       ShelveID = notice.ShelveID,
                       Origin = notice.Origin,
                       Product = product,
                       Input = input == null ? null : new Input
                       {
                           ID = input.ID,
                           Code = input.Code,
                           OriginID = input.OriginID,
                           OrderID = input.OrderID,
                           TinyOrderID=input.TinyOrderID,
                           ItemID = input.ItemID,
                           ProductID = input.ProductID,
                           ClientID = input.ClientID,
                           PayeeID = input.PayeeID,
                           ThirdID = input.ThirdID,
                           TrackerID = input.TrackerID,
                           SalerID = input.SalerID,
                           PurchaserID = input.PurchaserID,
                           Currency = (Currency)input.Currency,
                           UnitPrice = input.UnitPrice,
                           CreateDate = input.CreateDate,
                       }

                   };
        }
    }
}
