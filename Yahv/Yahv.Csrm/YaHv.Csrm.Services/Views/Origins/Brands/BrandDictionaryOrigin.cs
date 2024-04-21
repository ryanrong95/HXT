using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class BrandDictionaryOrigin : Yahv.Linq.UniqueView<BrandDictionary, PvbCrmReponsitory>
    {
        internal BrandDictionaryOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal BrandDictionaryOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<BrandDictionary> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>()
                   select new BrandDictionary
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       OtherName = entity.OtherName,
                       Source = entity.Source,
                       CreateDate = entity.CreateDate,
                       IsShort = entity.IsShort
                   };
        }
    }
}
