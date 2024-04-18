using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// 预归类产品扩展类
    /// </summary>
    public static class PreProductExtends
    {
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void BatchInsert(this ClientModels.Client.PreProduct[] preProducts, ScCustomReponsitory reponsitory)
        {
            reponsitory.Insert(preProducts.Select(item => new Layers.Data.Sqls.ScCustoms.PreProducts
            {
                ID = item.ID = Guid.NewGuid().ToString("N").ToUpper(),
                ClientID = item.ClientID,
                ProductUnionCode = item.ProductUnionCode,
                Model = item.Model,
                Manufacturer = item.Manufacturer,
                Qty = item.Qty,
                Price = item.Price,
                Currency = item.Currency,
                Supplier = item.Supplier,
                CompanyType = (int)item.CompanyType,
                BatchNo = item.BatchNo,
                Description = item.Description,
                Pack = item.Pack,
                AreaOfProduction = item.AreaOfProduction,
                UseFor = item.UseFor,
                Status = (int)item.Status,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                Summary = item.Summary,
                DueDate = item.DueDate,
            }).ToArray());
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void Update(this ClientModels.Client.PreProduct preProduct, ScCustomReponsitory reponsitory)
        {
            reponsitory.Update<Layers.Data.Sqls.ScCustoms.PreProducts>(new
            {
                ClientID = preProduct.ClientID,
                ProductUnionCode = preProduct.ProductUnionCode,
                Model = preProduct.Model,
                Manufacturer = preProduct.Manufacturer,
                Qty = preProduct.Qty,
                Price = preProduct.Price,
                Currency = preProduct.Currency,
                Supplier = preProduct.Supplier,
                CompanyType = (int)preProduct.CompanyType,
                BatchNo = preProduct.BatchNo,
                Description = preProduct.Description,
                Pack = preProduct.Pack,
                AreaOfProduction = preProduct.AreaOfProduction,
                UseFor = preProduct.UseFor,
                Status = (int)preProduct.Status,
                UpdateDate = DateTime.Now,
                Summary = preProduct.Summary,
                DueDate = preProduct.DueDate,
            }, item => item.ID == preProduct.ID);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void Insert(this ClientModels.Client.PreProduct preProduct, ScCustomReponsitory reponsitory)
        {
            reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.PreProducts
            {
                ID = preProduct.ID = Guid.NewGuid().ToString("N").ToUpper(),
                ClientID = preProduct.ClientID,
                ProductUnionCode = preProduct.ProductUnionCode,
                Model = preProduct.Model,
                Manufacturer = preProduct.Manufacturer,
                Qty = preProduct.Qty,
                Price = preProduct.Price,
                Currency = preProduct.Currency,
                Supplier = preProduct.Supplier,
                CompanyType = (int)preProduct.CompanyType,
                BatchNo = preProduct.BatchNo,
                Description = preProduct.Description,
                Pack = preProduct.Pack,
                AreaOfProduction = preProduct.AreaOfProduction,
                UseFor = preProduct.UseFor,
                Status = (int)preProduct.Status,
                CreateDate = preProduct.CreateDate,
                UpdateDate = preProduct.UpdateDate,
                Summary = preProduct.Summary,
                DueDate = preProduct.DueDate,
                UseType=(int)preProduct.UseType,
            });
        }
    }

    /// <summary>
    /// 预归类信息对象扩展
    /// </summary>
    public static class PreProductCategories
    {
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void InsertCategories(this ClientModels.Client.PreProduct preProduct, ScCustomReponsitory reponsitory)
        {
            reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.PreProductCategories
            {
                ID = preProduct.PreProductCategoryID = Guid.NewGuid().ToString("N").ToUpper(),
                Type = (int)ItemCategoryType.Normal,
                PreProductID = preProduct.ID,
                ProductName=preProduct.ProductName,
                Model = preProduct.Model,
                Manufacture = preProduct.Manufacturer,
                ClassifyStatus = (int)ClassifyStatus.Unclassified,
                Status = (int)GeneralStatus.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = preProduct.Summary,
            });
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void UpdateCategories(this ClientModels.Client.PreProduct preProduct, ScCustomReponsitory reponsitory)
        {
            reponsitory.Update<Layers.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                PreProductID = preProduct.ID,
                ProductName=preProduct.ProductName,
                Model = preProduct.Model,
                Manufacture = preProduct.Manufacturer,
                ClassifyStatus = (int)ClassifyStatus.Unclassified,
                UpdateDate = DateTime.Now,
                Summary = preProduct.Summary,
            }, item => item.ID == preProduct.ID);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="preProducts"></param>
        public static void BatchInsertCategories(this ClientModels.Client.PreProduct[] preProducts, ScCustomReponsitory reponsitory)
        {
            reponsitory.Insert(preProducts.Select(item => new Layers.Data.Sqls.ScCustoms.PreProductCategories
            {
                ID = item.PreProductCategoryID = Guid.NewGuid().ToString("N").ToUpper(),
                Type = (int)ItemCategoryType.Normal,
                PreProductID = item.ID,
                ProductName=item.ProductName,
                Model = item.Model,
                Manufacture = item.Manufacturer,
                ClassifyStatus = (int)ClassifyStatus.Unclassified,
                Status = (int)GeneralStatus.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = item.Summary,
            }).ToArray());
        }
    }
}
