using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    public class PvWsOrderApiSetting
    {
        public string ApiName { get; internal set; }

        /// <summary>
        /// 到货信息推送代仓储后端
        /// </summary>
        public string SubmitChanged { get; private set; }

        /// <summary>
        /// 确认订单推送代仓储前端
        /// </summary>
        public string ClientConfirm { get; private set; }

        /// <summary>
        /// 同步3C审批结果
        /// </summary>
        public string SyncCccControl { get; private set; }

        /// <summary>
        /// 审核附件告知代仓储前端
        /// </summary>
        public string FileApprove { get; private set; }

        /// <summary>
        /// 调用客户端地址，内单信息给客户端
        /// </summary>
        public string OrderSubmit { get; set; }

        /// <summary>
        /// 调用客户端，订单退回
        /// </summary>
        public string OrderReturn { get; set; }

        /// <summary>
        /// 调用客户端，开票完成调用
        /// </summary>
        public string InvoiceComplete { get; set; }

        /// <summary>
        /// 跟单替客户确认报价
        /// </summary>
        public string DeclareConfirm { get; set; }
        /// <summary>
        /// 更改EnterCode
        /// </summary>
        public string UpdateEnterCode { get; set; }
        /// <summary>
        /// 更改订单状态
        /// </summary>
        public string UpdateOrderStatusToBoxed { get; set; }

        public PvWsOrderApiSetting()
        {
            ClientConfirm = "ClientConfirm/ClientConfirm";
            SubmitChanged = "Sorted/SubmitChanged";
            SyncCccControl = "Classify/SyncCccControl";
            FileApprove = "XDTFile/FileApprove";
            ApiName = "PvWsOrderApi";
            OrderSubmit = "Order/OrderSubmit";
            OrderReturn = "Order/OrderReturn";
            InvoiceComplete = "Order/InvoiceComplete";
            DeclareConfirm = "Order/DeclareConfirm ";
            UpdateEnterCode = "Order/UpdateEnterCode";
            UpdateOrderStatusToBoxed = "Order/UpdateOrderStatusToBoxed";
        }
    }
}
