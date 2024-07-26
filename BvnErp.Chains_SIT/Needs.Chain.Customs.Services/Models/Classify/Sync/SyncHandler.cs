using Layer.Data.Sqls;
using Needs.Ccs.Services.ApiSettings;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 数据同步基类
    /// </summary>
    public abstract class SyncHandlerBase
    {
        protected IEnumerable<ClassifyProduct> cps;

        public SyncHandlerBase For(params ClassifyProduct[] cps)
        {
            this.cps = cps;
            return this;
        }

        /// <summary>
        /// 做数据同步
        /// </summary>
        /// <returns></returns>
        public abstract JMessage DoSync();
    }

    /// <summary>
    /// 归类数据同步
    /// </summary>
    public class Classify_SyncHandler : SyncHandlerBase
    {
        #region 属性

        private PvDataApiSetting apisetting;

        #endregion

        #region 构造器

        internal Classify_SyncHandler(PvDataApiSetting apisetting)
        {
            this.apisetting = apisetting;
        }

        #endregion

        public override JMessage DoSync()
        {
            var data = this.cps.Select(cp => new
            {
                ItemID = cp.ID,
                PartNumber = cp.Model,
                Manufacturer = cp.Manufacturer,

                HSCode = cp.Category.HSCode,
                TariffName = cp.Category.Name,
                TaxCode = cp.Category.TaxCode,
                TaxName = cp.Category.TaxName,
                LegalUnit1 = cp.Category.Unit1,
                LegalUnit2 = cp.Category.Unit2,
                VATRate = cp.AddedValueTax.Rate,
                ImportPreferentialTaxRate = cp.ImportTax.ImportPreferentialTaxRate,
                OriginRate = cp.ImportTax.OriginRate,
                ExciseTaxRate = cp.ExciseTax?.Rate ?? 0m,
                CIQCode = cp.Category.CIQCode,
                Elements = cp.Category.Elements,

                Ccc = (cp.Category.Type & Enums.ItemCategoryType.CCC) > 0,
                Embargo = (cp.Category.Type & Enums.ItemCategoryType.Forbid) > 0,
                HkControl = (cp.Category.Type & Enums.ItemCategoryType.HKForbid) > 0,
                Coo = (cp.Category.Type & Enums.ItemCategoryType.OriginProof) > 0,
                CIQ = (cp.Category.Type & Enums.ItemCategoryType.Inspection) > 0,
                CIQprice = cp.InspectionFee.GetValueOrDefault(),
                IsHighPrice = (cp.Category.Type & Enums.ItemCategoryType.HighValue) > 0,
                IsDisinfected = (cp.Category.Type & Enums.ItemCategoryType.Quarantine) > 0,

                CreatorID = Icgoo.DefaultCreator,

                CreateDate = cp.Category.CreateDate,
                UpdateDate = cp.Category.UpdateDate
            });

            var url = ConfigurationManager.AppSettings[this.apisetting.ApiName] + this.apisetting.SyncClassified;
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
            {
                results = data.ToList()
            });

            if (result.code == 300)
            {
                throw new Exception("归类数据同步异常 - " + result.data);
            }

            return result;
        }
    }

    /// <summary>
    /// 税费变更数据同步
    /// </summary>
    public class TaxChange_SyncHandler : SyncHandlerBase
    {
        #region 属性

        private PvDataApiSetting apisetting;

        #endregion

        #region 构造器

        internal TaxChange_SyncHandler(PvDataApiSetting apisetting)
        {
            this.apisetting = apisetting;
        }

        #endregion

        public override JMessage DoSync()
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                var itemIDs = this.cps.Select(cp => cp.ID).Distinct().ToArray();
                var items = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(item => itemIDs.Contains(item.ID)).ToArray();
                var categories = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(oic => itemIDs.Contains(oic.OrderItemID)).ToArray();
                var taxes = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Where(oit => itemIDs.Contains(oit.OrderItemID)).ToArray();

                var data = from entity in items
                           join category in categories on entity.ID equals category.OrderItemID
                           join importTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax) on entity.ID equals importTax.OrderItemID
                           join addedValueTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax) on entity.ID equals addedValueTax.OrderItemID
                           join exciseTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.ConsumeTax) on entity.ID equals exciseTax.OrderItemID into exciseTaxes
                           from exciseTax in exciseTaxes.DefaultIfEmpty()
                           select new
                           {
                               ItemID = entity.ID,
                               PartNumber = entity.Model,
                               Manufacturer = entity.Manufacturer,

                               HSCode = category.HSCode,
                               TariffName = category.Name,
                               TaxCode = category.TaxCode,
                               TaxName = category.TaxName,
                               LegalUnit1 = category.Unit1,
                               LegalUnit2 = category.Unit2,
                               VATRate = addedValueTax.Rate,
                               ImportPreferentialTaxRate = importTax.Rate,
                               OriginRate = 0m,
                               ExciseTaxRate = exciseTax?.Rate ?? 0m,
                               CIQCode = category.CIQCode,
                               Elements = category.Elements,

                               CreatorID = category.ClassifySecondOperator,

                               CreateDate = category.CreateDate,
                               UpdateDate = category.UpdateDate
                           };

                var url = ConfigurationManager.AppSettings[this.apisetting.ApiName] + this.apisetting.SyncTaxChanged;
                var result = Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
                {
                    results = data.ToList()
                });

                if (result.code == 300)
                {
                    throw new Exception("税费变更数据同步异常 - " + result.data);
                }

                return result;
            }
        }
    }

    /// <summary>
    /// 归类数据补偿，仅用于线上代仓储出现归类数据丢失，需要人工从华芯通同步数据的情况
    /// </summary>
    public class Classify_Compensation : SyncHandlerBase
    {
        #region 属性

        private PvDataApiSetting apisetting;

        #endregion

        #region 构造器

        internal Classify_Compensation(PvDataApiSetting apisetting)
        {
            this.apisetting = apisetting;
        }

        #endregion

        public override JMessage DoSync()
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                var itemIDs = this.cps.Select(cp => cp.ID).ToArray();
                var items = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(item => itemIDs.Contains(item.ID)).ToArray();
                var categories = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(oic => itemIDs.Contains(oic.OrderItemID)).ToArray();
                var taxes = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Where(oit => itemIDs.Contains(oit.OrderItemID)).ToArray();
                var premiums = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(op => itemIDs.Contains(op.OrderItemID)).ToArray();

                var data = from cp in items
                           join category in categories on cp.ID equals category.OrderItemID
                           join importTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax) on cp.ID equals importTax.OrderItemID
                           join addedValueTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax) on cp.ID equals addedValueTax.OrderItemID
                           join exciseTax in taxes.Where(t => t.Type == (int)Enums.CustomsRateType.ConsumeTax) on cp.ID equals exciseTax.OrderItemID into exciseTaxes
                           from exciseTax in exciseTaxes.DefaultIfEmpty()
                           join premium in premiums on cp.ID equals premium.OrderItemID into temp
                           from premium in temp.DefaultIfEmpty()
                           select new
                           {
                               ItemID = cp.ID,
                               PartNumber = cp.Model,
                               Manufacturer = cp.Manufacturer,

                               HSCode = category.HSCode,
                               TariffName = category.Name,
                               TaxCode = category.TaxCode,
                               TaxName = category.TaxName,
                               LegalUnit1 = category.Unit1,
                               LegalUnit2 = category.Unit2,
                               VATRate = addedValueTax.Rate,
                               ImportPreferentialTaxRate = importTax.Rate,
                               OriginRate = 0m,
                               ExciseTaxRate = exciseTax?.Rate ?? 0m,
                               CIQCode = category.CIQCode,
                               Elements = category.Elements,

                               Ccc = (category.Type & (int)Enums.ItemCategoryType.CCC) > 0,
                               Embargo = (category.Type & (int)Enums.ItemCategoryType.Forbid) > 0,
                               HkControl = (category.Type & (int)Enums.ItemCategoryType.HKForbid) > 0,
                               Coo = (category.Type & (int)Enums.ItemCategoryType.OriginProof) > 0,
                               CIQ = (category.Type & (int)Enums.ItemCategoryType.Inspection) > 0,
                               CIQprice = premium == null ? 0m : premium.UnitPrice,
                               IsHighPrice = (category.Type & (int)Enums.ItemCategoryType.HighValue) > 0,
                               IsDisinfected = (category.Type & (int)Enums.ItemCategoryType.Quarantine) > 0,

                               CreatorID = Icgoo.DefaultCreator,

                               CreateDate = category.CreateDate,
                               UpdateDate = category.UpdateDate
                           };

                var url = ConfigurationManager.AppSettings[this.apisetting.ApiName] + this.apisetting.SyncClassified;
                var result = Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
                {
                    results = data.ToList()
                });

                if (result.code == 300)
                {
                    throw new Exception("归类数据同步异常 - " + result.data);
                }

                return result;
            }
        }
    }

    /// <summary>
    /// 3C审批结果同步
    /// </summary>
    public class CccControl_SyncHandler : SyncHandlerBase
    {
        #region 属性

        private PvWsOrderApiSetting apisetting;

        #endregion

        #region 构造器

        internal CccControl_SyncHandler(PvWsOrderApiSetting apisetting)
        {
            this.apisetting = apisetting;
        }

        #endregion

        public override JMessage DoSync()
        {
            var cp = this.cps.FirstOrDefault();
            var url = ConfigurationManager.AppSettings[this.apisetting.ApiName] + this.apisetting.SyncCccControl;
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
            {
                itemID = cp.ID,
                isCCC = cp.IsCCC
            });

            if (result.code == 300)
            {
                throw new Exception("3C审批结果同步异常 - " + result.data);
            }

            return result;
        }
    }
}
