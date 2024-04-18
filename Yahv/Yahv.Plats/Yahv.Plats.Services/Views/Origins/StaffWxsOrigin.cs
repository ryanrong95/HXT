using System.Linq;
using Layers.Data.Sqls;
using Yahv.Plats.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Plats.Services.Views.Origins
{
    /// <summary>
    /// 员工微信视图
    /// </summary>
    public class StaffWxsOrigin : UniqueView<StaffWxs, PvbErmReponsitory>
    {
        public StaffWxsOrigin()
        {

        }

        protected override IQueryable<StaffWxs> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.StaffWxs>()
                   select new StaffWxs()
                   {
                       ID = entity.ID,
                       UnionID = entity.UnionID,
                       OpenID = entity.OpenID,
                       City = entity.City,
                       Country = entity.Country,
                       HeadImgurl = entity.HeadImgurl,
                       Nickname = entity.Nickname,
                       Province = entity.Province,
                       Sex = entity.Sex
                   };
        }
    }
}