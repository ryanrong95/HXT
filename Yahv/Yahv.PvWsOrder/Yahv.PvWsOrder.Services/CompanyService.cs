using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services
{
    /// <summary>
    /// 合同双方公司信息服务
    /// </summary>
    public class PartyContext<T> where T : class, new()
    {
        static T current;

        protected static string _settingPrefix = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public static T Current
        {
            get
            {
                var name = System.Configuration.ConfigurationManager.AppSettings[_settingPrefix + typeof(T).Name];
                return GetParty(name);
            }
        }

        public T Current1
        {
            get
            {
                var name = System.Configuration.ConfigurationManager.AppSettings[_settingPrefix + typeof(T).Name];
                return GetParty(name);
            }
        }

        /// <summary>
        /// 获取当前的合同方公司
        /// </summary>
        /// <param name="name">配置文件中的公司名称</param>
        /// <returns></returns>
        static T GetParty(string name)
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\jsons\\" + name + ".json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    return jsonObject.ToObject<T>();
                }
            }
        }
    }

    public sealed class PurchaserContext : PartyContext<Purchaser>
    {

    }

    /// <summary>
    /// 合同双方中的买方
    /// </summary>
    public class Purchaser
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 公司印章的Url
        /// </summary>
        public string SealUrl { get; set; }

        /// <summary>
        /// 报关专用章
        /// </summary>
        public string DeclareStamp { get; set; }

        /// <summary>
        /// 公司英文名称
        /// </summary>
        public string DomesticConsigneeEname { get; set; }

        /// <summary>
        /// 使用单位联系人
        /// </summary>
        public string UseOrgPersonCode { get; set; }

        /// <summary>
        /// 使用单位联系方式
        /// </summary>
        public string UseOrgPersonTel { get; set; }

        /// <summary>
        /// 统一社会编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 十位海关编码
        /// </summary>
        public string CustomsCode { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CiqCode { get; set; }

        /// <summary>
        /// 海关操作员姓名
        /// </summary>
        public string DeclareName { get; set; }

        /// <summary>
        /// 报关员姓名章
        /// </summary>
        public string DeclareNameStampUrl { get; set; }

        /// <summary>
        /// 录入员IC卡号
        /// </summary>
        public string ICCode { get; set; }

        /// <summary>
        /// 海关导入客户端ID编号
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 备案关区
        /// </summary>
        public string CustomMaster { get; set; }

        /// <summary>
        /// 合同专用章
        /// </summary>
        public string ContactStamp { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 开户账户
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 发票章
        /// </summary>
        public string InvoiceStamp { get; set; }
        /// <summary>
        /// 账单的章
        /// </summary>
        public string BillStamp { get; set; }
        /// <summary>
        /// 页眉图片
        /// </summary>
        public string HeaderImg { get; set; }
        /// <summary>
        /// 官方网址
        /// </summary>
        public string OfficalWebsite { get; set; }
        /// <summary>
        /// 官方网址简
        /// </summary>
        public string OfficalHtml { get; set; }
        /// <summary>
        /// 合同号前缀
        /// </summary>
        public string ContractNoPrefix { get; set; }
        /// <summary>
        /// 提运单号前缀
        /// </summary>
        public string BillNoPrefix { get; set; }
        /// <summary>
        /// 商检合同号前缀
        /// </summary>
        public string SJContractNoPrefix { get; set; }
        /// <summary>
        /// 商检提运单号前缀
        /// </summary>
        public string SJBillNoPrefix { get; set; }
        /// <summary>
        /// 报关单表头ID前缀
        /// </summary>
        public string DecHeadIDPrefix { get; set; }
    }
    public sealed class VendorContext : PartyContext<Vendor>
    {
        //public VendorContext()
        //{

        //}

        public VendorContext(Enums.ClientType clientType)
        {
            switch (clientType)
            {
                case Enums.ClientType.Internal:
                    _settingPrefix = clientType.ToString();
                    break;
                case Enums.ClientType.External:
                    _settingPrefix = clientType.ToString();
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 合同双方中的卖方
    /// </summary>
    public class Vendor
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 公司印章的Url
        /// </summary>
        public string SealUrl { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string OverseasConsignorCname { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string OverseasConsignorAddr { get; set; }

        /// <summary>
        /// 带签名的公司印章Url
        /// </summary>
        public string SignSealUrl { get; set; }

        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 英文地址
        /// </summary>
        public string AddressEN { get; set; }

        /// <summary>
        /// 合同号前缀
        /// </summary>
        public string ContractNoPrefix { get; set; }
        /// <summary>
        /// 提运单号前缀
        /// </summary>
        public string BillNoPrefix { get; set; }
        /// <summary>
        /// 商检合同号前缀
        /// </summary>
        public string SJContractNoPrefix { get; set; }
        /// <summary>
        /// 商检提运单号前缀
        /// </summary>
        public string SJBillNoPrefix { get; set; }

        /// <summary>
        /// LogoUrl
        /// </summary>
        public string LogoUrl { get; set; }
    }

    public enum VendorContextInitParam
    {
        ClientID = 1,

        OrderID = 2,

        DecHeadID = 3,

        SwapNoticeID = 4,
    }
}
