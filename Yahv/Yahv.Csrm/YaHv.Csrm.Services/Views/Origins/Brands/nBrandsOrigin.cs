using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 司机
    /// </summary>
    public class nBrandsOrigin : Yahv.Linq.UniqueView<nBrand, PvbCrmReponsitory>
    {
        internal nBrandsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nBrandsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nBrand> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nBrands>()

                   join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.BrandSynonymTopView>() on entity.BrandID equals brand.ID

                   join enterpise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enterpise.ID
                   where entity.Status == (int)DataStatus.Normal
                   select new nBrand
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       BrandName = brand.Name,
                       ChineseName = brand.ChineseName,
                       ShortName = brand.ShortName,
                       EnterpriseID = enterpise.ID,
                       EnterpriseName = enterpise.Name,
                       Status = (DataStatus)entity.Status,
                   };
        }
    }


    /// <summary>
    /// 司机
    /// </summary>
    public class nBrandsOrigin_Chenhan : Yahv.Linq.UniqueView<nBrand_Chenhan, PvbCrmReponsitory>
    {
        internal nBrandsOrigin_Chenhan()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nBrandsOrigin_Chenhan(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nBrands>()
                   join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.BrandSynonymTopView>() on entity.BrandID equals brand.ID
                   where entity.Status == (int)DataStatus.Normal
                   select new nBrand_Chenhan
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       BrandName = brand.Name,
                       ChineseName = brand.ChineseName,
                       ShortName = brand.ShortName,
                       EnterpriseID = entity.EnterpriseID,
                       IsAgent = brand.IsAgent,
                       Status = (DataStatus)entity.Status,
                   };
        }
    }

}
