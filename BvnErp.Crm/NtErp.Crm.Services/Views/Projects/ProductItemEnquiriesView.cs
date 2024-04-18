using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views.Projects
{
    /// <summary>
    /// 销售机会型号询价视图
    /// </summary>
    public class ProductItemEnquiriesView : UniqueView<Enquiry, BvCrmReponsitory>, Needs.Underly.IFkoView<Enquiry>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ProductItemEnquiriesView()
        {

        }

        string productItemID;
        /// <summary>
        /// 产品型号下的询价信息
        /// </summary>
        /// <param name="productItemID"></param>
        public ProductItemEnquiriesView(string productItemID)
        {
            this.productItemID = productItemID;
        }

        internal ProductItemEnquiriesView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 基础数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Enquiry> GetIQueryable()
        {
            if (string.IsNullOrEmpty(this.productItemID))
            {
                // 所有询价集合
                return this.Queryable();
            }
            else
            {
                // 产品型号的询价集合
                return this.GetMapQueryable().Where(item => item.ProductItemID == this.productItemID);
            }
        }

        /// <summary>
        /// 获取产品参考价查询集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<EnquiryReference> GetReferences()
        {
            var productsView = new StandardProductAlls(this.Reponsitory);

            var linqs = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry>()
                        join enquiry in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemEnquiries>() on map.ProductItemEnquiryID equals enquiry.ID
                        join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on map.ProductItemID equals item.ID
                        join product in productsView on item.StandardID equals product.ID
                        select new EnquiryReference
                        {
                            Name = product.Name,
                            Manufacturer = product.Manufacturer.Name,
                            OriginModel = enquiry.OriginModel,
                            MOQ = enquiry.MOQ,
                            MPQ = enquiry.MPQ,
                            Validity = enquiry.Validity,
                            ValidityCount = enquiry.ValidityCount,
                            Currency = (Enums.CurrencyType)enquiry.Currency,
                            SalePrice = enquiry.SalePrice,
                            Summary = enquiry.Summary,
                            UpdateDate = enquiry.UpdateDate
                        };

            return linqs;
        }

        /// <summary>
        /// 获取带ProductItemID关系的视图
        /// </summary>
        /// <returns></returns>
        internal IQueryable<Enquiry> GetMapQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry>()
                   join entity in this.Queryable() on map.ProductItemEnquiryID equals entity.ID
                   select new Models.Enquiry()
                   {
                       ProductItemID = map.ProductItemID,
                       ID = entity.ID,
                       ReplyPrice = entity.ReplyPrice,
                       ReplyDate = entity.ReplyDate,
                       RFQ = entity.RFQ,
                       OriginModel = entity.OriginModel,
                       MOQ = entity.MOQ,
                       MPQ = entity.MPQ,
                       Currency = (Enums.CurrencyType)entity.Currency,
                       ExchangeRate = entity.ExchangeRate,
                       TaxRate = entity.TaxRate,
                       Tariff = entity.Tariff,
                       OtherRate = entity.OtherRate,
                       Cost = entity.Cost,
                       Validity = entity.Validity,
                       ValidityCount = entity.ValidityCount,
                       SalePrice = entity.SalePrice,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       ReportDate = entity.ReportDate,

                       Voucher = entity.Voucher
                   };
        }

        /// <summary>
        /// 基础视图
        /// </summary>
        /// <returns></returns>
        private IQueryable<Enquiry> Queryable()
        {
            var filesView = new ProductItemFileAlls(this.Reponsitory).Where(item => item.Status == Enums.Status.Normal);

            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemEnquiries>()
                        join _file in filesView on entity.ID equals _file.SubID into files
                        from file in files.DefaultIfEmpty()

                        select new Enquiry
                        {
                            ID = entity.ID,
                            ReplyPrice = entity.ReplyPrice,
                            ReplyDate = entity.ReplyDate,
                            RFQ = entity.RFQ,
                            OriginModel = entity.OriginModel,
                            MOQ = entity.MOQ,
                            MPQ = entity.MPQ,
                            Currency = (Enums.CurrencyType)entity.Currency,
                            ExchangeRate = entity.ExchangeRate,
                            TaxRate = entity.TaxRate,
                            Tariff = entity.Tariff,
                            OtherRate = entity.OtherRate,
                            Cost = entity.Cost,
                            Validity = entity.Validity,
                            ValidityCount = entity.ValidityCount,
                            SalePrice = entity.SalePrice,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                            Summary = entity.Summary,
                            ReportDate = entity.ReportDate,

                            Voucher = file
                        };

            return linqs;
        }
    }
}
