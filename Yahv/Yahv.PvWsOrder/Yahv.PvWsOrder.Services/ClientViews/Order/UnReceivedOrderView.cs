using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 待确认收货订单
    /// </summary>
    public class UnReceivedOrderView : UniqueView<ReceiptedOrder, PvWsOrderReponsitory>
    {
        private readonly IUser _user;

        private UnReceivedOrderView()
        {

        }

        public UnReceivedOrderView(IUser user)
        {
            this._user = user;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ReceiptedOrder> GetIQueryable()
        {
            //深圳待确认收货订单
            var waybills = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>().Where(item => item.WareHouseID.StartsWith("SZ")).
                Where(item => item.ConfirmReceiptStatus == (int)ConfirmReceiptStatus.UnConfirm);

            var orders = new WsOrdersTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.ClientID == this._user.EnterpriseID);

            if (!this._user.IsMain)
            {
                orders = orders.Where(item => item.CreatorID == this._user.ID);
            }

            return from order in orders
                   join waybill in waybills on order.ID equals waybill.OrderID into _waybills
                   where _waybills.Any()
                   select new ReceiptedOrder
                   {
                       ID = order.ID,
                       MainStatus = order.MainStatus,
                       Type = order.Type,
                       CreateDate = order.CreateDate,
                       OutWaybills = _waybills.Select(item => new Waybill
                       {
                           ID = item.wbID,
                           Code = item.wbCode,
                           Type = (WaybillType)item.wbType,
                           Subcodes = item.wbSubcodes,
                           CreateDate = item.wbModifyDate ?? item.wbCreateDate,
                           Consignee = new Yahv.Services.Models.WayParter()
                           {
                               ID = item.coeID,
                               Company = item.coeCompany,
                               Place = item.coePlace,
                               Address = item.coeAddress,
                               Contact = item.coeContact,
                               Phone = item.coePhone,
                               Zipcode = item.coeZipcode,
                               Email = item.coeEmail,
                               CreateDate = item.coeCreateDate,
                               IDType = (IDType?)item.coeIDType,
                               IDNumber = item.coeIDNumber,
                           },
                       }).ToArray(),
                   };
        }

        /// <summary>
        /// 获取“待确认”和“已确认”的订单
        /// </summary>
        public Tuple<ReceiptedOrder[], int> GetAllConfirmReceiptOrders(
            Expression<Func<Yahv.Services.Models.WsOrder, bool>> expOrder,
            int? confirmReceiptStatus,
            int? waybillType,
            int page = 1, int rows = 10)
        {
            //深圳待确认收货订单
            var waybillsVew = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CgWaybillsTopView>().Where(item => item.WareHouseID.StartsWith("SZ"));

            var orders = new WsOrdersTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.ClientID == this._user.EnterpriseID);

            if (!this._user.IsMain)
            {
                orders = orders.Where(item => item.CreatorID == this._user.ID);
            }

            if (expOrder.Body.ToString() != "True")
            {
                orders = orders.Where(expOrder);
            }

            #region 通过加在 waybill 上的条件筛选

            var waybillsFiltered = waybillsVew;
            if (confirmReceiptStatus != null)
            {
                waybillsFiltered = waybillsFiltered.Where(t => t.ConfirmReceiptStatus == (int)confirmReceiptStatus);
            }
            if (waybillType != null)
            {
                waybillsFiltered = waybillsFiltered.Where(t => t.wbType == (int)waybillType);
            }

            #endregion

            var linqAllOrderIds = from waybill in waybillsFiltered
                                  select waybill.OrderID;
            var linqOrderIds = linqAllOrderIds.Distinct();

            orders = from order in orders
                     join orderId in linqOrderIds on order.ID equals orderId
                     select order;

            orders = orders.OrderByDescending(t => t.ID);


            int total = orders.Count();
            var pagedOrders = orders.Skip(rows * (page - 1)).Take(rows).ToArray();

            var resultOrderIds = pagedOrders.Select(t => t.ID).ToArray();
            var resultWaybills = waybillsVew.Where(t => resultOrderIds.Contains(t.OrderID)).ToArray();

            var result = from order in pagedOrders
                         join waybill in resultWaybills on order.ID equals waybill.OrderID into _waybills
                         // where _waybills.Any()
                         select new ReceiptedOrder
                         {
                             ID = order.ID,
                             MainStatus = order.MainStatus,
                             Type = order.Type,
                             CreateDate = order.CreateDate,
                             OutWaybills = _waybills.Select(item => new Waybill
                             {
                                 ID = item.wbID,
                                 Code = item.wbCode,
                                 Type = (WaybillType)item.wbType,
                                 Subcodes = item.wbSubcodes,
                                 CreateDate = item.wbModifyDate ?? item.wbCreateDate,
                                 Consignee = new Yahv.Services.Models.WayParter()
                                 {
                                     ID = item.coeID,
                                     Company = item.coeCompany,
                                     Place = item.coePlace,
                                     Address = item.coeAddress,
                                     Contact = item.coeContact,
                                     Phone = item.coePhone,
                                     Zipcode = item.coeZipcode,
                                     Email = item.coeEmail,
                                     CreateDate = item.coeCreateDate,
                                     IDType = (IDType?)item.coeIDType,
                                     IDNumber = item.coeIDNumber,
                                 },
                                 ConfirmReceiptStatus = item.ConfirmReceiptStatus,
                             }).ToArray(),
                         };

            return new Tuple<ReceiptedOrder[], int>(result.ToArray(), total);
        }

        private DateTime? 获取用于排序OutWaybillCreateDate(IEnumerable<Layers.Data.Sqls.PvWms.CgWaybillsTopView> _waybills)
        {
            // _waybills.Any(t => t.ConfirmReceiptStatus == (int)ConfirmReceiptStatus.UnConfirm)
            if (_waybills == null || !_waybills.Any())
            {
                return null;
            }

            Layers.Data.Sqls.PvWms.CgWaybillsTopView theWaybill;
            if (_waybills.Any(t => t.ConfirmReceiptStatus == (int)ConfirmReceiptStatus.UnConfirm))
            {
                theWaybill = _waybills
                    .Where(t => t.ConfirmReceiptStatus == (int)ConfirmReceiptStatus.UnConfirm)
                    .OrderBy(t => t.wbModifyDate ?? t.wbCreateDate).FirstOrDefault();
            }
            else
            {
                theWaybill = _waybills.OrderBy(t => t.wbModifyDate ?? t.wbCreateDate).FirstOrDefault();
            }

            if (theWaybill == null)
            {
                return null;
            }
            return theWaybill.wbModifyDate ?? theWaybill.wbCreateDate;
        }

        /// <summary>
        /// 确认收货处理
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="waybillId"></param>
        public void ReceiptOrder(string orderId, string waybillId)
        {
            if (this[orderId] == null)
            {
                throw new Exception("订单不存在！");
            }

            //校验是否存在未收货运单
            var isReceipt = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>().Any(item => item.OrderID == orderId && item.WareHouseID.StartsWith("SZ") && item.wbID != waybillId && item.ConfirmReceiptStatus != (int)ConfirmReceiptStatus.Confirmed);

            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            {
                //更新状态为客户已收货
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ConfirmReceiptStatus = (int)ConfirmReceiptStatus.Confirmed,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == waybillId);
                //订单确认收货
                if (!isReceipt)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.MainID == orderId && item.Type == (int)OrderStatusType.MainStatus);
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderId,
                        Type = (int)OrderStatusType.MainStatus,
                        Status = (int)CgOrderStatus.客户已收货,
                        CreateDate = DateTime.Now,
                        CreatorID = this._user.ID,
                        IsCurrent = true,
                    });

                    #region 状态同步给芯达通
                    var apisetting = new WlAdminApiSetting();
                    var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ConfirmReceipted;
                    var data = new { MainOrderID = orderId };

                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
                    Logger.log(this._user.ID, new Yahv.Services.Models.OperatingLog
                    {
                        MainID = orderId,
                        Operation = "芯达通订单客户确认收货对接结果日志！",
                        Summary = result,
                    });
                    #endregion
                }
            }
        }
    }

    /// <summary>
    /// 待确认收货订单
    /// </summary>
    public class ReceiptedOrder : Order
    {
        /// <summary>
        /// 深圳出库运单
        /// </summary>
        public Waybill[] OutWaybills { get; set; }

        public DateTime? 用于排序OutWaybillCreateDate { get; set; }

        public int? 用于排序ConfirmReceiptStatus { get; set; }
    }


    /// <summary>
    /// 确认收货状态
    /// </summary>
    public enum ConfirmReceiptStatus
    {
        [Description("待确认")]
        UnConfirm = 100,

        [Description("已确认")]
        Confirmed = 200,
    }
}
