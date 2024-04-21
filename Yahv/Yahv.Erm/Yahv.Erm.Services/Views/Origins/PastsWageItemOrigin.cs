using Layers.Data.Sqls;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    internal class PastsWageItemOrigin : UniqueView<PastsWageItem, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private PastsWageItemOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PastsWageItemOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PastsWageItem> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Pasts_MapsWageItem>()
                   select new PastsWageItem
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       WageItemID = entity.WageItemID,
                       DefaultValue = entity.DefaultValue,
                       CreateDate = entity.CreateDate,
                       AdminID = entity.AdminID,
                   };
        }
    }
}
