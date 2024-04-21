using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class EnterpriseRegistersOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.EnterpriseRegister, PvdCrmReponsitory>
    {
        internal EnterpriseRegistersOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal EnterpriseRegistersOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<EnterpriseRegister> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>()
                   select new EnterpriseRegister
                   {
                       ID = entity.ID,
                       IsSecret = entity.IsSecret,
                       IsInternational = entity.IsInternational,
                       Corperation = entity.Corperation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Currency = (Currency)entity.Currency,
                       RegistFund = entity.RegistFund,
                       RegistCurrency = (Currency)entity.RegistCurrency,
                       Industry = entity.Industry,
                       RegistDate = entity.RegistDate,
                       Summary = entity.Summary,
                       BusinessState = entity.BusinessState,
                       Employees = entity.Employees,
                       WebSite = entity.WebSite,
                       Nature = entity.Nature,
                   };
        }
    }
}
