using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class nFixedBrandsOrigin : Yahv.Linq.UniqueView<nFixedBrand, PvdCrmReponsitory>
    {
        internal nFixedBrandsOrigin()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nFixedBrandsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nFixedBrand> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.nFixedBrands>()
                   select new nFixedBrand
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Brand = entity.Brand,
                       IsProhibited = entity.IsProhibited,
                       IsPromoted = entity.IsPromoted,
                       IsDiscounted = entity.IsDiscounted,
                       IsAdvantaged = entity.IsAdvantaged,
                       IsSpecial = entity.IsSpecial,
                       Summary = entity.Summary,
                       CreatorID = entity.CreatorID,
                   };
        }
    }
}
