using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 员工微信视图
    /// </summary>
    public class StaffWxsAll : UniqueView<StaffWxs, PvbErmReponsitory>
    {
        public StaffWxsAll()
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
