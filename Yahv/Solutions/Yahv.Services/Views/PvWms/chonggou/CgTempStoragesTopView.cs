using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class CgTempStoragesTopView<TReponsitory> : QueryView<TempInventory, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CgTempStoragesTopView()
        {
        }

        public CgTempStoragesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<TempInventory> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgTempStoragesTopView>()
                       select new TempInventory
                       {
                           ID = entity.ID,
                           WareHouseID = entity.WareHouseID,
                           SortingID = entity.SortingID,
                           ProductID = entity.ProductID,
                           Total = entity.Total.HasValue ? (decimal)entity.Total : 0,
                           Quantity = entity.Quantity,
                           Origin = (Underly.Origin)Enum.Parse(typeof(Underly.Origin), entity.Origin),
                           IsLock = entity.IsLock,
                           CreateDate = entity.CreateDate,
                           ShelveID = entity.ShelveID,
                           Supplier = entity.Supplier,
                           DateCode = entity.DateCode,
                           Summary = entity.Summary,
                           CustomsName = entity.CustomsName,
                           NoticeID = entity.NoticeID,
                           WaybillID = entity.WaybillID,
                           BoxCode = entity.BoxCode,
                           AdminID = entity.AdminID,
                           Weight = entity.Weight.HasValue ? (decimal)entity.Weight : 0,
                           NetWeight = entity.NetWeight.HasValue ? (decimal)entity.NetWeight : 0,
                           Volume = entity.Volume.HasValue ? (decimal)entity.Volume : 0,
                           PartNumber = entity.PartNumber,
                           Manufacturer = entity.Manufacturer,
                           PackageCase = entity.PackageCase,
                           Packaging = entity.Packaging,
                           EnterCode = entity.EnterCode,
                       };

            return linq;
        }

        /// <summary>
        /// 获取暂存产品
        /// </summary>
        /// <returns></returns>
        public IQueryable<TempInventory> GetTempStorage()
        {
            var linq = this.GetIQueryable().Where(item => item.Quantity > 0);
            return linq;
        }

        /// <summary>
        /// 获取暂存运单
        /// </summary>
        /// <returns></returns>
        public IQueryable<Waybill> GetWaybills()
        {
            var waybillIds = this.GetIQueryable().Where(item => item.Quantity > 0).Select(item => item.WaybillID).Distinct();
            var waybillView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => waybillIds.Contains(item.wbID));
            var linq = from entity in waybillView
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
                               Driver = entity.wldDriver,
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
