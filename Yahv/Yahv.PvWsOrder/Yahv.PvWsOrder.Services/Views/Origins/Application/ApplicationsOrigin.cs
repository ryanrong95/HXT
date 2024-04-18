using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 代收付款申请视图
    /// </summary>
    public class ApplicationsOrigin : UniqueView<Application, PvWsOrderReponsitory>
    {
        internal protected ApplicationsOrigin()
        {

        }

        public ApplicationsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Application> GetIQueryable()
        {
            var linq = from apply in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
                       select new Application()
                       {
                           ID = apply.ID,
                           ClientID = apply.ClientID,
                           Type = (ApplicationType)apply.Type,
                           TotalPrice = apply.TotalPrice,
                           Currency = (Currency)apply.Currency,
                           InCompanyName = apply.InCompanyName,
                           InBankName = apply.InBankName,
                           InBankAccount = apply.InBankAccount,
                           OutCompanyName = apply.OutCompanyName,
                           OutBankName = apply.OutBankName,
                           OutBankAccount = apply.OutBankAccount,
                           Status = (GeneralStatus)apply.Status,
                           ApplicationStatus = (ApplicationStatus)apply.ApplicationStatus,
                           ReceiveStatus = (ApplicationReceiveStatus)apply.ReceiveStatus,
                           PaymentStatus = (ApplicationPaymentStatus)apply.PaymentStatus,
                           IsEntry = apply.IsEntry,
                           DelivaryOpportunity = (DelivaryOpportunity)apply.DelivaryOpportunity,
                           CheckCarrier = apply.CheckCarrier,
                           CheckDelivery = (CheckDeliveryType)apply.CheckDelivery,
                           CheckWaybillCode = apply.CheckWaybillCode,
                           CreateDate = apply.CreateDate,
                           PaymentDate = apply.PaymentDate,
                           ReceiveDate = apply.ReceiveDate,
                           UserID = apply.UserID,
                           CheckConsignee = apply.CheckConsignee,
                           CheckPayeeAccount = apply.CheckPayeeAccount,
                           CheckTitle = apply.CheckTitle,
                           RateToUSD = apply.RateToUSD,
                           HandlingFeePayerType = apply.HandlingFeePayerType != null ? apply.HandlingFeePayerType.ToString() : null,
                           HandlingFee = apply.HandlingFee,
                           USDRate = apply.USDRate,
                       };
            return linq;
        }
    }
}
