using Layers.Data.Sqls;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views
{
    public class PastsWageItemAlls : UniqueView<PastsWageItem, PvbErmReponsitory>
    {
        public PastsWageItemAlls()
        {

        }

        /// <summary>
        /// 结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PastsWageItem> GetIQueryable()
        {
            var PastsWageItems = new PastsWageItemOrigin(this.Reponsitory);
            var admins = new AdminsOrigin(this.Reponsitory);
            var wageitems = new WageItemsOrigin(this.Reponsitory);

            return from pastswageitem in PastsWageItems
                   join admin in admins on pastswageitem.AdminID equals admin.ID
                   join wage in wageitems on pastswageitem.WageItemID equals wage.ID
                   select new PastsWageItem
                   {
                       ID = pastswageitem.ID,
                       StaffID = pastswageitem.StaffID,
                       WageItemID = pastswageitem.WageItemID,
                       WageItemName = wage.Name,
                       DefaultValue = pastswageitem.DefaultValue,
                       CreateDate = pastswageitem.CreateDate,
                       AdminID = pastswageitem.AdminID,
                       AdminName = admin.RealName,
                   };
        }
    }
}
