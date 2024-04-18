using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Enums
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        [Description("管理员日志")]
        Admin = 100,

        [Description("代仓储订单日志")]
        WsOrder = 200,

        [Description("错误日志")]
        Error = 300,
        
        [Description("送货单打印日志")]
        DeliveryPrint=400,

        [Description("库房管理日志")]
        WarehouseManagement = 500



    }
}
