using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 运单视图
    /// </summary>
    public class WaybillsAlls : UniqueView<Waybill, PvWsOrderReponsitory>
    {
        public WaybillsAlls()
        {

        }

        internal WaybillsAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Waybill> GetIQueryable()
        {
            var waybillTopView = new Yahv.Services.Views.WaybillsTopView<PvWsOrderReponsitory>(this.Reponsitory);
            var linq = from waybill in waybillTopView
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
                           FatherID = waybill.FatherID,
                           Supplier = waybill.Supplier,
                           ExcuteStatus = waybill.ExcuteStatus,
                           Summary = waybill.Summary,

                           OrderID = waybill.OrderID,
                           Source = waybill.Source,
                           NoticeType = waybill.NoticeType,
                           TempEnterCode = waybill.TempEnterCode,
                           CuttingOrderStatus = waybill.CuttingOrderStatus,
                           ConfirmReceiptStatus = waybill.ConfirmReceiptStatus,

                           Consignee = waybill.Consignee,
                           Consignor = waybill.Consignor,
                           WayLoading = waybill.WayLoading,
                           WayChcd = waybill.WayChcd,
                           WayCharge = waybill.WayCharge,
                       };
            return linq;
        }
    }
}
