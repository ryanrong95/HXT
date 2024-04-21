using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 标准品牌
    /// </summary
    [Obsolete]
    public class BrandsOrigin : Yahv.Linq.UniqueView<Models.Origins.Brand, PvbCrmReponsitory>
    {
        internal BrandsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal BrandsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected BrandsOrigin(PvbCrmReponsitory reponsitory, IQueryable<Models.Origins.Brand> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<Models.Origins.Brand> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>()
                   select new Models.Origins.Brand
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       ShortName = entity.ShortName,
                       CreateDate = entity.CreateDate,
                       Status = (GeneralStatus)entity.Status
                   };
        }
    }
    
}
