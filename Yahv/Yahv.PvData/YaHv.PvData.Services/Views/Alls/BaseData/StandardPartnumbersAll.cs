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
    /// 标准型号
    /// </summary>
    public class StandardPartnumbersAll : UniqueView<Models.StandardPartnumber, PvDataReponsitory>
    {
        public StandardPartnumbersAll()
        {
        }

        internal StandardPartnumbersAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<StandardPartnumber> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.StandardPartnumbers>()
                   select new Models.StandardPartnumber
                   {
                       ID = entity.ID,
                       Name = entity.Name
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="name">型号名称</param>
        /// <returns></returns>
        public new IQueryable<Models.StandardPartnumber> this[string name]
        {
            get
            {
                return this.Where(item => item.Name.Trim().StartsWith(name.Trim()));
            }
        }
    }
}
