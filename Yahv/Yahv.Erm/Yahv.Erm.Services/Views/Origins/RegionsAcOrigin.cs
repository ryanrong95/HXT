using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 区域视图
    /// </summary>
    public class RegionsAcOrigin : UniqueView<RegionAc, PvbErmReponsitory>
    {
        public RegionsAcOrigin()
        {

        }

        public RegionsAcOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<RegionAc> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.RegionsAc>()
                   select new RegionAc()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       ModifyDate = entity.ModifyDate,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ModifyID = entity.ModifyID,
                       FatherID = entity.FatherID,
                       Status = (GeneralStatus)entity.Status,
                   };
        }
    }
}