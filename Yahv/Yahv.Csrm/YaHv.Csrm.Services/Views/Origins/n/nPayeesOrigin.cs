using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class nPayeesOrigin : Yahv.Linq.UniqueView<nPayee, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal nPayeesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nPayeesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nPayee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nPayees>()
                   join et in enterpriseView on entity.RealID equals et.ID into ets
                   from enterprise in ets.DefaultIfEmpty()
                   select new nPayee()
                   {
                       ID = entity.ID,
                       nSupplierID = entity.nSupplierID,
                       EnterpriseID = entity.EnterpriseID,
                       RealID = entity.RealID,
                       RealEnterprise = enterprise,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       SwiftCode = entity.SwiftCode,
                       Methord = (Methord)entity.Methord,
                       Currency = (Currency)entity.Currency,
                       Contact = entity.Contact,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Creator = entity.Creator,
                       IsDefault = entity.IsDefault,
                       Place=entity.Place
                   };

        }
    }
}
