using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class OwnedDriversRoll : Origins.DriversOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 构造函数
        /// </summary>
        public OwnedDriversRoll(Enterprise Enterprise)
        {
            this.enterprise = Enterprise;
        }
        protected override IQueryable<Driver> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Enterprise.ID == this.enterprise.ID);
        }
    }
    public class DriversRoll : Origins.DriversOrigin
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DriversRoll(Enterprise Enterprise)
        {

        }
        protected override IQueryable<Driver> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
