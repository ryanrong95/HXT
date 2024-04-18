using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Views
{
    public class ClientNoticesView<TReponsitory> : QueryView<object, TReponsitory>
        where TReponsitory : class, IReponsitory, IDisposable, new()
    {

        public ClientNoticesView()
        {

        }

        public ClientNoticesView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<object> GetIQueryable()
        {
            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                   on notice.InputID equals input.ID
                   join waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                    on notice.WaybillID equals waybill.wbID
                   join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                    on notice.ProductID equals product.ID
                   where notice.Type == (int)NoticeType.CustomsOut
                   select new
                   {
                       ID = notice.ID,
                       WareHouseID = notice.WareHouseID,
                       WaybillID = notice.WaybillID,
                       CreateDate = notice.CreateDate,
                       BoxCode = notice.BoxCode,
                       Weight = notice.Weight,
                       NetWeight = notice.NetWeight,
                       IsOutput = (Underly.CgPickingExcuteStatus)waybill.wbExcuteStatus == Underly.CgPickingExcuteStatus.Completed,//是否出库
                       Quantity = notice.Quantity,
                       Product = new
                       {
                           PartNumber = product.PartNumber,
                           Manufacturer = product.Manufacturer,
                           PackageCase = product.PackageCase,
                           Packaging = product.Packaging,
                       },
                       Input = new
                       {
                           OrderID = input.OrderID,
                           ClientID = input.ClientID,
                       },
                       waybillType = (Underly.WaybillType)waybill.wbType,
                       AppointTime1 = waybill.AppointTime,
                       AppointTime2 = waybill.wldTakingDate
                   };
        }
    }
}
