using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.SsoUsers
{

    /// <summary>
    /// 用户令牌状态 字段说明：100 未使用 200 已使用
    /// </summary>
    public enum UserTokenStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        NonUse = 100,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,
    }

    /// <summary>
    /// 用户令牌类型 1 激活  2 找回密码 3 用户登入
    /// </summary>
    public enum UserTokenType
    {
        /// <summary>
        /// 激活 (基本)
        /// </summary>
        Activing = 100,
        /// <summary>
        /// 找回密码
        /// </summary>
        Password = 200,
        /// <summary>
        /// 用户登入
        /// </summary>
        UserLogin = 300,
        /// <summary>
        /// 邮箱确认
        /// </summary>
        EmailActiving = 110
    }

    /// <summary>
    /// 用户状态  待激活 100 正常 200  删除 400
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 待激活
        /// </summary>
        [Naming("待激活")]
        Activing = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Naming("正常")]
        Normal = 200,
        /// <summary>
        /// 删除
        /// </summary>
        [Naming("删除")]
        Deleted = 400
    }

    /// <summary>
    /// 账户类型 1 现金余额 2 信用
    /// </summary>
    public enum UserAccountType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Naming("Unknown")]
        Unknown = 0,
        /// <summary>
        /// 现金余额
        /// </summary>
        [Naming("Cash")]
        Cash = 1,
        /// <summary>
        /// 信用
        /// </summary>
        [Naming("Credit")]
        Credit = 2
    }

    /// <summary>
    /// 账户收入来源
    /// </summary>
    public enum InputSource
    {
        /// <summary>
        /// 现金充值
        /// </summary>
        Cach = 1,
        /// <summary>
        /// 后台充值
        /// </summary>
        BackCach = 2,
        /// <summary>
        /// 信用申请
        /// </summary>
        Credit = 3,
        /// <summary>
        /// 信用提额申请
        /// </summary>
        AddCredit = 4,
        /// <summary>
        /// 后台信用
        /// </summary>
        BackCredit = 5,
        /// <summary>
        /// 后台信用提额
        /// </summary>
        BackAddCredit = 6,
        /// <summary>
        /// 信用还款账单
        /// </summary>
        CreditBill = 7,
        /// <summary>
        /// 信用还款
        /// </summary>
        CachToCredit = 8
    }

    /// <summary>
    /// 用户发票类型（1 普票、2 增票）
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 普票
        /// </summary>
        [Naming("普通发票")]
        Invoice = 1,
        /// <summary>
        /// 增票
        /// </summary>
        [Naming("增值税专用发票")]
        VAT = 2
    }

    /// <summary>
    /// 申请来源
    /// </summary>
    public enum ApplySource
    {
        /// <summary>
        /// 信用
        /// </summary>
        [Naming("信用")]
        Credit = 1,
        /// <summary>
        /// 信用提额
        /// </summary>
        [Naming("信用提额")]
        AddCredit = 2,
        /// <summary>
        /// 换货
        /// </summary>
        [Naming("换货")]
        Replace = 3,
        /// <summary>
        /// 退货
        /// </summary>
        [Naming("退货")]
        Return = 4,
    }
    /// <summary>
    /// 用户注册类型
    /// </summary>
    public enum UserRegisterType
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Naming("Email")]
        Email = 1,
        /// <summary>
        /// 手机号
        /// </summary>
        [Naming("Mobile")]
        Mobile,
        /// <summary>
        /// QQ
        /// </summary>
        [Naming("QQ")]
        QQ,
        /// <summary>
        /// 微信
        /// </summary>
        [Naming("WX")]
        WX,
        /// <summary>
        /// ICQ
        /// </summary>
        [Naming("ICQ")]
        ICQ

    }


}
