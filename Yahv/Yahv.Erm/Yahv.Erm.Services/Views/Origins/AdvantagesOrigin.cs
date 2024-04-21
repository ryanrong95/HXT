using Layers.Data.Sqls;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services.Views.Origins
{
    public class AdvantagesOrigin : Yahv.Linq.QueryView<Advantage, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal AdvantagesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal AdvantagesOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Advantage> GetIQueryable()
        {
            var adminView = new AdminsOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Advantages>()
                   join admin in adminView on entity.AdminID equals admin.ID
                   select new Advantage
                   {
                       AdminID = admin.ID,
                       Manufacturers = entity.Manufacturers,
                       PartNumbers = entity.PartNumbers,
                       Admin = admin
                   };
        }
    }
}
