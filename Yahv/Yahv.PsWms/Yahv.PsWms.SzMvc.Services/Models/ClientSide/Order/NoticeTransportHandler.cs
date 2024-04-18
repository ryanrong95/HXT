using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    /// <summary>
    /// 同步货运信息
    /// </summary>
    public class NoticeTransportHandler
    {
        private string _OrderID { get; set; }

        public NoticeTransportHandler(string orderID)
        {
            this._OrderID = orderID;
        }

        public void Sync()
        {
            NoticeTransportsTopViewModel noticeTransportsTopViewModel;

            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                noticeTransportsTopViewModel = repository.ReadTable<Layers.Data.Sqls.PsOrder.NoticeTransportsTopView>()
                                                .Where(t => t.FormID == this._OrderID)
                                                .Select(item => new NoticeTransportsTopViewModel
                                                {
                                                    NoticeID = item.NoticeID,
                                                    NoticeType = item.NoticeType,
                                                    FormID = item.FormID,
                                                    NoticeTransportID = item.NoticeTransportID,
                                                    TransportMode = item.TransportMode,
                                                    Carrier = item.Carrier,
                                                    WaybillCode = item.WaybillCode,
                                                    ExpressPayer = item.ExpressPayer,
                                                    ExpressTransport = item.ExpressTransport,
                                                }).FirstOrDefault();
            }

            if (noticeTransportsTopViewModel == null)
            {
                new Log
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.SyncOrderTransportQuery.GetDescription(),
                    MainID = this._OrderID,
                    Content = "根据 OrderID = " + this._OrderID + " 在 NoticeTransportsTopView 中查不到数据",
                    CreateDate = DateTime.Now,
                }.Insert();

                return;
            }

            string noticeTransportsTopViewContent = JsonConvert.SerializeObject(noticeTransportsTopViewModel);
            new Log
            {
                ID = Guid.NewGuid().ToString("N"),
                ActionName = LogAction.SyncOrderTransportQuery.GetDescription(),
                MainID = this._OrderID,
                Content = noticeTransportsTopViewContent,
                CreateDate = DateTime.Now,
            }.Insert();

            //入库订单
            if (noticeTransportsTopViewModel.NoticeType == (int)NoticeType.Inbound)
            {
                //快递
                if (noticeTransportsTopViewModel.TransportMode == (int)TransportMode.Express)
                {
                    using (var repository = new Layers.Data.Sqls.PsOrderRepository())
                    {
                        repository.Update<Layers.Data.Sqls.PsOrder.OrderTransports>(new
                        {
                            Carrier = noticeTransportsTopViewModel.Carrier,
                            WaybillCode = noticeTransportsTopViewModel.WaybillCode,
                        }, item => item.OrderID == this._OrderID);
                    }
                }
            }
            //出库订单
            else if (noticeTransportsTopViewModel.NoticeType == (int)NoticeType.Outbound)
            {
                //快递
                if (noticeTransportsTopViewModel.TransportMode == (int)TransportMode.Express)
                {
                    using (var repository = new Layers.Data.Sqls.PsOrderRepository())
                    {
                        repository.Update<Layers.Data.Sqls.PsOrder.OrderTransports>(new
                        {
                            Carrier = noticeTransportsTopViewModel.Carrier,
                            WaybillCode = noticeTransportsTopViewModel.WaybillCode,
                            ExpressPayer = noticeTransportsTopViewModel.ExpressPayer ?? 0,
                            ExpressTransport = noticeTransportsTopViewModel.ExpressTransport,
                        }, item => item.OrderID == this._OrderID);
                    }
                }
            }
        }

        public class NoticeTransportsTopViewModel
        {
            /// <summary>
            /// 通知ID
            /// </summary>
            public string NoticeID { get; set; }

            /// <summary>
            /// 通知类型 1-入库 2-出库
            /// </summary>
            public int NoticeType { get; set; }

            /// <summary>
            /// 订单ID
            /// </summary>
            public string FormID { get; set; }

            /// <summary>
            /// 发货人/收货人 ID
            /// </summary>
            public string NoticeTransportID { get; set; }

            /// <summary>
            /// 货运方式 1-自提 2-快递 3-送货上门
            /// </summary>
            public int TransportMode { get; set; }

            /// <summary>
            /// 承运商
            /// </summary>
            public string Carrier { get; set; }

            /// <summary>
            /// 运单号
            /// </summary>
            public string WaybillCode { get; set; }

            /// <summary>
            /// 支付方式
            /// </summary>
            public int? ExpressPayer { get; set; }

            /// <summary>
            /// 承运类型
            /// </summary>
            public string ExpressTransport { get; set; }
        }
    }
}
