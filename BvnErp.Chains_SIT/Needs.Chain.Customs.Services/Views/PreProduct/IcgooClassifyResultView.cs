using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class IcgooClassifyResultView : QueryView<Models.IcgooClassifyResult, ScCustomsReponsitory>
    {
        public string Model { get; set; }
        public IcgooClassifyResultView(string model)
        {
            this.Model = model;
        }

        internal IcgooClassifyResultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected IcgooClassifyResultView(ScCustomsReponsitory reponsitory, IQueryable<Models.IcgooClassifyResult> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.IcgooClassifyResult> GetIQueryable()
        {
            var linq_res = (from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                            join orderItemCate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on orderItem.ID equals orderItemCate.OrderItemID
                            join orderItemTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on new { OrderItemID = orderItem.ID, TaxType = 0 } equals new { OrderItemID = orderItemTax.OrderItemID, TaxType = orderItemTax.Type }
                            where orderItem.ClassifyStatus == (int)ClassifyStatus.Done && orderItem.Model== Model
                            select new Models.IcgooClassifyResult
                            {
                                Model = orderItem.Model,
                                ProductName = orderItemCate.Name,
                                Manufacturer = orderItem.Manufacturer,
                                HSCode = orderItemCate.HSCode,
                                CIQCode = orderItemCate.CIQCode,
                                Elements = orderItemCate.Elements,
                                Origin = orderItem.Origin,
                                TariffRate = orderItemTax.Rate,
                                ReceiptRate = orderItemTax.ReceiptRate,
                                TaxName = orderItemCate.TaxName,
                                TaxCode = orderItemCate.TaxCode,
                                UpdateDate = orderItemCate.UpdateDate,
                                Type = (ItemCategoryType)orderItemCate.Type,
                                Unit1 = orderItemCate.Unit1,
                                Unit2 = orderItemCate.Unit2,
                                ProductUniqueCode = orderItem.ProductUniqueCode,
                                CreateDate = orderItem.CreateDate

                            }).
                           Union(
                             from preProd in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                             join preProdCate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>() on preProd.ID equals preProdCate.PreProductID
                             join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on preProd.ProductUnionCode equals orderItem.ProductUniqueCode into orderItems
                             from orderitem in orderItems.DefaultIfEmpty()
                             where preProd.CompanyType == (int)CompanyTypeEnums.Icgoo && 
                             preProdCate.ClassifyStatus == (int)ClassifyStatus.Done && 
                             orderitem.ProductUniqueCode == null &&
                             preProd.Model == Model
                             select new Models.IcgooClassifyResult
                             {
                                 Model = preProd.Model,
                                 ProductName = preProdCate.ProductName,
                                 Manufacturer = preProd.Manufacturer,
                                 HSCode = preProdCate.HSCode,
                                 CIQCode = preProdCate.CIQCode,
                                 Elements = preProdCate.Elements,
                                 Origin = "",
                                 TariffRate = preProdCate.TariffRate.Value,
                                 ReceiptRate = 0m,
                                 TaxName = preProdCate.TaxName,
                                 TaxCode = preProdCate.TaxCode,
                                 UpdateDate = preProdCate.UpdateDate,
                                 Type = (ItemCategoryType)preProdCate.Type,
                                 Unit1 = preProdCate.Unit1,
                                 Unit2 = preProdCate.Unit2,
                                 ProductUniqueCode = preProd.ProductUnionCode,
                                 CreateDate = preProd.CreateDate
                             });
          

            return linq_res;
        }

        public List<Models.IcgooClassifyResult> ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.IcgooClassifyResult> iquery = this.IQueryable.Cast<Models.IcgooClassifyResult>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var results = iquery.ToList();

            var eccn = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvDataEccn>().Where(t => t.PartNumber == this.Model).OrderByDescending(t=>t.CreateDate).FirstOrDefault();
            if (eccn != null) 
            {
                foreach(var item in results) 
                {
                    item.Eccn = eccn.Code;
                }
            }

            return results;
        }


        public IcgooClassifyResultView SearchByBrand(string brand)
        {
            var linq = from query in this.IQueryable
                       where query.Manufacturer == brand
                       select query;

            var view = new IcgooClassifyResultView(this.Reponsitory, linq);
            return view;
        }
    }
}
