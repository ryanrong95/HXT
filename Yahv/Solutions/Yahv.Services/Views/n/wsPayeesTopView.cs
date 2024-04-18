using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 公有收款人
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsPayeesTopView<TReponsitory> : UniqueView<wsPayee, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsPayeesTopView()
        {

        }

        public wsPayeesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsPayee> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsPayeesTopView>()
                   select new wsPayee
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,//内部公司ID
                       EnterpriseName = entity.EnterpriseName,//内部公司名称
                       RealEnterpriseID = entity.RealEnterpriseID,//真正的收款企业ID
                       RealEnterpriseName = entity.RealEnterpriseName,//真正的收款企业
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       Currency = (Currency)entity.Currency,
                       Methord = (Methord)entity.Methord,
                       SwiftCode = entity.SwiftCode,
                       Contact = entity.Contact,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Place = entity.Place,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
