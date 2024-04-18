using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.DappApi.Services.Enums
{
    /// <summary>
    /// 业务类型
    /// </summary>
    public enum Conduct
    {
        /// <summary>
        /// 代仓储, 仓储服务：入库为代入库, 出库为代出库
        /// </summary>
        [Description("代仓储")]
        Warehousings = 1,

        /// <summary>
        /// 贸易, 入库为采购;出库为销售
        /// </summary>
        [Description("贸易")]
        Tradings = 2,

        /// <summary>
        /// 售后, 售后服务：入库为退货, 出库为补货, 退换货就以上两个过程
        /// </summary>
        [Description("售后")]
        PostSale = 3,
    }

    /// <summary>
    /// 地址类型
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// 收货地址
        /// </summary>
        [Description("")]
        Consignee = 1,

        /// <summary>
        /// 交货地址
        /// </summary>
        [Description("交货地址")]
        Consignor = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 3,
    }

    /// <summary>
    /// 证件类型
    /// </summary>
    public enum IDType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 0,

        /// <summary>
        /// 身份证
        /// </summary>
        [Description("身份证")]
        IDCard = 1,        
    }

    /// <summary>
    /// 报告类型
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Inbound = 1,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 2,

        /// <summary>
        /// 即入即出
        /// </summary>
        [Description("即入即出")]
        InAndOut = 3,

        /// <summary>
        /// 盘点, 在库行为
        /// </summary>
        [Description("盘点")]
        Stocktaking = 101,

        /// <summary>
        /// 检测, 在库行为
        /// </summary>
        [Description("检测")]
        Check = 102,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 200
    }

    /// <summary>
    /// 货运类型
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        /// 未知, 不限
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        PickUp = 1,

        /// <summary>
        /// 快递, 默认
        /// </summary>
        [Description("快递")]
        Express = 2,

        /// <summary>
        /// 送货上门
        /// </summary>
        [Description("送货上门")]
        Dtd = 3
    }

    /// <summary>
    /// 盘点类型
    /// </summary>
    public enum StocktakingType
    {
        /// <summary>
        /// 按个
        /// </summary>
        [Description("按个")]
        Single = 1,

        /// <summary>
        /// 最小包装
        /// </summary>
        [Description("最小包装")]
        MinPackage = 2,
    }

    /// <summary>
    /// 库存类型
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// 库存库
        /// </summary>
        [Description("库存库")]
        Store = 1,

        /// <summary>
        /// 流水库
        /// </summary>
        [Description("流水库")]
        Flow = 2,

        /// <summary>
        /// 暂存库
        /// </summary>
        [Description("暂存库")]
        Park = 3,

        /// <summary>
        /// 在途库, 订购期间物资, 在途物资
        /// </summary>
        [Description("在途库")]
        Ordering = 4,

        /// <summary>
        /// 报废库
        /// </summary>
        [Description("报废库")]
        Scrap = 100,
    }

    /// <summary>
    /// 账目来源
    /// </summary>
    public enum AccountSource
    {
        /// <summary>
        /// 库房, 库管
        /// </summary>
        [Description("库房")]
        Keeper = 1,

        /// <summary>
        /// 跟单, 客服
        /// </summary>
        [Description("跟单")]
        Tracker = 2
    }

    /// <summary>
    /// 账目账户类型
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 现金, 所有未知类型账目默认算作现金账
        /// </summary>
        [Description("现金")]
        Cash = 1,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 2
    }

    /// <summary>
    /// 运费付款人
    /// </summary>
    public enum FreightPayer
    {
        /// <summary>
        /// 发件人,寄付
        /// </summary>
        [Description("寄付")]
        Sender = 1,

        /// <summary>
        /// 收件人, 到付
        /// </summary>
        [Description("到付")]
        Recipient = 2,

        /// <summary>
        /// 第三方月结，需要另外的月结账户
        /// </summary>
        [Description("第三方月结")]
        ThirdParty = 3,

        /// <summary>
        /// 月结, 芯达通月结
        /// </summary>
        [Description("月结")]
        Monthly = 4,
    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 外观拍照
        /// </summary>
        [Description("外观拍照")]
        Exterior = 1,

        /// <summary>
        /// 单据拍照
        /// </summary>
        [Description("单据拍照")]
        Form = 2,

        /// <summary>
        /// 标签
        /// </summary>
        [Description("标签")]
        Label = 3,

        /// <summary>
        /// 出库文件
        /// </summary>
        [Description("出库送货文件")]
        OutDelivery = 4,

        /// <summary>
        /// 入库文件
        /// </summary>
        [Description("入库送货文件")]
        InDelivery = 5,

        /// <summary>
        /// 入库自提使用
        /// </summary>
        [Description("提货文件")]
        Taking = 6,

        /// <summary>
        /// 账单
        /// </summary>
        [Description("账单")]
        Bill = 7,

        /// <summary>
        /// 司机签字文件
        /// </summary>
        [Description("司机签字")]
        DriverSign = 8,

        /// <summary>
        /// 客户签字文件
        /// </summary>
        [Description("客户签字")]
        CustomSign = 9,

        /// <summary>
        /// 回签单文件
        /// </summary>
        [Description("回签单文件")]
        TrackingCode = 10
    }

    /// <summary>
    /// 接受任务人类型
    /// </summary>
    public enum TakerType
    {
        /// <summary>
        /// 司机
        /// </summary>
        [Description("司机")]
        Driver = 1,

        /// <summary>
        /// 拿货人， 一般拿货人
        /// </summary>
        [Description("拿货人")]
        Taker = 2,
    }

    /// <summary>
    /// 快递
    /// </summary>
    public enum Express
    {
        /// <summary>
        /// 顺丰
        /// </summary>
        [Description("顺丰")]
        SF = 1,

        /// <summary>
        /// 跨越
        /// </summary>
        [Description("跨越")]
        KY = 2,

        /// <summary>
        /// 德邦
        /// </summary>
        [Description("德邦")]
        DB = 3,
    }

    /// <summary>
    /// 快递方式-顺丰
    /// </summary>
    public enum ExpressMethodSF
    {
        [Description("顺丰标快")]
        顺丰标快 = 1,

        [Description("顺丰标快-陆运")]
        顺丰标快_陆运 = 2,

        [Description("顺丰次晨")]
        顺丰次晨 = 5,

        [Description("顺丰即日")]
        顺丰即日 = 6,

        [Description("国际特惠-商家代理")]
        国际特惠_商家代理 = 27,

        [Description("便利箱产品")]
        便利箱产品 = 31,

        [Description("岛内件-80CM")]
        岛内件_80CM = 33,

        [Description("高铁站配")]
        高铁站配 = 53,

        [Description("顺丰特惠D")]
        顺丰特惠D = 111,

        [Description("顺丰空配")]
        顺丰空配 = 112,

        [Description("重货包裹")]
        重货包裹 = 154,

        [Description("标准零担")]
        标准零担 = 155,

        [Description("极速包裹")]
        极速包裹 = 199,

        [Description("冷运特惠")]
        冷运特惠 = 201,

        [Description("医药快运")]
        医药快运 = 203,

        [Description("高铁专送")]
        高铁专送 = 209,

        [Description("大票直送")]
        大票直送 = 215,

        [Description("冷运")]
        冷运 = 221,

        [Description("精温专递")]
        精温专递 = 229,

        [Description("陆运包裹")]
        陆运包裹 = 231,

        [Description("精温专递-样本陆")]
        精温专递_样本陆 = 233,

        [Description("商务标快")]
        商务标快 = 234,
    }

    /// <summary>
    /// 快递方式-跨越
    /// </summary>
    public enum ExpressMethodKY
    {
        [Description("当天达")]
        当天达 = 10,

        [Description("次日达")]
        次日达 = 20,

        [Description("隔日达")]
        隔日达 = 30,

        [Description("陆运件")]
        陆运件 = 40,

        [Description("同城次日")]
        同城次日 = 50,

        [Description("同城即日")]
        同城即日 = 70,

        [Description("省内次日")]
        省内次日 = 160,

        [Description("省内即日")]
        省内即日 = 170,
    }

    /// <summary>
    /// 快递方式-德邦
    /// </summary>
    public enum ExpressMethodDB
    {
        [Description("标准快递")]
        标准快递 = 1,

        [Description("360特惠件")]
        _360特惠件 = 2,

        [Description("电商尊享")]
        电商尊享 = 3,

        [Description("特准快件")]
        特准快件 = 4,

        [Description("3.60特重件")]
        _3_60特重件 = 5,

        [Description("重包入户")]
        重包入户 = 6,

        [Description("同城件")]
        同城件 = 7,
    }
}
