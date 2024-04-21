using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;

namespace YaHv.CrmPlus.Services.Views.Rolls
{
    /// <summary>
    /// 供应商信用
    /// </summary>
    public class SupplierCreditsRoll : CreditsOrgin
    {
        public SupplierCreditsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SupplierCreditsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Credit> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Type == CreditType.CreditReceiver);
        }

    }
    /// <summary>
    /// 客户信用
    /// </summary>

    public class ClientCreditsRoll : CreditsOrgin
    {
        public ClientCreditsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientCreditsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Credit> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Type == CreditType.GrantingParty);
        }

    }
}
