using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvRoute.Services.Enums
{
    public enum PKeyType
    {
        /// <summary>
        /// 账单
        /// </summary>
        [Repository(typeof(PvRouteReponsitory))]
        [PKey("Bill", PKeySigner.Mode.Date, 4)]
        Bill,

        /// <summary>
        /// 账单联系人
        /// </summary>
        [Repository(typeof(PvRouteReponsitory))]
        [PKey("BillContact", PKeySigner.Mode.Normal, 4)]
        BillContact,

        /// <summary>
        /// 运输日志
        /// </summary>
        [Repository(typeof(PvRouteReponsitory))]
        [PKey("LogsTransports", PKeySigner.Mode.Date, 4)]
        LogsTransports,

        /// <summary>
        /// 运输日志收货人
        /// </summary>
        [Repository(typeof(PvRouteReponsitory))]
        [PKey("TransportConsignees", PKeySigner.Mode.Normal, 4)]
        TransportConsignees

    }
}
