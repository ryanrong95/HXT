using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class PayeesOrigin : Yahv.Linq.UniqueView<Payee, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal PayeesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal PayeesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Payee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Payees>()
                   join et in enterpriseView on entity.RealID equals et.ID into ets
                   from enterprise in ets.DefaultIfEmpty()
                   select new Payee()
                   {
                       ID = entity.ID,
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
                       Place=entity.Place
                   };

        }
    }

}
