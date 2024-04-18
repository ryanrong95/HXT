using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// /// <summary>
    /// 
    /// </summary>
    /// </summary>
    public class VoyagesView : UniqueView<Models.Voyage, ScCustomsReponsitory>
    {
        private string ManifestID;

        public VoyagesView()
        {
        }

        internal VoyagesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        public VoyagesView(string manifestID) : this()
        {
            this.ManifestID = manifestID;
        }

        protected override IQueryable<Models.Voyage> GetIQueryable()
        {
            var carriersView = new CarriersView(this.Reponsitory);
            var orderVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();

            return from voyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                   join carrier in carriersView on voyage.CarrierCode equals carrier.Code into carriers
                   from carrier in carriers.DefaultIfEmpty()
                   where (carrier.CarrierType == Enums.CarrierType.InteLogistics || carrier == null)
                   && voyage.Status == (int)Enums.Status.Normal
                   //&& !orderVoyagesView.Any(s => s.VoyageID == voyage.ID)
                   orderby voyage.CutStatus ascending, voyage.CreateTime descending 
                   select new Models.Voyage
                   {
                       ID = voyage.ID,
                       HKLicense = voyage.HKLicense,
                       DriverCode = voyage.DriverCode,
                       DriverName = voyage.DriverName,
                       HKDeclareStatus = voyage == null ? false : voyage.HKDeclareStatus,
                       Summary = voyage.Summary,
                       Carrier = carrier,
                       CreateTime = voyage.CreateTime,
                       CutStatus = (Enums.CutStatus)voyage.CutStatus,
                       TransportTime = voyage.TransportTime,
                       Type = (Enums.VoyageType)voyage.Type,
                       Status = (Enums.Status)voyage.Status,
                       UpdateDate = voyage.UpdateDate,
                       
                       CarrierType = voyage.CarrierType,
                       CarrierName = voyage.CarrierName,
                       CarrierQueryMark = voyage.CarrierQueryMark,
                       ContactMobile = voyage.ContactMobile,
                       CarrierAddress = voyage.CarrierAddress,
                       ContactName = voyage.ContactName,
                       ContactFax = voyage.ContactFax,
                       VehicleType = voyage.VehicleType,
                       VehicleLicence = voyage.VehicleLicence,
                       VehicleWeight = voyage.VehicleWeight,
                       VehicleSize = voyage.VehicleSize,
                       DriverMobile = voyage.DriverMobile,
                       DriverHSCode = voyage.DriverHSCode,
                       DriverHKMobile = voyage.DriverHKMobile,
                       DriverCardNo = voyage.DriverCardNo,
                       DriverPortElecNo = voyage.DriverPortElecNo,
                       DriverLaoPaoCode = voyage.DriverLaoPaoCode,
                       CarrierCode = carrier != null ? (carrier.Code ?? "") : "",
                       HKSealNumber = voyage.HKSealNumber,
                   };
        }
    }

    public class WayBillManifestView : UniqueView<Models.OutputWayBill, ScCustomsReponsitory>
    {
        private string BillNo;

        public WayBillManifestView()
        {
        }

        internal WayBillManifestView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        public WayBillManifestView(string billNo) : this()
        {
            this.BillNo = billNo;
        }

        protected override IQueryable<Models.OutputWayBill> GetIQueryable()
        {
            var view = new ManifestConsignmentsView(this.Reponsitory);
            return from bill in view
                   join manifest in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>() on bill.Manifest.ID equals manifest.ID
                   //join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on bill.ID equals decHead.BillNo
                   //  where bill.ID == this.BillNo
                   select new Models.OutputWayBill
                   {
                       ID = bill.ID,
                       ConditionCode = bill.ConditionCode,
                       Voyage = new Models.Voyage
                       {
                           ID = manifest.ID,
                           HKLicense = manifest.HKLicense,
                           DriverCode = manifest.DriverCode,
                           DriverName = manifest.DriverName,
                           HKDeclareStatus = manifest.HKDeclareStatus,
                       },
                       PaymentType = bill.PaymentType,
                       GovProcedureCode = bill.GovProcedureCode,
                       TransitDestination = bill.TransitDestination,
                       PackNum = bill.PackNum,
                       PackType = bill.PackType,
                       Cube = bill.Cube,
                       GrossWt = bill.GrossWt,
                       GoodsValue = bill.GoodsValue,
                       Currency = bill.Currency,
                       Consolidator = bill.Consolidator,
                       ConsignorName = bill.ConsignorName
                   };
        }
    }



    public class OrderListVoyagesView : UniqueView<Models.Voyage, ScCustomsReponsitory>
    {

        public OrderListVoyagesView()
        {
        }

        internal OrderListVoyagesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        public OrderListVoyagesView(string manifestID) : this()
        {
        }

        protected override IQueryable<Models.Voyage> GetIQueryable()
        {
            var carriersView = new CarriersView(this.Reponsitory);

            return from voyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                   join carrier in carriersView on voyage.CarrierCode equals carrier.Code into carriers
                   from carrier in carriers.DefaultIfEmpty()
                   where (carrier.CarrierType == Enums.CarrierType.InteLogistics || carrier == null) && voyage.Status == (int)Enums.Status.Normal
                   select new Models.Voyage
                   {
                       ID = voyage.ID,
                       HKLicense = voyage.HKLicense,
                       DriverCode = voyage.DriverCode,
                       DriverName = voyage.DriverName,
                       HKDeclareStatus = voyage == null ? false : voyage.HKDeclareStatus,
                       Summary = voyage.Summary,
                       Carrier = carrier,
                       CreateTime = voyage.CreateTime,
                       CutStatus = (Enums.CutStatus)voyage.CutStatus,
                   };
        }
    }

    public class OrderVoyagesOriginView : UniqueView<Models.OrderVoyage, ScCustomsReponsitory>
    {
        public OrderVoyagesOriginView()
        {
        }

        public OrderVoyagesOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderVoyage> GetIQueryable()
        {
            return from orderVoyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderVoyage.OrderID equals order.ID
                   where orderVoyage.Status == (int)Enums.Status.Normal
                   select new Models.OrderVoyage
                   {
                       ID = orderVoyage.ID,
                       Order = new Models.Order { ID = orderVoyage.OrderID, OrderStatus = (OrderStatus)order.OrderStatus },
                       Type = (Enums.OrderSpecialType)orderVoyage.Type,
                       Status = (Enums.Status)orderVoyage.Status,
                       CreateTime = orderVoyage.CreateDate,
                       UpdateDate = orderVoyage.UpdateDate,
                       Summary = orderVoyage.Summary,
                   };
        }
    }

}
