using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    /// <summary>
    /// 合同双方公司信息服务
    /// </summary>
    public class PartyContext<T> where T : class, new()
    {
        //static readonly object locker = new object();
        static T current;

        protected static string _settingPrefix = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public static T Current
        {
            get
            {
                //if (current == null)
                //{
                //    lock (locker)
                //    {
                //        if (current == null)
                //        {
                //            var name = System.Configuration.ConfigurationManager.AppSettings[_settingPrefix + typeof(T).Name];
                //            current = GetParty(name);
                //        }
                //    }
                //}

                //return current;

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

        public VendorContext(VendorContextInitParam initParam, string id)
        {
            switch (initParam)
            {
                case VendorContextInitParam.ClientID:

                    #region 通过 ClientID

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var client = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Where(t => t.ID == id).FirstOrDefault();
                        if (client != null && client.ClientType != null)
                        {
                            Enums.ClientType clientType = (Enums.ClientType)client.ClientType;
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
                        else
                        {
                            throw new Exception("VendorContext 中通过该 ClientID 查询不到或 ClientType 为空");
                        }
                    }

                    #endregion

                    break;
                case VendorContextInitParam.OrderID:

                    #region 通过 OrderID

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                        var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

                        var clientModel = (from order in orders
                                           join client in clients
                                              on new
                                              {
                                                  ClientID = order.ClientID,
                                                  OrderDataStatus = order.Status,
                                                  ClientDataStatus = (int)Enums.Status.Normal,
                                                  OrderID = order.ID,
                                              }
                                              equals new
                                              {
                                                  ClientID = client.ID,
                                                  OrderDataStatus = (int)Enums.Status.Normal,
                                                  ClientDataStatus = client.Status,
                                                  OrderID = id,
                                              }
                                           select client).FirstOrDefault();
                        if (clientModel != null && clientModel.ClientType != null)
                        {
                            Enums.ClientType clientType = (Enums.ClientType)clientModel.ClientType;
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
                        else
                        {
                            throw new Exception("VendorContext 中通过该 OrderID 查询不到或 ClientType 为空");
                        }
                    }

                    #endregion

                    break;
                case VendorContextInitParam.DecHeadID:

                    #region 通过 DecHeadID

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                        var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                        var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

                        var clientModel = (from order in orders
                                           join client in clients
                                              on new
                                              {
                                                  ClientID = order.ClientID,
                                                  OrderDataStatus = order.Status,
                                                  ClientDataStatus = (int)Enums.Status.Normal,
                                              }
                                              equals new
                                              {
                                                  ClientID = client.ID,
                                                  OrderDataStatus = (int)Enums.Status.Normal,
                                                  ClientDataStatus = client.Status,
                                              }
                                           join decHead in decHeads
                                               on new
                                               {
                                                   OrderID = order.ID,
                                                   DecHeadID = id,
                                               }
                                               equals new
                                               {
                                                   OrderID = decHead.OrderID,
                                                   DecHeadID = decHead.ID,
                                               }
                                           select client).FirstOrDefault();

                        if (clientModel != null && clientModel.ClientType != null)
                        {
                            Enums.ClientType clientType = (Enums.ClientType)clientModel.ClientType;
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
                        else
                        {
                            throw new Exception("VendorContext 中通过该 DecHeadID 查询不到或 ClientType 为空");
                        }
                    }

                    #endregion

                    break;
                case VendorContextInitParam.SwapNoticeID:

                    #region SwapNoticeID

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var swapNoticeItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>().Where(t => t.SwapNoticeID == id).FirstOrDefault();
                        if (swapNoticeItem == null)
                        {
                            throw new Exception("VendorContext 中通过该 SwapNoticeID 查询不到 SwapNoticeItem");
                        }

                        string decHeadID = swapNoticeItem.DecHeadID;

                        var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                        var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                        var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

                        var clientModel = (from order in orders
                                           join client in clients
                                              on new
                                              {
                                                  ClientID = order.ClientID,
                                                  OrderDataStatus = order.Status,
                                                  ClientDataStatus = (int)Enums.Status.Normal,
                                              }
                                              equals new
                                              {
                                                  ClientID = client.ID,
                                                  OrderDataStatus = (int)Enums.Status.Normal,
                                                  ClientDataStatus = client.Status,
                                              }
                                           join decHead in decHeads
                                               on new
                                               {
                                                   OrderID = order.ID,
                                                   DecHeadID = decHeadID,
                                               }
                                               equals new
                                               {
                                                   OrderID = decHead.OrderID,
                                                   DecHeadID = decHead.ID,
                                               }
                                           select client).FirstOrDefault();

                        if (clientModel != null && clientModel.ClientType != null)
                        {
                            Enums.ClientType clientType = (Enums.ClientType)clientModel.ClientType;
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
                        else
                        {
                            throw new Exception("VendorContext 中通过该 SwapNoticeID 查询不到或 ClientType 为空");
                        }
                    }

                    #endregion

                    break;

                case VendorContextInitParam.Pointed:
                    #region 指定方式
                    switch (id)
                    {
                        case "北京创新在线电子产品销售有限公司杭州分公司":
                            _settingPrefix = "WLT";
                            break;

                        case "北京创新在线电子产品销售有限公司":
                            _settingPrefix = "WLT";
                            break;

                        case "北京创新在线电子产品销售有限公司深圳分公司":
                            _settingPrefix = "WLT";
                            break;

                        case "深圳市创芯在线电子商务有限公司":
                            _settingPrefix = "WLT";
                            break;

                        case "山东创新在线电子商务有限公司":
                            _settingPrefix = "WLT";
                            break;

                        case "北京芯动能科技有限公司":
                            _settingPrefix = "WLT";
                            break;

                        case "深圳市丰掣供应链管理有限公司":
                            _settingPrefix = "WLT";
                            break;

                        case "深圳创芯在线检测技术有限公司":
                            _settingPrefix = "WLT";
                            break;
                        //深圳市丰掣供应链管理有限公司
                        //深圳创芯在线检测技术有限公司

                        case "杭州比一比电子科技有限公司":
                            _settingPrefix = "CY";
                            break;

                        case "北京远大创新科技有限公司":
                            _settingPrefix = "CY";
                            break;

                        default:
                            _settingPrefix = "CY";
                            break;
                    }
                    #endregion
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 根据报关日期，DDate 小于 2019-11-18 00:00:00，使用指定的 SettingPrefix
        /// DDate 在2019-11-18 00:00:00 到 2020-06-28 之间的，根据ClientType来判断
        /// DDate 大于等于 2020-06-28 00:00:00 ，根据公司名称来
        /// </summary>
        /// <param name="initParam"></param>
        /// <param name="id"></param>
        public VendorContext(VendorContextInitParam initParam, string id, string settingPrefix)
        {
            switch (initParam)
            {
                case VendorContextInitParam.SwapNoticeID:

                    #region SwapNoticeID

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var swapNoticeItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>().Where(t => t.SwapNoticeID == id).FirstOrDefault();
                        if (swapNoticeItem == null)
                        {
                            throw new Exception("VendorContext 中通过该 SwapNoticeID 查询不到 SwapNoticeItem");
                        }

                        string decHeadID = swapNoticeItem.DecHeadID;

                        var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                        var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                        var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
                        var companys = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

                        var resultModel = (from order in orders
                                           join client in clients
                                              on new
                                              {
                                                  ClientID = order.ClientID,
                                                  OrderDataStatus = order.Status,
                                                  ClientDataStatus = (int)Enums.Status.Normal,
                                              }
                                              equals new
                                              {
                                                  ClientID = client.ID,
                                                  OrderDataStatus = (int)Enums.Status.Normal,
                                                  ClientDataStatus = client.Status,
                                              }
                                           join decHead in decHeads
                                               on new
                                               {
                                                   OrderID = order.ID,
                                                   DecHeadID = decHeadID,
                                               }
                                               equals new
                                               {
                                                   OrderID = decHead.OrderID,
                                                   DecHeadID = decHead.ID,
                                               }
                                           join company in companys
                                              on new
                                              {
                                                  companyID = client.CompanyID
                                              }
                                              equals new
                                              {
                                                  companyID = company.ID
                                              }
                                           select new
                                           {
                                               Client = client,
                                               DecHead = decHead,
                                               CompanyName = company.Name,
                                           }).FirstOrDefault();

                        if (resultModel == null)
                        {
                            throw new Exception("VendorContext(带指定SettingPrefix) 中通过该 SwapNoticeID 查询不到");
                        }
                        if (resultModel.DecHead == null)
                        {
                            throw new Exception("VendorContext(带指定SettingPrefix) 中通过该 SwapNoticeID 查询不到 DecHead");
                        }

                        if (resultModel.DecHead.DDate != null && (DateTime)resultModel.DecHead.DDate < new System.DateTime(2019, 11, 18))
                        {
                            _settingPrefix = settingPrefix;
                        }
                        else
                        {
                            if ((DateTime)resultModel.DecHead.DDate < new System.DateTime(2020, 06, 28))
                            {
                                if (resultModel.Client == null || resultModel.Client.ClientType == null)
                                {
                                    throw new Exception("VendorContext(带指定SettingPrefix) 中通过该 SwapNoticeID 查询不到 Client 或 ClientType 为空");
                                }

                                Enums.ClientType clientType = (Enums.ClientType)resultModel.Client.ClientType;
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
                            else
                            {
                                switch (resultModel.CompanyName)
                                {
                                    case "北京创新在线电子产品销售有限公司杭州分公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京创新在线电子产品销售有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京创新在线电子产品销售有限公司深圳分公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳市创芯在线电子商务有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "山东创新在线电子商务有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京芯动能科技有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳市丰掣供应链管理有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳创芯在线检测技术有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "杭州比一比电子科技有限公司":
                                        _settingPrefix = "CY";
                                        break;

                                    case "北京远大创新科技有限公司":
                                        _settingPrefix = "CY";
                                        break;

                                    default:
                                        _settingPrefix = "CY";
                                        break;
                                }
                            }
                        }

                    }

                    #endregion

                    break;

                case VendorContextInitParam.Instrument:
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                        var orderCreatedate = orders.Where(t => t.ID == id).FirstOrDefault()?.CreateDate;
                        if ((DateTime)orderCreatedate >= new System.DateTime(2020, 06, 28))
                        {
                            var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                            var companys = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
                            var clientModel = (from order in orders
                                               join client in clients
                                                  on new
                                                  {
                                                      ClientID = order.ClientID,
                                                      OrderDataStatus = order.Status,
                                                      ClientDataStatus = (int)Enums.Status.Normal,
                                                      OrderID = order.ID,
                                                  }
                                                  equals new
                                                  {
                                                      ClientID = client.ID,
                                                      OrderDataStatus = (int)Enums.Status.Normal,
                                                      ClientDataStatus = client.Status,
                                                      OrderID = id,
                                                  }
                                               join company in companys
                                                   on new
                                                   {
                                                       companyID = client.CompanyID
                                                   }
                                                   equals new
                                                   {
                                                       companyID = company.ID
                                                   }
                                               select company.Name).FirstOrDefault();

                            if (clientModel != null)
                            {
                                switch (clientModel)
                                {
                                    case "北京创新在线电子产品销售有限公司杭州分公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京创新在线电子产品销售有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京创新在线电子产品销售有限公司深圳分公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳市创芯在线电子商务有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "山东创新在线电子商务有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "北京芯动能科技有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳市丰掣供应链管理有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "深圳创芯在线检测技术有限公司":
                                        _settingPrefix = "WLT";
                                        break;

                                    case "杭州比一比电子科技有限公司":
                                        _settingPrefix = "CY";
                                        break;

                                    case "北京远大创新科技有限公司":
                                        _settingPrefix = "CY";
                                        break;

                                    default:
                                        _settingPrefix = "CY";
                                        break;
                                }
                            }
                        }

                    }
                    break;

                default:
                    break;
            }
        }
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

        /// <summary>
        /// 每张发票的最大限额
        /// </summary>
        public string MaxAmountPerInvoice { get; set; }
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

        /// <summary>
        /// Pdf样式
        /// </summary>
        public string PdfStyle { get; set; }
    }

    public enum VendorContextInitParam
    {
        ClientID = 1,

        OrderID = 2,

        DecHeadID = 3,

        SwapNoticeID = 4,

        /// <summary>
        /// 指定
        /// 外单对应畅运，
        /// 杭州比一比对应畅运，
        /// 创新在线对应万路通，
        /// </summary>
        Pointed = 5,

        /// <summary>
        /// 代理报关委托书专用
        /// </summary>
        Instrument = 6
    }
}
