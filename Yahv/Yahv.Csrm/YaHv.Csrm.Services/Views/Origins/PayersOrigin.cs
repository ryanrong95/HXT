using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class PayersOrigin : Yahv.Linq.UniqueView<Payer, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal PayersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal PayersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Payer> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Payers>()
                   join et in enterpriseView on entity.RealID equals et.ID into ets
                   from enterprise in ets.DefaultIfEmpty()
                   select new Payer
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
                       Place = entity.Place
                   };

        }
    }

}
