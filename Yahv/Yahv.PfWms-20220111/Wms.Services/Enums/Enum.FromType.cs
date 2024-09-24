using System;
using Yahv.Underly.Attributes;

//#define DEVELOP     //开发
//#define TEST      //测试版
//#define FINAL     //生产版

namespace Wms.Services
{
    public enum FromType
    {

#if DEBUG
        [Description("http://hv.erp.b1b.com/PvWsOrderApi")]
        OrderApi = 3,
        [Description("http://api0.for-ic.net/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToXDT = 4,
        [Description("http://api0.for-ic.net/Declaration/DeclarationNotice")]
        CustomApply = 5,

        /// <summary>
        /// 华芯通 深圳出库接口
        /// </summary>
        /// <remarks>
        /// 荣检
        /// </remarks>
        [Description("http://api0.for-ic.net/SZWarehouse/OutStock")]
        XdtSZShiped = 6,
        /// <summary>
        /// 深圳确认收货接口 
        /// </summary>
        /// <remarks>
        /// 荣检
        /// </remarks>
        [Description("http://api0.for-ic.net/SZWarehouse/ReceiptConfirmed")]
        XdtSZSZReceiptConfirm = 7,
        [Description("http://api0.for-ic.net/Finance/InsertInvoiceNoticeFiles")]
        InsertInvoiceNoticeFiles = 8,
        [Description("http://api0.for-ic.net/Finance/DeleteInvoiceNoticeFiles")]
        DeleteInvoiceNoticeFiles = 9,
        /// <summary>
        /// 通知变更
        /// </summary>
        /// <remarks>
        /// 董建接口
        /// </remarks>
        [Description("http://hv.erp.b1b.com/PvWsOrderApi/Sorted/SubmitSorted")]
        NoticeDelivery,
        /// <summary>
        /// 产品变更通知(品牌,产地的变更)
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://api0.for-ic.net/HKWarehouse/ItemChangeNotice")]
        ItemChangeNotice,

        /// <summary>
        /// 拆项变更通知
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://api0.for-ic.net/HKWarehouse/SplitChangeNotice")]
        SplitChangeNotice,

        /// <summary>
        /// 同步记账
        /// </summary>
        /// <remarks>
        /// 荣检接口
        /// </remarks>
        [Description("http://api0.for-ic.net/Finance/InsertHKWarehouseFee")]
        ForReceivables,
        [Description("http://api0.wl.net.cn/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToHY,

        /// <summary>
        /// 向客户发送通知
        /// </summary>
        /// 陆凯提供的接口
        [Description("http://172.30.10.48:19700/sms/msg")]
        SendMsgToClient,

#elif TEST
        [Description("http://erp80.foric.b1b.cn/PvWsOrderApi")]
        OrderApi = 3,
        [Description("http://api0.for-ic.net/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToXDT = 4,
        [Description("http://api0.for-ic.net/Declaration/DeclarationNotice")]
        CustomApply = 5,
        [Description("http://api0.for-ic.net/SZWarehouse/OutStock")]
        XdtSZShiped=6,
        [Description("http://api0.for-ic.net/SZWarehouse/ReceiptConfirmed")]
        XdtSZSZReceiptConfirm=7,
        [Description("http://api0.for-ic.net/Finance/InsertInvoiceNoticeFiles")]
        InsertInvoiceNoticeFiles=8,
        [Description("http://api0.for-ic.net/Finance/DeleteInvoiceNoticeFiles")]
        DeleteInvoiceNoticeFiles = 9,
         /// <summary>
        /// 通知变更
        /// </summary>
        /// <remarks>
        /// 董建接口
        /// </remarks>
        [Description("http://erp80.foric.b1b.cn/PvWsOrderApi/Sorted/SubmitSorted")]
        NoticeDelivery,
        /// <summary>
        /// 产品变更通知(品牌,产地的变更)
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://api0.for-ic.net/HKWarehouse/ItemChangeNotice")]
        ItemChangeNotice,
        /// <summary>
        /// 拆项变更通知
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://api0.for-ic.net/HKWarehouse/SplitChangeNotice")]
        SplitChangeNotice,

        /// <summary>
        /// 同步记账
        /// </summary>
        /// <remarks>
        /// 荣检接口
        /// </remarks>
        [Description("http://api0.for-ic.net/Finance/InsertHKWarehouseFee")]
        ForReceivables,
        [Description("http://api0.wl.net.cn/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToHY,

        /// <summary>
        /// 向客户发送通知
        /// </summary>
        /// 陆凯提供的接口
        [Description("http://172.30.10.48:19700/sms/msg")]
        SendMsgToClient,

#else
        [Description("http://erp8.wapi.for-ic.net:60077/PvWsOrderApi")]
        OrderApi = 3,
        [Description("http://foricapi.wapi.for-ic.net:60077/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToXDT = 4,
        [Description("http://foricapi.wapi.for-ic.net:60077/Declaration/DeclarationNotice")]
        CustomApply = 5,
        [Description("http://foricapi.wapi.for-ic.net:60077/SZWarehouse/OutStock")]
        XdtSZShiped=6,
        [Description("http://foricapi.wapi.for-ic.net:60077/SZWarehouse/ReceiptConfirmed")]
        XdtSZSZReceiptConfirm=7,
        [Description("http://foricapi.wapi.for-ic.net:60077/Finance/InsertInvoiceNoticeFiles")]
        InsertInvoiceNoticeFiles=8,
        [Description("http://foricapi.wapi.for-ic.net:60077/Finance/DeleteInvoiceNoticeFiles")]
        DeleteInvoiceNoticeFiles = 9,
        /// <summary>
        /// 通知变更
        /// </summary>
        /// <remarks>
        /// 董建接口
        /// </remarks>
        [Description("http://erp8.wapi.for-ic.net:60077/PvWsOrderApi/Sorted/SubmitSorted")]
        NoticeDelivery,
        /// <summary>
        /// 产品变更通知(品牌,产地的变更)
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://foricapi.wapi.for-ic.net:60077/HKWarehouse/ItemChangeNotice")]
        ItemChangeNotice,
        /// <summary>
        /// 拆项变更通知
        /// </summary>
        /// 荣捡提供的接口
        [Description("http://foricapi.wapi.for-ic.net:60077/HKWarehouse/SplitChangeNotice")]
        SplitChangeNotice,

        /// <summary>
        /// 同步记账
        /// </summary>
        /// <remarks>
        /// 荣检接口
        /// </remarks>
        [Description("http://foricapi.wapi.for-ic.net:60077/Finance/InsertHKWarehouseFee")]
        ForReceivables,
        [Description("http://api.wl.net.cn/HKWarehouse/DeliveryNotice")]
        ArrivalInfoToHY,

        /// <summary>
        /// 向客户发送通知
        /// </summary>
        /// 陆凯提供的接口
        [Description("http://172.30.10.92:19700/sms/msg")]
        SendMsgToClient,
#endif


    }
}
