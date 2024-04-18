using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 申请中客户的付款人视图
    /// </summary>
    public class ApplicationPayersOrigin : UniqueView<ApplicationPayer, PvWsOrderReponsitory>
    {
        protected ApplicationPayersOrigin()
        {

        }

        public ApplicationPayersOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApplicationPayer> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>()
                       select new ApplicationPayer()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           PayerID = entity.PayerID,
                           EnterpriseID = entity.EnterpriseID,
                           EnterpriseName = entity.EnterpriseName,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           Method = (Methord)entity.Method,
                           Currency = (Currency)entity.Currency,
                           Amount = entity.Amount
                       };
            return linq;
        }
    }

    /// <summary>
    /// 申请中客户的付款人视图
    /// </summary>
    public class ApplicationPayersRoll : UniqueView<ApplicationPayer, PvWsOrderReponsitory>
    {
        protected ApplicationPayersRoll()
        {

        }

        public ApplicationPayersRoll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApplicationPayer> GetIQueryable()
        {
            var wspayerView = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.wsPayersTopView>();

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>()
                       join wspayer in wspayerView on entity.PayerID equals wspayer.ID
                       select new ApplicationPayer()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           PayerID = entity.PayerID,
                           EnterpriseID = entity.EnterpriseID,
                           EnterpriseName = entity.EnterpriseName,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           Method = (Methord)entity.Method,
                           Currency = (Currency)entity.Currency,
                           Amount = entity.Amount,
                           Contact = wspayer.Contact,
                       };
            return linq;
        }
    }
}
