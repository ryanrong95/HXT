using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class EnumsDictionariesOrigin : Yahv.Linq.UniqueView<Models.Origins.EnumsDictionary, PvdCrmReponsitory>
    {
        internal EnumsDictionariesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal EnumsDictionariesOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Origins.EnumsDictionary> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>()
                   select new Models.Origins.EnumsDictionary
                   {
                       ID = entity.ID,
                       Enum = entity.Enum,
                       IsFixed = entity.IsFixed,
                       Field = entity.Field,
                       Description = entity.Description,
                       Value = entity.Value,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
