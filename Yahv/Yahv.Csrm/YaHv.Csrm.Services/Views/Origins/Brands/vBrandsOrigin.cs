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
    public class vBrandsOrigin : Yahv.Linq.UniqueView<vBrand, PvbCrmReponsitory>
    {
        internal vBrandsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal vBrandsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<vBrand> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.vBrands>()
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new vBrand
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       AdminID = entity.AdminID,
                       AdminRealName = admin.RealName,
                       AdminRoleName = admin.RoleName
                   };
        }
    }
}
