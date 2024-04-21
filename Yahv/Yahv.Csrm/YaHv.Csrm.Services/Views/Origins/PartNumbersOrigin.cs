using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class PartNumbersOrigin : Yahv.Linq.UniqueView<PartNumber, PvbCrmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PartNumbersOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal PartNumbersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<PartNumber> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PartNumbers>()
                   select new PartNumber
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Manufacturer = entity.Manufacturer
                   };
        }
    }
}
