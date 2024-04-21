
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvdCrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class MapsTopNOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.MapsTopN, PvdCrmReponsitory>
    {

        internal MapsTopNOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal MapsTopNOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Yahv.CrmPlus.Service.Models.Origins.MapsTopN> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsTopN>()
                   select new Yahv.CrmPlus.Service.Models.Origins.MapsTopN
                   {
                       ID = entity.ID,
                       TopOrder = entity.TopOrder,
                       ClientID = entity.ClientID,
                       OwnerID = entity.OwnerID,
                   };
        }
    }
}
