using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Client.Services.PageModels;
using Needs.Wl.Models;
using Needs.Wl.Models.Enums;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyUnReceivedExitNoticeView : View<AllUnReceivedExitNoticeViewModel, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyUnReceivedExitNoticeView(IPlatUser user)
        {
            this.User = user;
        }

        internal MyUnReceivedExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<AllUnReceivedExitNoticeViewModel> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return this.ClientOrders();
            }
            else
            {
                return this.UserOrders();
            }
        }

        private IQueryable<AllUnReceivedExitNoticeViewModel> UserOrders()
        {
            var result = from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                         join exitDelivers in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>() on exitNotice.ID equals exitDelivers.ExitNoticeID into b
                         from exitNotices in b.DefaultIfEmpty()
                         join consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Consignees>() on exitNotices.ConsigneeID equals consignee.ID into c
                         from consignees in c.DefaultIfEmpty()
                         join deliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Delivers>() on exitNotices.DeliverID equals deliver.ID into d
                         from delivers in d.DefaultIfEmpty()
                         join exprssage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Expressages>() on exitNotices.ExpressageID equals exprssage.ID into e
                         from exprssages in e.DefaultIfEmpty()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on exitNotice.OrderID equals order.ID
                         where exitNotice.WarehouseType == (int)WarehouseType.ShenZhen && order.UserID == this.User.ID &&exitNotice.Status ==(int)Status.Normal
                         orderby exitNotice.CreateDate descending
                         select new AllUnReceivedExitNoticeViewModel
                         {
                             ID = exitNotice.ID,
                             OrderID = exitNotice.OrderID,
                             CreateDate = exitNotice.CreateDate,
                             ExitType = (Wl.Models.Enums.ExitType)exitNotice.ExitType,
                             ReceiveCompanyName = exitNotices.Name,
                             ReceiverName = consignees != null ? consignees.Name : (delivers != null ? delivers.Contact : (exprssages != null ? exprssages.Contact : "")),                           
                             MainCreateDate = order.CreateDate,
                             ExitNoticeStatus = (ExitNoticeStatus)exitNotice.ExitNoticeStatus
                         };

            return result;
        }

        private IQueryable<AllUnReceivedExitNoticeViewModel> ClientOrders()
        {
            var result = from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                         join exitDelivers in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>() on exitNotice.ID equals exitDelivers.ExitNoticeID into b
                         from exitNotices in b.DefaultIfEmpty()
                         join consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Consignees>() on exitNotices.ConsigneeID equals consignee.ID into c
                         from consignees in c.DefaultIfEmpty()
                         join deliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Delivers>() on exitNotices.DeliverID equals deliver.ID into d
                         from delivers in d.DefaultIfEmpty()
                         join exprssage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Expressages>() on exitNotices.ExpressageID equals exprssage.ID into e
                         from exprssages in e.DefaultIfEmpty()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on exitNotice.OrderID equals order.ID
                         where exitNotice.WarehouseType == (int)WarehouseType.ShenZhen && exitNotice.Status == (int)Status.Normal
                         && order.ClientID == this.User.ClientID
                         orderby exitNotice.CreateDate descending
                         select new AllUnReceivedExitNoticeViewModel
                         {
                             ID = exitNotice.ID,
                             OrderID = exitNotice.OrderID,
                             CreateDate = exitNotice.CreateDate,
                             ExitType = (Wl.Models.Enums.ExitType)exitNotice.ExitType,
                             ReceiveCompanyName = exitNotices.Name,
                             ReceiverName = consignees != null ? consignees.Name :(delivers!=null?delivers.Contact:(exprssages!=null?exprssages.Contact:"")),
                             MainCreateDate = order.CreateDate,
                             ExitNoticeStatus = (ExitNoticeStatus)exitNotice.ExitNoticeStatus
                         };

            return result;
        }
    }
}