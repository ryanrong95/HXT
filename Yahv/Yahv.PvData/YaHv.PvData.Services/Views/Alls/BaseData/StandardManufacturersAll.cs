using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 标准品牌
    /// </summary>
    public class StandardManufacturersAll : UniqueView<Models.StandardManufacturer, PvDataReponsitory>
    {
        public StandardManufacturersAll()
        {
        }

        internal StandardManufacturersAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<StandardManufacturer> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.StandardManufacturers>()
                   select new Models.StandardManufacturer
                   {
                       ID = entity.ID,
                       Name = entity.Name
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="name">品牌名称</param>
        /// <returns></returns>
        public new IQueryable<Models.StandardManufacturer> this[string name]
        {
            get
            {
                return this.Where(item => item.Name.Trim().StartsWith(name.Trim()));
            }
        }
    }
}
