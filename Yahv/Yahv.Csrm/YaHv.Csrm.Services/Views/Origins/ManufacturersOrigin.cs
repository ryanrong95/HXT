using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class ManufacturersOrigin : Yahv.Linq.UniqueView<Models.Origins.Manufacturer, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        internal ManufacturersOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ManufacturersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Manufacturer> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Manufacturers>()
                   select new Manufacturer
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Agent = entity.Agent
                   };
        }
    }
}
