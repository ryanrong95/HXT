using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    /// <summary>
    /// Api接口设置
    /// </summary>
    public class WlAdminApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 预归类产品自动归类
        /// </summary>
        public string AutoPreClassify { get; private set; }

        /// <summary>
        /// 订单提交接口
        /// </summary>
        public string GetReceptorOrder { get; private set; }

        /// <summary>
        /// 订单文件对接接口
        /// </summary>
        public string MainOrderFile { get; private set; }

        /// <summary>
        /// 客户确认接口
        /// </summary>
        public string OrderConfirm { get; private set; }

        /// <summary>
        /// 付汇申请
        /// </summary>
        public string PayExchange { get; private set; }

        /// <summary>
        /// 删除付汇申请
        /// </summary>
        public string DeletePayExchange { get; private set; }

        /// <summary>
        /// 确认收货对接
        /// </summary>
        public string ConfirmReceipted { get; private set; }

        /// <summary>
        /// 代仓储添加订单费用
        /// </summary>
        public string AddOrderPremiums { get; set; }


        public WlAdminApiSetting()
        {
            AutoPreClassify = "api/PreClassify/AutoClassify";
            GetReceptorOrder = "Order/GetReceptorOrder";
            OrderConfirm = "Order/OrderConfirm";
            MainOrderFile = "Order/OrderFile";
            PayExchange = "api/payExchangeapplys";
            DeletePayExchange = "api/deleteapply";
            ApiName = "WlAdminApiUrl";
            ConfirmReceipted = "Order/UserConfirmReceipt";
            AddOrderPremiums = "Order/AddOrderPremiums";
        }
    }
}
