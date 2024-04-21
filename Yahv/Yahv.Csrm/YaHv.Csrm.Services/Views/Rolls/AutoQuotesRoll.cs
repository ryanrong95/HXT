using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class AutoQuotesRoll : Origins.AutoQuotesOrign
    {
        Models.Origins.Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public AutoQuotesRoll(Models.Origins.Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<AutoQuote> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.SupplierID == this.enterprise.ID
                   select item;
        }
    }
}
