using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单管控类型、订单挂起原因
    /// 备注：因这个将来需要对外作为产品归类结果的代码提供出去，所以设计成100、200...
    /// </summary>
    public enum OrderControlType
    {
        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 100,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 200,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginCertificate = 300,

        /// <summary>
        /// 超出垫款上限
        /// </summary>
        [Description("超出垫款上限")]
        ExceedLimit = 400,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        ClassifyAnomaly = 500,

        /// <summary>
        /// 分拣异常
        /// </summary>
        [Description("分拣异常")]
        SortingAbnomaly = 600,

        /// <summary>
        /// 抽检异常
        /// </summary>
        [Description("抽检异常")]
        CheckingAbnomaly = 700,

        /// <summary>
        /// 产地变更
        /// </summary>
        [Description("产地变更")]
        OriginChange = 800,

        /// <summary>
        /// 删除型号
        /// </summary>
        [Description("删除型号")]
        DeleteModel = 900,

        /// <summary>
        /// 修改数量
        /// </summary>
        [Description("修改数量")]
        ChangeQuantity = 1000,
    }
}
