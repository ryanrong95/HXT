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



    public class EnterprisesOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Enterprise, PvdCrmReponsitory>
    {
        internal EnterprisesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal EnterprisesOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Enterprise> GetIQueryable()
        {
            //var enusmdic = new EnumsDictionariesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   join register in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>()
                   on entity.ID equals register.ID
                   select new Enterprise
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       IsDraft = entity.IsDraft,
                       Status = (AuditStatus)entity.Status,
                       District = entity.District,
                       Place = entity.Place,
                       Grade = entity.Grade,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       DyjCode = entity.DyjCode,
                       EnterpriseRegister = new EnterpriseRegister
                       {
                           ID = register.ID,
                           IsSecret = register.IsSecret,
                           IsInternational = register.IsInternational,
                           Corperation = register.Corperation,
                           RegAddress = register.RegAddress,
                           Uscc = register.Uscc,
                           Currency = (Currency)register.Currency,
                           RegistFund = register.RegistFund,
                           RegistCurrency = (Currency)register.RegistCurrency,
                           Industry = register.Industry,
                           RegistDate = register.RegistDate,
                           Summary = register.Summary,
                           BusinessState = register.BusinessState,
                           Employees = register.Employees,
                           WebSite = register.WebSite,
                           Nature = register.Nature,
                       }
                   };
        }
    }
}
