
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 帐号类型
    /// </summary>
    public enum BookAccountType
    {
        /// <summary>
        /// 收款人
        /// </summary>
        [Description("收款人")]
        Payee = 1,
        /// <summary>
        /// 付款人
        /// </summary>
        [Description("付款人")]
        Payer = 2
    }
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum BookAccountMethord
    {
        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        Wx = 1,
        /// <summary>
        /// 付款人
        /// </summary>
        [Description("支付宝")]
        Zfb = 2,
        /// <summary>
        /// 银行转账
        /// </summary>
        [Description("银行转账")]
        Bank = 3,
    }


    public enum Nature
    {
        /// <summary>
        /// 个人
        /// </summary>
        [Description("个人")]
        Person = 0,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 1,

    }

}
