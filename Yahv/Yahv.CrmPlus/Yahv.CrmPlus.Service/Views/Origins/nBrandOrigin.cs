using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class nBrandOrigin : Yahv.Linq.UniqueView<nBrand, PvdCrmReponsitory>
    {
        public nBrandOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        public nBrandOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nBrand> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var standardBrandView = new Origins.BrandsOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.nBrands>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join brand in standardBrandView on entity.BrandID equals brand.ID
                   select new nBrand
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       EnterpriseName = enterprise.Name,
                       Type = (Underly.nBrandType)entity.Type,
                       BrandID = entity.BrandID,
                       BrandName = brand.Name,
                       CreatorID = entity.CreatorID,
                       Summary = entity.Summary
                   };
        }

    }
}
