using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum OrderChangeType
    {
        /// <summary>
        /// 报关完成，上传缴费流水，关税增值税不一样，引起的变更
        /// </summary>
        [Description("税费变更")]
        TaxChange = 1,

        /// <summary>
        /// 重新归类，引起的变更，品牌，型号等(重新归类，引起的关税增值税的变化也属于产品变更)
        /// </summary>
        [Description("产品变更")]
        ProductChange = 2,

        /// <summary>
        /// 跟单确认到货的时候，引起的变更，有以下四种情况
        /// 1、到货修改了订单的数量
        /// 2、到货删除了原来订单的型号
        /// 3、到货新增了型号
        /// 4、需要重新归类(型号变更，品牌变更，产地变更,批次号变更)
        /// </summary>
        [Description("到货变更")]
        ArrivalChange = 3,
    }


    /// <summary>
    /// 订单产品变更类型
    /// </summary>
    public enum OrderItemChangeType
    {
        [Description("")]
        None = 0,

        /// <summary>
        /// 产地变更
        /// </summary>
        [Description("产地变更")]
        OriginChange = 1,

        /// <summary>
        /// 品牌变更
        /// </summary>
        [Description("品牌变更")]
        BrandChange = 2,

        /// <summary>
        /// 型号变更
        /// </summary>
        [Description("型号变更")]
        ProductModelChange = 3,

        /// <summary>
        /// 批次变更
        /// </summary>
        [Description("批次变更")]
        BatchChange = 4,

        /// <summary>
        /// 海关编码变更
        /// </summary>
        [Description("海关编码变更")]
        HSCodeChange = 5,

        /// <summary>
        /// 报关品名变更
        /// </summary>
        [Description("报关品名变更")]
        TariffNameChange = 6,
    }


    /// <summary>
    /// 订单变更状态处理状态
    /// </summary>
    public enum ProcessState
    {

        [Description("未处理")]
        UnProcess = 1,
        [Description("待跟单处理")]
        Processing = 2,
        [Description("已处理")]
        Processed = 3,
    }

    /// <summary>
    /// 引发来源
    /// </summary>
    public enum TriggerSource
    {
        /// <summary>
        /// 香港库房(若香港库房再有细分，可修改该枚举值描述)
        /// </summary>
        [Description("香港库房")]
        HKPacking = 1,

        /// <summary>
        /// 对单人员
        /// </summary>
        [Description("对单人员")]
        CheckDecListMan = 2,
    }

}
