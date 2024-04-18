using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    /// <summary>
    /// 登录页面视图
    /// </summary>
    public class LoginViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RemberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// 会员信息
    /// </summary>
    public class BasicInfoViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public  string CompanyName { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { get; set; }

        /// <summary>
        /// 公司法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 营业执照
        /// </summary>
        public FileModel BLFile { get; set; }

        /// <summary>
        /// 发票
        /// </summary>
        public InvoiceViewModel Invoice { get; set; }

        /// <summary>
        /// 是否编辑发票
        /// </summary>
        public  bool IsEditInvoice { get; set; }
    }

    /// <summary>
    /// 查询企业Model
    /// </summary>
    public class SearchEnterpriseModel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }
    }


    public class DownloadCenterReturnModel
    {
        public string FileName { get; set; }

        public string FileUrl { get; set; }
    }

    /// <summary>
    /// 微信登录参数
    /// </summary>
    public class WeChatLoginModel
    {
        /// <summary>
        /// 用于解析session_key
        /// </summary>
        public string code { get; set; }

        public string randomstring { get; set; }

        /// <summary>
        /// 不包括敏感信息的原始数据字符串，用于计算签名
        /// </summary>
        public string rawData { get; set; }

        /// <summary>
        /// 包括敏感数据在内的完整用户信息的加密数据
        /// </summary>
        public string encryptedData { get; set; }
        /// <summary>
        /// 加密算法的初始向量
        /// </summary>
        public string iv { get; set; }
        /// <summary>
        /// 使用 sha1( rawData + sessionkey ) 得到字符串，用于校验用户信息
        /// </summary>
        public string signature { get; set; }
    }

    #region 手机登录

    public class SendCodeForMobileLoginModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }

    public class LoginPhoneNumberModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
    }

    public class LoginPhoneNumberReturnModel
    {
        public int AccountCount { get; set; }

        public List<User> Users { get; set; }

        public class User
        {
            public string ID { get; set; }

            public string UserName { get; set; }

            public string AvatarUrl { get; set; }
        }
    }

    public class LoginOneAccountModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }

    public class SwitchAccountModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }

    #endregion


    public class UserAvatar
    {
        public string ID { get; set; }

        public string AvatarUrl { get; set; }
    }

}