using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;

namespace YaHv.Erm.Services.Views.Rolls
{
    public class AdvantageRoll : AdvantagesOrigin
    {
        Admin admin;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public AdvantageRoll(Admin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<Advantage> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.AdminID == this.admin.ID
                   select item;
            // return new AdvantagesOrigin().Where(item => item.AdminID == this.admin.ID);
        }


    }
}
