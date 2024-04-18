using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PSLToSolrData.Import.Models;

namespace Yahv.PSLToSolrData.Import.Views
{
    /// <summary>
    /// 标准产品视图
    /// </summary>
    public class ProductsView : UniqueView<Models.Product, Linqs.PSLReponsitory>
    {
        internal ProductsView()
        {
        }

        internal ProductsView(Linqs.PSLReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Product> GetIQueryable()
        {
            var piView = new ProductInventoriesView(Reponsitory);

            return from p in Reponsitory.ReadTable<Linqs.Products>()
                   join mc in Reponsitory.ReadTable<Linqs.MapsCategory>() on p.ID equals mc.ProductID
                   join pc in Reponsitory.ReadTable<Linqs.Categories>() on mc.CategoryID equals pc.ID
                   join po in Reponsitory.ReadTable<Linqs.ProductOthers>() on p.ID equals po.ProductID into pos
                   join pf in Reponsitory.ReadTable<Linqs.ProductFiles>() on p.ID equals pf.ProductID into pfs
                   join pi in piView on p.ID equals pi.ProductID into pis
                   select new Models.Product()
                   {
                       ID = p.ID,
                       PartNumber = p.PartNumber,
                       Manufacturer = p.Manufacturer,
                       PackageCase = p.PackageCase,
                       CreateDate = p.CreateDate,

                       //产品分类
                       ProductCategory = new Models.ProductCategory()
                       {
                           ID = pc.ID,
                           Type = pc.Type,
                           NameCN = pc.NameCN,
                           NameEN = pc.NameEN,
                           CreateDate = pc.CreateDate,
                           ModifyDate = pc.ModifyDate
                       },

                       //产品其他信息扩展
                       ProductOthers = pos.Select(item => new Models.ProductOther()
                       {
                           ID = item.ID,
                           ProductID = item.ProductID,
                           Description = item.Description,
                           Language = item.Language,
                           Source = item.Source

                       }).ToArray(),

                       //产品文件
                       ProductFiles = pfs.Select(item => new Models.ProductFile()
                       {
                           ID = item.ID,
                           ProductID = item.ProductID,
                           Type = item.Type,
                           Name = item.Name,
                           Uri = item.Uri,
                           CreateDate = item.CreateDate,
                           ModifyDate = item.ModifyDate
                       }).ToArray(),

                       //产品库存
                       ProductInventories = pis.ToArray()
                   };
        }
    }
}
