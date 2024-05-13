using System.Linq;
using Layers.Data.Sqls;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Wms.Services.Views
{
    /// <summary>
    /// 收付款申请视图
    /// </summary>
    public class ApplicationsTopView : UniqueView<Application, PvWmsRepository>
    {
        public ApplicationsTopView()
        {

        }

        public ApplicationsTopView(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Application> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ApplicationItemsTopView>()
                   select new Application()
                   {
                       ID = entity.ID,
                       Currency = (Currency)entity.Currency,
                       CreateDate = entity.CreateDate,
                       ClientID = entity.ClientID,
                       Type = (ApplicationType)entity.Type,
                       ApplicationStatus = (ApplicationStatus)entity.ApplicationStatus,
                       CheckCarrier = entity.CheckCarrier,
                       CheckDeliveryType = (CheckDeliveryType)entity.CheckDelivery,
                       CheckWaybillCode = entity.CheckWaybillCode,
                       DelivaryOpportunity = entity.DelivaryOpportunity,
                       InBankAccount = entity.InBankAccount,
                       InBankName = entity.InBankName,
                       InCompanyName = entity.InCompanyName,
                       IsEntry = entity.IsEntry,
                       OutBankAccount = entity.OutBankAccount,
                       OutBankName = entity.OutBankName,
                       OutCompanyName = entity.OutCompanyName,
                       PaymentDate = entity.PaymentDate,
                       PaymentStatus = (ApplicationPaymentStatus)entity.PaymentStatus,
                       ReceiveDate = entity.ReceiveDate,
                       ReceiveStatus = (ApplicationReceiveStatus)entity.ReceiveStatus,
                       Status = (GeneralStatus)entity.Status,
                       TotalPrice = entity.TotalPrice,
                       UserID = entity.UserID,
                       OrderID = entity.OrderID,
                   };
        }
    }
}