using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 收付款申请视图
    /// </summary>
    public class ApplicationsAlls : UniqueView<Application, PvWsOrderReponsitory>
    {
        public ApplicationsAlls()
        {

        }

        public ApplicationsAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Application> GetIQueryable()
        {
            var clientView = new WsClientsAlls(this.Reponsitory);
            var payeeView = new ApplicationPayeesOrigin(this.Reponsitory);
            var payerView = new ApplicationPayersRoll(this.Reponsitory);
            var itemView = new ApplicationItemsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal);
            var applications = new ApplicationsOrigin(this.Reponsitory)
                .Where(item => item.Status == GeneralStatus.Normal).OrderByDescending(item => item.CreateDate).AsQueryable();

            var linq = from apply in applications
                       join client in clientView on apply.ClientID equals client.ID
                       join payer in payerView on apply.ID equals payer.ApplicationID into payers
                       join payee in payeeView on apply.ID equals payee.ApplicationID into payees
                       join item in itemView on apply.ID equals item.ApplicationID into items
                       select new Application()
                       {
                           ID = apply.ID,
                           ClientID = apply.ClientID,
                           Type = apply.Type,
                           TotalPrice = apply.TotalPrice,
                           Currency = apply.Currency,
                           InCompanyName = apply.InCompanyName,
                           InBankName = apply.InBankName,
                           InBankAccount = apply.InBankAccount,
                           OutCompanyName = apply.OutCompanyName,
                           OutBankName = apply.OutBankName,
                           OutBankAccount = apply.OutBankAccount,
                           Status = apply.Status,
                           ApplicationStatus = apply.ApplicationStatus,
                           ReceiveStatus = apply.ReceiveStatus,
                           PaymentStatus = apply.PaymentStatus,
                           IsEntry = apply.IsEntry,
                           DelivaryOpportunity = apply.DelivaryOpportunity,
                           CheckCarrier = apply.CheckCarrier,
                           CheckDelivery = apply.CheckDelivery,
                           CheckWaybillCode = apply.CheckWaybillCode,
                           CreateDate = apply.CreateDate,
                           PaymentDate = apply.PaymentDate,
                           ReceiveDate = apply.ReceiveDate,
                           UserID = apply.UserID,
                           CheckConsignee = apply.CheckConsignee,
                           CheckPayeeAccount = apply.CheckPayeeAccount,
                           CheckTitle = apply.CheckTitle,
                           RateToUSD = apply.RateToUSD,


                           Client = client,
                           Payees = payees,
                           Payers = payers,
                           Items = items,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 管理员代收代付货款申请
    /// </summary>
    public class MyApplicationView : QueryRoll<Application, Application, PvWsOrderReponsitory>
    {
        //系统管理员
        private Underly.Erps.IErpAdmin admin;

        //主体公司
        private string CompanyID;

        public MyApplicationView(Underly.Erps.IErpAdmin admin, string companyID)
        {
            this.admin = admin;
            this.CompanyID = companyID;
        }

        protected override IQueryable<Application> GetIQueryable(Expression<Func<Application, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = new ApplicationsAlls(this.Reponsitory).AsQueryable();

            if (!admin.IsSuper)
            {
                //管理员的代仓储客户
                var clientIds = new Yahv.Services.Views.TrackerWsClients<PvWsOrderReponsitory>(this.Reponsitory, this.admin, this.CompanyID)
                    .Select(item => item.ID).ToArray();
                linq = linq.Where(item => clientIds.Contains(item.ClientID));
            }

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Application, bool>>);
            }
            return linq.Where(expression);
        }

        protected override IEnumerable<Application> OnReadShips(Application[] results)
        {
            var linq = from apply in results
                       select new Application
                       {
                           ID = apply.ID,
                           ClientID = apply.ClientID,
                           Type = apply.Type,
                           TotalPrice = apply.TotalPrice,
                           Currency = apply.Currency,
                           InCompanyName = apply.InCompanyName,
                           InBankName = apply.InBankName,
                           InBankAccount = apply.InBankAccount,
                           OutCompanyName = apply.OutCompanyName,
                           OutBankName = apply.OutBankName,
                           OutBankAccount = apply.OutBankAccount,
                           Status = apply.Status,
                           ApplicationStatus = apply.ApplicationStatus,
                           ReceiveStatus = apply.ReceiveStatus,
                           PaymentStatus = apply.PaymentStatus,
                           IsEntry = apply.IsEntry,
                           DelivaryOpportunity = apply.DelivaryOpportunity,
                           CheckCarrier = apply.CheckCarrier,
                           CheckDelivery = apply.CheckDelivery,
                           CheckWaybillCode = apply.CheckWaybillCode,
                           CreateDate = apply.CreateDate,
                           PaymentDate = apply.PaymentDate,
                           ReceiveDate = apply.ReceiveDate,
                           UserID = apply.UserID,
                           CheckConsignee = apply.CheckConsignee,
                           CheckPayeeAccount = apply.CheckPayeeAccount,
                           CheckTitle = apply.CheckTitle,
                           RateToUSD = apply.RateToUSD,

                           Client = apply.Client,
                           Payees = apply.Payees,
                           Payers = apply.Payers,
                           Items = apply.Items,
                       };
            return linq;
        }
    }
}
