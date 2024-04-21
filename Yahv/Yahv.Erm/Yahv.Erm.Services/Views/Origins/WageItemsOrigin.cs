using Layers.Data.Sqls;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    internal class WageItemsOrigin : UniqueView<WageItem, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal WageItemsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal WageItemsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WageItem> GetIQueryable()
        {
            return from wageitem in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.WageItems>()
                   select new WageItem
                   {
                       ID = wageitem.ID,
                       Name = wageitem.Name,
                       AdminID = wageitem.AdminID,
                       OrderIndex = wageitem.OrderIndex,
                       CreateDate = wageitem.CreateDate,
                       Status = (Status)wageitem.Status,
                       Type = (WageItemType)wageitem.Type,
                       CalcOrder = wageitem.CalcOrder,
                       Formula = wageitem.Formula,
                       Description = wageitem.Description,
                       IsImport = wageitem.IsImport,
                       InputerId = wageitem.InputerId,
                   };
        }
    }
}
