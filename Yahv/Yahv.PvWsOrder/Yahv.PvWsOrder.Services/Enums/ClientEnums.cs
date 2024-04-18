using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 客户类型
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// 自有公司
        /// </summary>
        [Description("自有公司")]
        Internal = 1,

        /// <summary>
        /// 外单客户
        /// </summary>
        [Description("外单客户")]
        External = 2
    }

    /// <summary>
    /// 客户等级
    /// </summary>
    public enum ClientRank
    {
        /// <summary>
        /// 一级
        /// </summary>
        [Description("一级")]
        ClassOne = 1,

        /// <summary>
        /// 二级
        /// </summary>
        [Description("二级")]
        ClassTwo = 2,

        /// <summary>
        /// 三级
        /// </summary>
        [Description("三级")]
        ClassThree = 3,

        /// <summary>
        /// 四级
        /// </summary>
        [Description("四级")]
        ClassFour = 4,

        /// <summary>
        /// 五级
        /// </summary>
        [Description("五级")]
        ClassFive = 5,

        /// <summary>
        /// 六级
        /// </summary>
        [Description("六级")]
        ClassSix = 6
    }

    /// <summary>
    /// 客户状态
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// 未完善
        /// </summary>
        [Description("未完善")]
        Auditing = 1,

        /// <summary>
        /// 已完善
        /// </summary>
        [Description("已完善")]
        Confirmed = 2,
    }

    /// <summary>
    /// 客户的客服类型
    /// </summary>
    public enum ClientAdminType
    {
        /// <summary>
        /// 业务经理
        /// </summary>
        [Description("业务经理")]
        ServiceManager = 1,

        /// <summary>
        /// 跟单员
        /// </summary>
        [Description("跟单员")]
        Merchandiser = 2
    }

    /// <summary>
    /// 发票日志状态
    /// </summary>
    public enum ClientInvoiceStatus
    {
        /// <summary>
        /// 未标注
        /// </summary>
        UnMarked = 0,

        /// <summary>
        /// 已标注
        /// </summary>
        Marked = 1
    }

    /// <summary>
    /// 客户发票交付方式
    /// </summary>
    public enum ClientInvoiceDeliveryType
    {
        /// <summary>
        /// 邮寄
        /// </summary>
        [Description("邮寄")]
        SendByPost = 1,

        /// <summary>
        /// 随货同行
        /// </summary>
        [Description("随货同行")]
        FollowUpGoods = 2,
    }

    /// <summary>
    /// 注册申请状态
    /// </summary>
    public enum HandleStatus
    {
        [Description("待处理")]
        Pending = 0,

        [Description("已处理")]
        Processed = 1,
    }
   
}
