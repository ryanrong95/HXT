﻿using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 用于生成主键，对应表:PrimaryKeys
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 代理订单
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("Order", PKeySigner.Mode.Time, 6)]
        Order = 10000,

        /// <summary>
        /// 代理订单项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OrderItem", PKeySigner.Mode.Time, 10)]
        OrderItem = 10001,

        /// <summary>
        /// 代理订单附件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OrderFile", PKeySigner.Mode.Time, 6)]
        OrderFile = 10002,

        /// <summary>
        /// 代理订单附加费用
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OrderFee", PKeySigner.Mode.Time, 6)]
        OrderPremium = 10003,

        /// <summary>
        /// 代理订单日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OrderLog", PKeySigner.Mode.Time, 10)]
        OrderLog = 10004,

        /// <summary>
        /// 代理订单日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OrderTrace", PKeySigner.Mode.Time, 10)]
        OrderTrace = 10005,

        /// <summary>
        /// 报关通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("CN", PKeySigner.Mode.Time, 6)]
        Notice = 20000,

        /// <summary>
        /// 报关通知项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("CNI", PKeySigner.Mode.Normal, 4)]
        NoticeItem = 20001,

        /// <summary>
        /// 会员条款
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("AI", PKeySigner.Mode.Normal, 6)]
        AgreementItem = 30001,

        /// <summary>
        /// 入库通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("EN", PKeySigner.Mode.Time)]
        EntryNotice = 50000,

        /// <summary>
        /// 入库通知项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("ENI", PKeySigner.Mode.Time)]
        EntryNoticeItem = 50001,

        /// <summary>
        /// 装箱结果
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("Sorting", PKeySigner.Mode.Time)]
        Sorting = 50002,

        /// <summary>
        /// 出库通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("N", PKeySigner.Mode.Time, 3)]
        ExitNotice = 50003,

        /// <summary>
        /// 出库通知项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("EXNItem", PKeySigner.Mode.Time)]
        ExitNoticeItem = 50004,

        /// <summary>
        /// 暂存记录
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("TMP", PKeySigner.Mode.Time)]
        Temporary = 50005,

        /// <summary>
        /// 暂存文件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("TMPFile", PKeySigner.Mode.Time)]
        TemporaryFile = 50006,

        /// <summary>
        /// 暂存日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("TMPLog", PKeySigner.Mode.Time)]
        TemporaryLog = 50007,

        /// <summary>
        /// 国际快递
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("Exp", PKeySigner.Mode.Time)]
        Expressage = 50008,

        /// <summary>
        /// 提货人
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DLPicker", PKeySigner.Mode.Time)]
        DeliveryPicker = 50009,

        /// <summary>
        /// 装箱结果
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PKing", PKeySigner.Mode.Time)]
        Packing = 50010,

        /// <summary>
        /// 装箱结果项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PKingItem", PKeySigner.Mode.Time)]
        PackingItem = 50011,

        /// <summary>
        /// 快递公司
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("ExpCompany", PKeySigner.Mode.Time)]
        ExpressCompany = 50012,

        /// <summary>
        /// 出库通知记录
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("EXNLog", PKeySigner.Mode.Time, 6)]
        ExitNoticeLog = 50013,

        /// <summary>
        /// 装箱结果项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("EXNFile", PKeySigner.Mode.Time)]
        ExitNoticeFile = 50014,

        /// <summary>
        /// 出库交货
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("EXDeliver", PKeySigner.Mode.Time)]
        ExitDeliver = 50015,

        /// <summary>
        /// 库房费用附件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OWPFile", PKeySigner.Mode.Time)]
        OrderWhesPremiumFile = 50016,

        /// <summary>
        /// 库房费用附件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OWPLog", PKeySigner.Mode.Time, 6)]
        OrderWhesPremiumLog = 50017,

        /// <summary>
        /// 库房费用
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OWP", PKeySigner.Mode.Time)]
        OrderWhesPremium = 50018,

        /// <summary>
        /// 提货通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DLNotice", PKeySigner.Mode.Time)]
        DeliveryNotice = 50019,

        /// <summary>
        /// 提货通知日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DLNLog", PKeySigner.Mode.Time)]
        DeliveryNoticeLog = 50020,

        /// <summary>
        /// 提货信息
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DLConsignee", PKeySigner.Mode.Time)]
        DeliveryConsignee = 50022,

        /// <summary>
        /// 库存库
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("SStorage", PKeySigner.Mode.Time)]
        StoreStorage = 60001,

        /// <summary>
        /// 付汇申请
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PEA", PKeySigner.Mode.Time, 3)]
        PayExchangeApply = 90001,

        /// <summary>
        /// 付汇申请文件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PEAFile", PKeySigner.Mode.Time)]
        PayExchangeApplyFile = 90002,

        /// <summary>
        /// 付汇申请项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PEAItem", PKeySigner.Mode.Time)]
        PayExchangeApplyItem = 90003,

        /// <summary>
        /// 付汇申请日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PEALog", PKeySigner.Mode.Time, 6)]
        PayExchangeApplyLog = 90004,

        /// <summary>
        /// 开票通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("IVNT", PKeySigner.Mode.Time, 6)]
        InvoiceNotice = 90005,

        /// <summary>
        /// 开票通知明细项
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("IVNTI", PKeySigner.Mode.Time, 6)]
        InvoiceNoticeItem = 90006,

        /// <summary>
        /// 开票通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("SwapN", PKeySigner.Mode.Time, 6)]
        SwapNotice = 90007,

        /// <summary>
        /// 开票通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("SwapNI", PKeySigner.Mode.Time, 6)]
        SwapNoticeItem = 90008,

        /// <summary>
        /// 付款申请
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PayApply", PKeySigner.Mode.Time, 6)]
        PaymentApply = 90009,

        /// <summary>
        /// 付款申请
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PALog", PKeySigner.Mode.Time, 6)]
        PaymentApplyLog = 90010,

        /// <summary>
        /// 付款通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PayNotice", PKeySigner.Mode.Time, 6)]
        PaymentNotice = 90011,

        /// <summary>
        /// 付款通知明细
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PayNoItem", PKeySigner.Mode.Time, 6)]
        PaymentNoticeItem = 90012,

        /// <summary>
        /// 付款通知附件
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PayNoFile", PKeySigner.Mode.Time, 6)]
        PaymentNoticeFile = 90013,

        /// <summary>
        /// 客户登录账号
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("User", PKeySigner.Mode.Normal)]
        User = 100000,

        /// <summary>
        /// 注册申请
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("SA", PKeySigner.Mode.Time, 6)]
        ServiceApply = 100001,

        /// <summary>
        /// 报关通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DC", PKeySigner.Mode.Time, 10)]
        DeclareNotice = 100200,

        /// <summary>
        /// 报关通知
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("DCL", PKeySigner.Mode.Time, 10)]
        DeclareNoticeLog = 100201,

        /// <summary>
        /// 报关单
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("CDO", PKeySigner.Mode.Time, 7)]
        DecHead = 80001,

        /// <summary>
        /// 报关单-报文轨迹ID
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("CDORM", PKeySigner.Mode.Time, 6)]
        DecHeadTrace = 80002,

        /// <summary>
        /// 报关单-合同
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("00000004SW", PKeySigner.Mode.Time, 10)]
        EdocContact = 81004,

        /// <summary>
        /// 报关单-发票
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("00000001SW", PKeySigner.Mode.Time, 10)]
        EdocInvoice = 81001,

        /// <summary>
        /// 报关单-装箱单
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("00000002SW", PKeySigner.Mode.Time, 10)]
        EdocPacking = 81002,

        /// <summary>
        /// 舱单ID
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("MFT", PKeySigner.Mode.Time, 6)]
        ManifestHead = 82001,

        /// <summary>
        /// 舱单-报文轨迹ID
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("MFORM", PKeySigner.Mode.Time, 6)]
        ManifestTrace = 82002,

        /// <summary>
        /// 舱单/运单-报文轨迹ID
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("MFTRM", PKeySigner.Mode.Time, 6)]
        ManifestConsignmentTrace = 82003,

        /// <summary>
        /// 运输批次-附件ID
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("VoyageFile", PKeySigner.Mode.Time, 6)]
        VoyageFile = 82004,

        /// <summary>
        /// 财务金库
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("FinVault", PKeySigner.Mode.Time, 6)]
        FinanceVault = 11000,

        /// <summary>
        /// 金库账户
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("FinAccount", PKeySigner.Mode.Time, 6)]
        FinanceAccount = 11001,

        /// <summary>
        /// 财务收款
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("FinReceipt", PKeySigner.Mode.Time, 6)]
        FinanceReceipt = 11002,

        /// <summary>
        /// 订单收款
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("OReceipt", PKeySigner.Mode.Time, 6)]
        OrderReceipt = 11003,

        /// <summary>
        /// 财务付款
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("FinPayment", PKeySigner.Mode.Time, 6)]
        FinancePayment = 11004,

        /// <summary>
        /// 账户流水
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("FinAcFlow", PKeySigner.Mode.Time, 6)]
        FinanceAccountFlow = 11005,

        /// <summary>
        /// 产品归类变更日志
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("PCCLog", PKeySigner.Mode.Time, 10)]
        ProductClassifyChangeLog = 12000,

        /// <summary>
        /// 角色
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("Role", PKeySigner.Mode.Time, 6)]
        Role = 1000,

        /// <summary>
        /// 管理员角色
        /// </summary>
        [Repository(typeof(ScCustomsReponsitory))]
        [PKey("AR", PKeySigner.Mode.Time, 6)]
        AdminRole = 2000,
    }
}
