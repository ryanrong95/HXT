using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Underly;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ProductItemAlls : UniqueView<ProductItem, BvCrmReponsitory>, Needs.Underly.IFkoView<ProductItem>
    {
        public ProductItemAlls()
        {

        }

        internal ProductItemAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }


        internal ProductItemAlls(BvCrmReponsitory reponsitory, IQueryable<ProductItem> iQuery) : base(reponsitory, iQuery)
        {

        }

        protected override IQueryable<ProductItem> GetIQueryable()
        {
            var sampleView = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemSamples>()
                             select new Sample
                             {
                                 ID = entity.ID,
                                 Type = (SampleType)entity.Type,
                                 UnitPrice = entity.UnitPrice,
                                 Quantity = entity.Quantity,
                                 TotalPrice = entity.TotalPrice,
                                 Date = entity.Date,
                                 Contactor = entity.Contactor,
                                 Phone = entity.Phone,
                                 Address = entity.Address,
                                 CreateDate = entity.CreateDate,
                                 UpdateDate = entity.UpdateDate,
                             };            

            var mapView = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry>()
                          join entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemEnquiries>() 
                          on map.ProductItemEnquiryID equals entity.ID                          
                          select new
                          {
                              map.ProductItemID,
                              Enquiry = new Enquiry
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
                                  ReportDate = entity.ReportDate,
                                  Summary = entity.Summary
                              }
                          };

            var mapgroupView = from map in mapView
                               group map by map.ProductItemID into maps
                               select new
                               {
                                   maps.Key,
                                   Enquiries = from item in maps
                                               select item.Enquiry
                               };

            var filegroupView = from file in new ProductItemFileAlls(this.Reponsitory).Where(item => item.Status == Status.Normal)
                                group file by file.ProductItemID into files
                                select new
                                {
                                    files.Key,
                                    Files = from item in files
                                            select item
                                };

            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                        join _sample in sampleView on entity.ID equals _sample.ID into samples
                        from sample in samples.DefaultIfEmpty()
                        join _map in mapgroupView on entity.ID equals _map.Key into maps
                        from map in maps.DefaultIfEmpty()
                        join _filegroup in filegroupView
                        on entity.ID equals _filegroup.Key into filegroups
                        from filegroup in filegroups.DefaultIfEmpty()
                        select new ProductItem
                        {
                            ID = entity.ID,
                            StandardID = entity.StandardID,
                            CompeteID = entity.CompeteID,
                            RefUnitQuantity = entity.RefUnitQuantity,
                            RefQuantity = entity.RefQuantity,
                            RefUnitPrice = entity.RefUnitPrice,
                            ExpectRate = entity.ExpectRate,
                            ExpectQuantity = entity.ExpectQuantity,
                            ExpectDate = entity.ExpectDate,
                            Status = (ProductStatus)entity.Status,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                            PMAdminID = entity.PMAdmin,
                            FAEAdminID = entity.FAEAdmin,
                            SaleAdminID = entity.SaleAdmin,
                            PurchaseAdminID = entity.PurChaseAdmin,
                            AssistantAdiminID = entity.AssistantAdmin,
                            Summary = entity.Summary,

                            Sample = sample,
                            Enquiries = map.Enquiries,
                            Files = filegroup.Files.ToArray(),
                        };
            return linqs;     
        }

        /// <summary>
        /// 根据projectID查询当前数据
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public ProductItemAlls SearchByProjectID(string ProjectID)
        {
            var standardproduct = new StandardProductAlls(this.Reponsitory);
            CompeteProductAlls competeProducts = new CompeteProductAlls(this.Reponsitory);
            var applies = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>().Where(item => item.Status == (int)ApplyStatus.Audting);

            var iQuery = from product in GetProductItems(ProjectID)
                         join standard in standardproduct on product.StandardID equals standard.ID
                         join apply in applies on product.ID equals apply.MainID into _applies
                         join competeproduct in competeProducts on product.CompeteID equals competeproduct.ID into products
                         from _competeproduct in products.DefaultIfEmpty()
                         select new ProductItem
                         {
                             ID = product.ID,
                             StandardID = product.StandardID,
                             standardProduct = standard,
                             CompeteID = product.CompeteID,
                             CompeteProduct = _competeproduct,
                             RefUnitQuantity = product.RefUnitQuantity,
                             RefQuantity = product.RefQuantity,
                             RefUnitPrice = product.RefUnitPrice,
                             ExpectRate = product.ExpectRate,
                             ExpectDate = product.ExpectDate,
                             ExpectQuantity = product.ExpectQuantity,                             
                             ReportDate = product.ReportDate,
                             IsReport = product.IsReport,
                             Status = product.Status,
                             CreateDate = product.CreateDate,
                             UpdateDate = product.UpdateDate,
                             PMAdminID = product.PMAdminID,
                             FAEAdminID = product.FAEAdminID,
                             SaleAdminID = product.SaleAdminID,
                             PurchaseAdminID = product.PurchaseAdminID,
                             AssistantAdiminID = product.AssistantAdiminID,
                             Sample = product.Sample,
                             Enquiries = product.Enquiries,
                             Summary = product.Summary,
                             Files = product.Files,
                             IsApr = _applies.Count() > 0,
                         };

            return new ProductItemAlls(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据项目ID获取items数据
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        internal IQueryable<ProductItem> GetProductItems(string ProjectID)
        {
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                   join item in this.IQueryable on map.ProductItemID equals item.ID
                   where map.ProjectID == ProjectID
                   select item;
        }

        /// <summary>
        /// 根据ID查询出关联数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductItem SearchByID(string ID)
        {
            var product = this[ID];
            if (product != null)
            {
                var standardproduct = new StandardProductAlls(this.Reponsitory)[product.StandardID];
                var competeproduct = new CompeteProductAlls(this.Reponsitory)[product.CompeteID];
                var files = new ProductItemFileAlls(this.Reponsitory).Where(item => item.ProductItemID == ID).ToArray();

                product.standardProduct = standardproduct;
                product.CompeteProduct = competeproduct;
                product.Files = files;                
            }
            return product;
        }
    }
}
