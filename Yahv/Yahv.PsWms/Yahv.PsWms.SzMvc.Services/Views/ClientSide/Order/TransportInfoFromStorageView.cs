using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class TransportInfoFromStorageView : QueryView<TransportInfoFromStorageViewModel, PsOrderRepository>
    {
        public TransportInfoFromStorageView()
        {
        }

        protected TransportInfoFromStorageView(PsOrderRepository reponsitory, IQueryable<TransportInfoFromStorageViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<TransportInfoFromStorageViewModel> GetIQueryable()
        {
            var noticeTransportsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.NoticeTransportsTopView>();

            var iQuery = from noticeTransports in noticeTransportsTopView
                         select new TransportInfoFromStorageViewModel
                         {
                             NoticeID = noticeTransports.NoticeID,
                             NoticeType = noticeTransports.NoticeType,
                             FormID = noticeTransports.FormID,
                             NoticeTransportID = noticeTransports.NoticeTransportID,
                             TransportMode = noticeTransports.TransportMode,
                             Carrier = noticeTransports.Carrier,
                             WaybillCode = noticeTransports.WaybillCode,
                             ExpressPayer = noticeTransports.ExpressPayer,
                             ExpressTransport = noticeTransports.ExpressTransport,
                         };

            return iQuery;
        }
    }

    public class TransportInfoFromStorageViewModel
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
