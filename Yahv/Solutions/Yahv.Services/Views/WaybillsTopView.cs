using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 运单通用视图
    /// </summary>
    public class WaybillsTopView<TReponsitory> : UniqueView<Waybill, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WaybillsTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WaybillsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Waybill> GetIQueryable()
        {
            
            var waybillView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WaybillsTopView>();
            //var wayLoadingView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadingsTopView>();
            var linq = from entity in waybillView
                       //join wayload in wayLoadingView on entity.wldID equals wayload.ID into wayloadEntity
                       //from d in wayloadEntity.DefaultIfEmpty()
                       select new Waybill
                       {
                           ID = entity.wbID,
                           Code = entity.wbCode,
                           Type = (WaybillType)entity.wbType,
                           Subcodes = entity.wbSubcodes,
                           CarrierID = entity.wbCarrierID,
                           ConsignorID = entity.wbConsignorID,
                           ConsigneeID = entity.wbConsigneeID,
                           FreightPayer = (WaybillPayer)entity.wbFreightPayer,
                           TotalParts = entity.wbTotalParts,
                           TotalWeight = entity.wbTotalWeight,
                           TotalVolume = entity.wbTotalVolume,
                           CarrierAccount = entity.wbCarrierAccount,
                           VoyageNumber = entity.wbVoyageNumber,
                           Condition = entity.wbCondition,
                           CreateDate = entity.wbCreateDate,
                           ModifyDate = entity.wbModifyDate ?? DateTime.Now,
                           EnterCode = entity.wbEnterCode,
                           Status = (GeneralStatus)entity.wbStatus,
                           CreatorID = entity.wbCreatorID,
                           ModifierID = entity.wbModifierID,
                           TransferID = entity.wbTransferID,
                           IsClearance = entity.wbIsClearance,
                           Packaging = entity.wbPackaging,
                           FatherID = entity.wbFatherID,
                           Summary = entity.wbSummary,
                           Supplier = entity.wbSupplier,
                           ExcuteStatus = entity.wbExcuteStatus,
                           OrderID = entity.OrderID,
                           Source = (CgNoticeSource)entity.Source,
                           NoticeType = (CgNoticeType)entity.NoticeType,
                           TempEnterCode = entity.TempEnterCode,
                           CuttingOrderStatus = entity.CuttingOrderStatus,
                           ConfirmReceiptStatus = entity.ConfirmReceiptStatus,
                           Consignor = new WayParter()
                           {
                               ID = entity.corID,
                               Company = entity.corCompany,
                               Place = entity.corPlace,
                               Address = entity.corAddress,
                               Contact = entity.corContact,
                               Phone = entity.corPhone,
                               Zipcode = entity.corZipcode,
                               Email = entity.corEmail,
                               CreateDate = entity.corCreateDate,
                               IDType = (IDType?)entity.corIDType,
                               IDNumber = entity.corIDNumber,
                           },
                           Consignee = new WayParter()
                           {
                               ID = entity.coeID,
                               Company = entity.coeCompany,
                               Place = entity.coePlace,
                               Address = entity.coeAddress,
                               Contact = entity.coeContact,
                               Phone = entity.coePhone,
                               Zipcode = entity.coeZipcode,
                               Email = entity.coeEmail,
                               CreateDate = entity.coeCreateDate,
                               IDType = (IDType?)entity.coeIDType,
                               IDNumber = entity.coeIDNumber,
                           },
                           WayLoading = new WayLoading()
                           {
                               ID = entity.wldID,
                               TakingDate = entity.wldTakingDate,
                               TakingAddress = entity.wldTakingAddress,
                               TakingContact = entity.wldTakingContact,
                               TakingPhone = entity.wldTakingPhone,
                               CarNumber1 = entity.wldCarNumber1,
                               //CarNumber1 = d.transCarNumber1 + d.transCarNumber2,
                               Driver = entity.wldDriver,
                               //Driver = d.DriverName,
                               Carload = entity.wldCarload,
                               CreateDate = entity.wldCreateDate,
                               ModifyDate = entity.wldModifyDate,
                               CreatorID = entity.wldCreatorID,
                               ModifierID = entity.wldModifierID,
                               LoadingExcuteStatus = (CgLoadingExcuteStauts?)entity.loadExcuteStatus,
                           },
                           WayChcd = new WayChcd()
                           {
                               ID = entity.chcdID,
                               LotNumber = entity.chcdLotNumber,
                               CarNumber1 = entity.chcdCarNumber1,
                               CarNumber2 = entity.chcdCarNumber2,
                               Carload = entity.chcdCarload,
                               IsOnevehicle = entity.chcdIsOnevehicle,
                               Driver = entity.chcdDriver,
                               PlanDate = entity.chcdPlanDate,
                               DepartDate = entity.chcdDepartDate,
                               TotalQuantity = entity.chcdTotalQuantity,
                               Phone = entity.chcdPhone
                           },
                           WayCharge = new WayCharge()
                           {
                               ID = entity.chgID,
                               Payer = (WayChargeType)entity.chgPayer,
                               PayMethod = (Methord)entity.chgPayMethod,
                               Currency = (Currency)entity.chgCurrency,
                               TotalPrice = entity.chgTotalPrice,
                           }
                       };
            return linq;
        }
    }
}
