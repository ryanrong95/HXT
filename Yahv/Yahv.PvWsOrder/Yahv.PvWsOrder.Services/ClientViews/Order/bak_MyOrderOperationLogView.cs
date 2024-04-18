using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Enums;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    //public class MyOrderOperationLogView : UniqueView<OrderOperationLog, PvWsOrderReponsitory>
    //{
    //    private IUser user;

    //    private MyOrderOperationLogView()
    //    {

    //    }

    //    private MyOrderOperationLogView(IUser User)
    //    {
    //        this.user = User;
    //    }

    //    protected override IQueryable<OrderOperationLog> GetIQueryable()
    //    {
    //        var OrderView = new OrderBaseOrigin(Reponsitory).Where(item => item.ClientID == this.user.EnterpriseID);
    //        if(!user.IsMain)
    //        {
    //            OrderView = OrderView.Where(item => item.CreatorID == user.ID);
    //        }
    //        //获取所有日志信息
    //        var operationlogview = new Logs_OperatingTopView<PvWsOrderReponsitory>(Reponsitory).Where(item => item.Type == LogType.WsOrder);

    //        return from order in OrderView
    //               join operation in operationlogview on order.ID equals operation.MainID
    //               orderby operation.CreateDate descending
    //               select new OrderOperationLog
    //               {
    //                   Order = order,
    //                   ID = operation.ID,
    //                   Type = operation.Type,
    //                   MainID = operation.MainID,
    //                   OrderPaymentStatus=order.PaymentStatus,
    //                   Operation = operation.Operation,
    //                   Summary = operation.Summary,
    //                   CreateDate = operation.CreateDate,
    //                   Creator = operation.Creator,
    //               };
    //    }
    //}
}
