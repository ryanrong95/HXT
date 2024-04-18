using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.DappApi.Services.Enums
{
    public enum FromType
    {
#if DEBUG
        /// <summary>
        /// 文件上传接口
        /// </summary>
        [Description("http://sz.warehouse.b1b.com/dappapi/Files/upload")]
        FileUpload = 1,

        /// <summary>
        /// 入库复核后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz0.ic360.cn/Order/ChangeOrderStatus")]
        ChangeOrder = 2,

        /// <summary>
        /// 同步费用到管理端
        /// </summary>
        [Description("http://apisz0.ic360.cn/PayeeLeft/SynPayeeLeft")]
        SynPayeeLeft = 3,

        /// <summary>
        /// 网站接口地址
        /// </summary>
        [Description("http://hv.erp.b1b.com")]
        SzApiUrl = 4,

        /// <summary>
        /// 出库后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz0.ic360.cn/Order/RealChangeOrderStatus")]
        RealChangeOrder = 5

#elif TEST
        /// <summary>
        /// 文件上传接口
        /// </summary>
        [Description("http://sz.warehouse0.ic360.cn/dappapi/Files/upload")]
        FileUpload = 1,

        /// <summary>
        /// 入库复核后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz0.ic360.cn/Order/ChangeOrderStatus")]
        ChangeOrder = 2,

        /// <summary>
        /// 同步费用到管理端
        /// </summary>
        [Description("http://apisz0.ic360.cn/PayeeLeft/SynPayeeLeft")]
        SynPayeeLeft = 3,

        /// <summary>
        /// 网站接口地址
        /// </summary>
        [Description("http://apisz0.ic360.cn/")]
        SzApiUrl = 4,

        /// <summary>
        /// 出库后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz0.ic360.cn/Order/RealChangeOrderStatus")]
        RealChangeOrder = 5
#else
        /// <summary>
        /// 文件上传接口
        /// </summary>
        [Description("http://sz.warehouse.for-ic.net:60077/dappapi/Files/upload")]
        FileUpload = 1,

        /// <summary>
        /// 入库复核后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz.wapi.for-ic.net:60077/Order/ChangeOrderStatus")]
        ChangeOrder = 2,

        /// <summary>
        /// 同步费用到管理端
        /// </summary>
        [Description("http://apisz.wapi.for-ic.net:60077/PayeeLeft/SynPayeeLeft")]
        SynPayeeLeft = 3,

        /// <summary>
        /// 网站接口地址
        /// </summary>
        [Description("http://apisz.wapi.for-ic.net:60077")]
        SzApiUrl = 4,

        /// <summary>
        /// 出库后通知管理端,并更新订单状态
        /// </summary>
        [Description("http://apisz.wapi.for-ic.net:60077/Order/RealChangeOrderStatus")]
        RealChangeOrder = 5
#endif
    }
}
