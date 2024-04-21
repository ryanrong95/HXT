
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 标准品牌
    /// </summary>
    public class BrandsOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.StandardBrand, PvdCrmReponsitory>
    {
        internal BrandsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal BrandsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected BrandsOrigin(PvdCrmReponsitory reponsitory, IQueryable<Yahv.CrmPlus.Service.Models.Origins.StandardBrand> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<Yahv.CrmPlus.Service.Models.Origins.StandardBrand> GetIQueryable()
        {
            var adminTopview = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>()
                   join admin in adminTopview on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Yahv.CrmPlus.Service.Models.Origins.StandardBrand
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       ChineseName = entity.ChineseName,
                       IsAgent = entity.IsAgent,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       Status = (DataStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                       Admin = admin
                   };
        }
    }



    //public class StandardBrandOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.StandardBrand, PvdCrmReponsitory>
    //{
    //    internal StandardBrandOrigin()
    //    {

    //    }

    //    internal StandardBrandOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
    //    {
    //    }

    //    protected override IQueryable<Yahv.CrmPlus.Service.Models.Origins.StandardBrand> GetIQueryable()
    //    {
    //        var companynBrandView = new CompanynBrandView();
    //        var suppliernBrandView = new SuppliernBrandView();
    //        var vbrandView = new vBrandOrigin();
    //        return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>()
    //               join company in companynBrandView on entity.ID equals company.BrandID into _companys
    //               from company in _companys.DefaultIfEmpty()
    //               join supplier in suppliernBrandView on entity.ID equals supplier.BrandID into _suppliers
    //              // join pms in vbrandView on  new { entity.ID,  } equals new { pms.BrandID, pms.RoleID=FixedRole.PM.GetFixedID().} 
    //               select new Yahv.CrmPlus.Service.Models.Origins.StandardBrand
    //               {
    //                   ID = entity.ID,
    //                   Name = entity.Name,
    //                   Code = entity.Code,
    //                   ChineseName = entity.ChineseName,
    //                   IsAgent = entity.IsAgent,
    //                   CreateDate = entity.CreateDate,
    //                   ModifyDate = entity.ModifyDate,
    //                   Summary = entity.Summary,
    //                   Status = (DataStatus)entity.Status,
    //                   CreatorID = entity.CreatorID,
                    

    //               };

    //    }
    //}

}
