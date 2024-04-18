using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    ///审批任务类型CrmPlus
    /// </summary>
    public enum ApplyTaskType
    {
        /// <summary>
        /// 客户注册
        /// </summary>
        [Description("客户注册")]
        ClientRegist = 1,

        /// <summary>
        /// 供应商注册
        /// </summary>
        [Description("供应商注册")]
        SupplierRegist = 2,

        // <summary>
        /// 供应商关联关系
        /// </summary>
        [Description("关联关系")]
        SupplierBusinessRelation = 3,

        // <summary>
        /// 客户关联关系
        /// </summary>
        [Description("关联关系")]
        ClientBusinessRelation = 4,

        // <summary>
        /// 供应商特色型号
        /// </summary>
        [Description("特色型号")]
        SupplierSpecials = 5,

        /// <summary>
        /// 客户地址
        /// </summary>
        [Description("客户地址")]
        ClientAddress = 6,

        // <summary>
        /// 供应商保护申请
        /// </summary>
        [Description("供应商保护申请")]
        SupplierProtected = 7,


        /// <summary>
        /// 客户申请保护
        /// </summary>
        [Description("客户申请保护")]
        ClientProtected = 8,

        [Description("公海认领")]
        ClientPublic = 9,

        [Description("送样申请")]
        ClientSample = 10,

        [Description("销售机会状态")]
        ClientProjectStatus = 11,

        [Description("贸易客户申请")]
        ClientTradingApply = 12,

        [Description("代理线客户申请")]
        ClientAgentApply = 13,

        /// <summary>
        /// 客户账期
        /// </summary>
        [Description("客户账期")]
        ClientCredit = 14,

        /// <summary>
        /// 客户授信
        /// </summary>
        [Description("客户授信")]
        ClientCreditFlow = 15,

        /// <summary>
        /// 客户使用激活
        /// </summary>
        [Description("客户激活")]
        ClientActive = 16,

        [Description("客户结对子")]
        ClientPaires = 17,

        [Description("供应商激活")]
        SupplierActive = 18,

    }
}
