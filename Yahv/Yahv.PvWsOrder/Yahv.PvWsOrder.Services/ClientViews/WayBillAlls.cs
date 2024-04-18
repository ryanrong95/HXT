using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class WayBillAlls : Yahv.Linq.UniqueView<Waybill,PvWsOrderReponsitory> 
    {
        public WayBillAlls()
        {

        }

        public WayBillAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Waybill> GetIQueryable()
        {
            return from waybill in new WaybillsTopView<PvWsOrderReponsitory>(this.Reponsitory)
                   select new Waybill
                   {
                       ID = waybill.ID,
                       Code = waybill.Code,
                       Type = waybill.Type,
                       Subcodes = waybill.Subcodes,
                       CarrierID = waybill.CarrierID,
                       ConsignorID = waybill.ConsignorID,
                       ConsigneeID = waybill.ConsigneeID,
                       FreightPayer = waybill.FreightPayer,
                       TotalParts = waybill.TotalParts,
                       TotalWeight = waybill.TotalWeight,
                       TotalVolume = waybill.TotalVolume,
                       CarrierAccount = waybill.CarrierAccount,
                       VoyageNumber = waybill.VoyageNumber,
                       Condition = waybill.Condition,
                       CreateDate = waybill.CreateDate,
                       ModifyDate = waybill.ModifyDate,
                       EnterCode = waybill.EnterCode,
                       Status = waybill.Status,
                       CreatorID = waybill.CreatorID,
                       ModifierID = waybill.ModifierID,
                       TransferID = waybill.TransferID,
                       IsClearance = waybill.IsClearance,
                       Packaging = waybill.Packaging,
                       Consignee = waybill.Consignee,
                       Consignor = waybill.Consignor,
                       WayChcd = waybill.WayChcd,
                       WayLoading = waybill.WayLoading,
                       WayCharge=waybill.WayCharge,
                       FatherID = waybill.FatherID,
                       Summary = waybill.Summary,
                       Supplier = waybill.Supplier,
                       ExcuteStatus = waybill.ExcuteStatus,
                       CuttingOrderStatus = waybill.CuttingOrderStatus,
                       ConfirmReceiptStatus = waybill.ConfirmReceiptStatus,
                   };
        }
    }
}
