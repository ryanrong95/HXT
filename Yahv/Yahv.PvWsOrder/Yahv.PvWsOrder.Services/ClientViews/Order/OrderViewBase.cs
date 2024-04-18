using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.ClientModels;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.Underly;
using Yahv.Services.Models;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 订单基础视图
    /// </summary>
    public class OrderViewBase : OrderAlls
    {
        //订单类型
        private OrderType[] types;
        private IUser User;

        protected internal OrderViewBase()
        {

        }

        protected OrderViewBase(IUser user, params OrderType[] orderTypes)
        {
            this.types = orderTypes;
            this.User = user;
        }

        protected OrderViewBase(IUser user, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery, params OrderType[] orderTypes) : base(reponsitory, iquery)
        {

        }


        protected override IQueryable<WsOrder> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => this.types.Contains(item.Type) && item.ClientID == User.EnterpriseID);

            if (!User.IsMain)
            {
                linq = linq.Where(item => item.CreatorID == User.ID);
            }
            return linq;
        }

        #region 客户取消,确认账单
        /// <summary>
        /// 报关业务客户取消
        /// </summary>
        /// <param name="ID">主键</param>
        /// <param name="reason">原因</param>
        public void ClientCancel(string ID, string reason)
        {
            var order = this[ID];
            if (order == null)
            {
                throw new Exception("订单不存在！");
            }
            //状态更新
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //修改主状态
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.MainStatus);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.MainStatus,
                    Status = (int)CgOrderStatus.取消,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });

                var waybillid = order.Type == OrderType.Declare ? order.Input.WayBillID : order.Output.WayBillID;
                //修改运单状态为关闭
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Status = (int)GeneralStatus.Closed,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == waybillid);
            }

            #region 芯达通接口同步数据
            Task.Run(() =>
            {
                var orderconfirm = new XDTModels.OrderConfirmed()
                {
                    OrderID = order.ID,
                    UserID = this.User.ID,
                    IsCancel = true,
                    CancelReason = reason,
                };

                var message = Extends.NoticeExtends.XDTOrderConfirm(orderconfirm);
                //调用结果日志
                Logger.log(this.User.ID, new OperatingLog
                {
                    MainID = order.ID,
                    Operation = "芯达通订单取消对接结果日志",
                    Summary = message,
                });
            });
            #endregion
        }
        #endregion
    }
}
