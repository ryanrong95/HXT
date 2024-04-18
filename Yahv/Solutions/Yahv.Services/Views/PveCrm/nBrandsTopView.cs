using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 供应商代理品牌通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class nBrandsTopView<TReponsitory> : UniqueView<nBrand, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public nBrandsTopView()
        {

        }

        public nBrandsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<nBrand> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.nBrandsTopView>()
                   select new nBrand
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       BrandName = entity.BrandName,
                       SupplierID = entity.SupplierID,
                       SupplierName = entity.SupplierName
                   };
        }
    }
}
