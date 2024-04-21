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
    public class ConductsOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Conduct, PvdCrmReponsitory>
    {
        internal ConductsOrigin()
        {

        }
        internal ConductsOrigin(Enterprise enterprise)
        {


        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ConductsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Conduct> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Conducts>()
                   select new Conduct
                   {
                       ID = entity.ID,
                       EnterpriseID=entity.EnterpriseID,
                       ConductType=(ConductType)entity.Type,
                       Grade=(ConductGrade)entity.Grade,
                       IsPublic=entity.IsPublic,
                       
                   };
        }
    }
}
