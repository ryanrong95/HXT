using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 中心库产品通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    /// <remarks>
    /// 逻辑层
    /// </remarks>
    public class ProductsTopView<TReponsitory> : UniqueView<CenterProduct, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ProductsTopView()
        {

        }
        public ProductsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CenterProduct> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductsTopView>()
                   select new CenterProduct
                   {
                       ID = item.ID,
                       PartNumber = item.PartNumber,
                       Manufacturer = item.Manufacturer,
                       PackageCase = item.PackageCase,
                       Packaging = item.Packaging,
                       CreateDate = item.CreateDate,
                   };
        }

        static public void Enter(CenterProduct product)
        {
            //调用接口 
            //ApiHelper.Current.Post("", product);

            //直接完成 reponsitory
            using (Layers.Data.Sqls.PvDataReponsitory reponsitory = new Layers.Data.Sqls.PvDataReponsitory())
            {
                product.ID = string.Concat(product.PartNumber, product.Manufacturer, product.PackageCase, product.Packaging).MD5();
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(item => item.ID == product.ID);
                if(!exist)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
                    {
                        ID = product.ID,
                        PartNumber = product.PartNumber,
                        Manufacturer = product.Manufacturer,
                        PackageCase = product.PackageCase,
                        Packaging = product.Packaging,
                        CreateDate = DateTime.Now,
                    });
                }
            }
        }
    }
}
