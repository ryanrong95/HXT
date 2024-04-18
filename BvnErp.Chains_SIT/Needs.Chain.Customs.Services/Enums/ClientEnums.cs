using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
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
    /// 代仓储客户类型
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("无")]
        Unknown = 0,
        /// <summary>
        /// 香港公司
        /// </summary>
        [Description("香港公司")]
        HKCompany = 1,
        /// <summary>
        /// 国内
        /// </summary>
        [Description("国内")]
        Domestic = 2,
      
        /// <summary>
        /// 个人
        /// </summary>
        [Description("个人")]
        Person=3
    }

    [Flags]
    public enum ServiceType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Customs = 1,
        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        Warehouse = 2,


        [Description("双服务")]
        Both =3


    }

    /// <summary>
    /// 客户性质
    /// </summary>
    public enum ClientNature
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 终端
        /// </summary>
        [Description("终端")]
        terminal = 1,
        /// <summary>
        /// 贸易商
        /// </summary>
        [Description("贸易商")]
        Trade = 2,
        /// <summary>
        /// OEM
        /// </summary>
        [Description("OEM")]
        OEM=3




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
        ClassSix = 6,

        /// <summary>
        //七级
        /// </summary>
        [Description("七级")]
        ClassSeven = 7,

        /// <summary>
        //八级
        /// </summary>
        [Description("八级")]
        ClassEight = 8,
        /// <summary>
        //七级
        /// </summary>
        [Description("九级")]
        ClassNine = 9,
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
        /// 待审核
        /// </summary>
        [Description("待风控审核")]
        Verifying = 3,

        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        WaitingApproval = 4,

        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Returned = 5,


        /// <summary>
        /// 已完善
        /// </summary>
        [Description("已完善")]
        Confirmed = 2,



    }

    /// <summary>
    /// 客户风控审核状态
    /// </summary>
    public enum ClientControlStatus
    {

        /// <summary>
        /// 待风控审核
        /// </summary>
        [Description("待风控审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,
        /// <summary>
        /// 审核不通过
        /// </summary>
        [Description("审核未通过")]
        RefuseAudit = 3,



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
        Merchandiser = 2,
        /// <summary>
        /// 引荐人
        /// </summary>
        [Description("引荐人")]
        Referrer = 3,

        /// <summary>
        /// 代仓储业务经理
        /// </summary>
        [Description("代仓储业务经理")]
        StorageServiceManager = 4,

        /// <summary>
        /// 代仓储跟单员
        /// </summary>
        [Description("代仓储跟单员")]
        StorageMerchandiser = 5,
        /// <summary>
        /// 代仓储引荐人
        /// </summary>
        [Description("代仓储引荐人")]
        StorageReferrer = 6,
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
    /// 注册申请状态
    /// </summary>
    public enum HandleStatus
    {
        [Description("待处理")]
        Pending = 0,

        [Description("已处理")]
        Processed = 1,
    }


    /// <summary>
    /// 供应商等级
    /// </summary>
    public enum SupplierGrade
    {

        [Description("等级一")]
        First = 1,
        [Description("等级二")]
        Second = 2,
        [Description("等级三")]
        Third = 3,
        //[Description("等级四")]
        //Fourth = 4,
        //[Description("等级五")]
        //Fifth = 5,
        //[Description("等级六")]
        //Sixth = 6,
        //[Description("等级七")]
        //Seventh = 7,
        //[Description("等级八")]
        //Eighth = 8,
        //[Description("等级九")]
        //Ninth = 9
    }

    /// <summary>
    /// 服务协议上传状态
    /// </summary>
    public enum SAUploadStatus
    {
        /// <summary>
        /// 未上交
        /// </summary>
        [Description("未上交")]
        UnUpload = 0,

        /// <summary>
        /// 已上交
        /// </summary>
        [Description("已上交")]
        Uploaded = 1,
    }

}