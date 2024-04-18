using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
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
