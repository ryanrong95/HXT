using System;
using Needs.Utils.Descriptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 香港申报状态
    /// </summary>
    public enum HKDeclareStatus
    {
        /// <summary>
        /// 未申报
        /// </summary>
        [Description("未申报")]
        UnDeclare = 0,

        /// <summary>
        /// 已申报
        /// </summary>
        [Description("已申报")]
        Declared = 1,
    }

    /// <summary>
    /// 截单状态
    /// </summary>
    public enum CutStatus
    {
        /// <summary>
        /// 未截单
        /// </summary>
        [Description("未截单")]
        UnCutting = 0,

        /// <summary>
        /// 已截单
        /// </summary>
        [Description("已截单")]
        Cutted = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 2
    }


    /// <summary>
    /// 订单特殊类型
    /// </summary>
    public enum OrderSpecialType : int
    {
        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 1,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 2,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 3,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 4,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 5,

        /// <summary>
        /// 原产地加征
        /// </summary>
        [Description("加征")]
        OriginATRate = 6,

        /// <summary>
        /// 敏感产地
        /// </summary>
        [Description("敏感产地")]
        SenOrigin = 7,
        
    }

    /// <summary>
    /// 运输批次-运输类型
    /// </summary>
    public enum VoyageType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Error = 0,

        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Normal = 1,

        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 2,
    }
}
