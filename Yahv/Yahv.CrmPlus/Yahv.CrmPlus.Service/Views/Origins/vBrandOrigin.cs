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
    public class vBrandOrigin : Yahv.Linq.UniqueView<vBrand, PvdCrmReponsitory>
    {
        public vBrandOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        public vBrandOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<vBrand> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.vBrands>()
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new vBrand
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       RoleID = entity.RoleID,
                       RoleName = admin.RoleName,
                       AdminID = entity.AdminID,
                       RealName = admin.RealName,
                       UserName=admin.UserName
                       
                   };
        }

    }
}
