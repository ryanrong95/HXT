using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class AdvantageRoll : Origins.AdvantagesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public AdvantageRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Advantage> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.Enterprise.ID == this.enterprise.ID
                   select item;
        }
    }
}
