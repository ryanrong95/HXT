using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class AccountController : UserController
    {
        #region 客户基础信息
        /// <summary>
        /// 客户基础信息数据加载
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, ToCompleteInfo = true)]
        public ActionResult BaseInformations()
        {
            var CurrentUser = Yahv.Client.Current;
            var Client = CurrentUser.MyClients;

            ServiceType serviceType = Client.ServiceType;
            WsIdentity storageType = Client.StorageType;

            // 查询头像信息
            var avatarFile = new CenterFilesView().Where(t => t.AdminID == CurrentUser.ID && t.Type == (int)FileType.Avatar).FirstOrDefault();

            var model = new
            {
                UserName = CurrentUser.UserName,
                Mobile = CurrentUser.Mobile,
                Email = CurrentUser.Email,
                CompanyName = Client?.Name,
                Corporation = Client?.Corporation,
                RegAddress = Client?.RegAddress,
                Uscc = Client?.Uscc,

                ThePageIsCustoms = (serviceType & ServiceType.Customs) == ServiceType.Customs,
                ThePageIsWarehouse = (serviceType & ServiceType.Warehouse) == ServiceType.Warehouse,
                StorageTypeInt = (int)storageType,
                ThePageHasExport = Client.HasExport.Value,

                AvatarUrl = !string.IsNullOrEmpty(avatarFile?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + avatarFile?.Url.ToUrl() : "",

                invoiceRuleForm = GetData()
            };

            return View(model);
        }

        /// <summary>
        /// 检查登录名是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult CheckName(string name)
        {
            name = name.InputText();
            var user = Yahv.Client.Current;

            if (Yahv.Alls.Current.AllUsers.UserNameIsExist(user.ID, name))
            {
                return base.JsonResult(VueMsgType.error, "登录名[" + name + "]已注册，不可使用。");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 修改会员名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult ChangeUserName(string username)
        {
            username = username.InputText();
            try
            {
                var user = Yahv.Client.Current;
                if (username == user.UserName)
                {
                    return base.JsonResult(VueMsgType.success, "修改登录名成功");
                }
                Client.Current.ChangeUserName(username);
                return base.JsonResult(VueMsgType.success, "修改登录名成功");
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 完善基本信息提示
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult CompleteBaseInfoTip()
        {
            Response.Headers.Add(UserAuthorizeAttribute.jumpto, JumpToPage.Account_CompleteBaseInfoTip.ToString());
            return View();
        }

        /// <summary>
        /// 不能下单
        /// </summary>
        /// <returns></returns>
        public ActionResult CannotOrderTip()
        {
            Response.Headers.Add(UserAuthorizeAttribute.jumpto, JumpToPage.Account_CannotOrderTip.ToString());
            return View();
        }

        #endregion


        #region 开票信息管理
        /// <summary>
        /// 开票信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult InvoiceInfo()
        {
            var current = Yahv.Client.Current;
            var Client = current.MyClients;

            ServiceType serviceType = Client.ServiceType;
            WsIdentity storageType = Client.StorageType;

            if (((serviceType & ServiceType.Warehouse) == ServiceType.Warehouse) && storageType == WsIdentity.Personal)
            {
                var model = new
                {
                    IsMain = current.IsMain,
                };

                return View("PersonInvoice", model);
            }
            else
            {
                var model = this.GetData();

                return View(model);
            }
        }

        /// <summary>
        /// 获取发票信息（刷新数据）
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetInvoiceData()
        {
            var model = this.GetData();
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 拼凑发票数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        private InvoiceViewModel GetData()
        {
            var current = Client.Current;
            var invoice = current.MyInvoice.SingleOrDefault();
            var model = new InvoiceViewModel()
            {
                ID = invoice?.ID,
                DeliveryType = ((int?)invoice?.DeliveryType).ToString(),
                DeliveryTypeName = invoice?.DeliveryType.GetDescription(),
                CompanyName = current.MyClients.Name,
                CompanyTel = invoice?.CompanyTel,
                Type = ((int?)invoice?.Type).ToString(),
                TypeName = invoice?.Type.GetDescription(),
                Bank = invoice?.Bank,
                BankAddress = invoice?.BankAddress,
                Account = invoice?.Account,
                RegAddress = invoice?.RegAddress,
                Postzip = invoice?.Postzip,
                Name = invoice?.Name,
                Mobile = invoice?.Mobile,
                Email = invoice?.Email,
                TaxperNumber = invoice?.TaxperNumber,
                Tel = invoice?.Tel,
                Address = invoice?.Address,
                AddressArray = invoice?.Address?.ToAddress(),
                AddressDetail = invoice?.Address?.ToDetailAddress(),
                InvoiceDeliveryTypeOptions = ExtendsEnum.ToDictionary<InvoiceDeliveryType>()
                                                .Where(t => t.Key != InvoiceDeliveryType.UnKnown.GetHashCode().ToString())
                                                .Select(item => new { value = item.Key, text = item.Value }).Json(),
                InvoiceTypeOptions = ExtendsEnum.ToDictionary<InvoiceType>().Where(item => item.Key != "0" && item.Key != "1" && item.Key != "4")
                                                                            .Select(item => new { value = item.Key, text = item.Value }).Json(),
                IsMain = current.IsMain,
            };
            return model;
        }

        /// <summary>
        /// 新增发票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult Invoice(InvoiceViewModel model)
        {
            try
            {
                var user = Client.Current;
                var invoice = new ShencLibrary.SyncInvoice()
                {
                    Type = (InvoiceType)int.Parse(model.Type ?? InvoiceType.Unkonwn.GetHashCode().ToString()), //如果前端传来的是null, 应该是查询出来的Type就是 null, 这里赋值"0"(未知)
                    Bank = model.Bank,
                    RegAddress = model.RegAddress,
                    Account = model.Account,
                    TaxperNumber = model.TaxperNumber,
                    Name = model.Name,
                    Tel = model.Tel,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    //Address = model.Address,
                    Address = string.Join(" ", model.AddressArray?.Concat(new string[] { model.AddressDetail.Trim() }) ?? Array.Empty<string>()),
                    Postzip = model.Postzip,
                    DeliveryType = (InvoiceDeliveryType)int.Parse(model.DeliveryType),
                    CompanyTel = model.CompanyTel,
                };
                //开票信息持久化
                new ShencLibrary.DccInvoice().Enter(user.EnterpriseID, invoice, user.ID);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "新增失败" + ex.Message);
            }

            return base.JsonResult(VueMsgType.success, "新增成功");
        }
        #endregion

        #region 个人发票信息 增删改查

        /// <summary>
        /// 获取个人发票信息列表数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetPersonInvoiceList()
        {
            var current = Client.Current;
            //收货地址列表
            var linq = current.MyPersonInvoices.Where(t => t.Status == GeneralStatus.Normal).ToArray().Select(item => new
            {
                ID = item.ID,
                PerOrEnpDes = item.IsPersonal ? "个人发票" : "企业发票",
                TypeDes = item.Type.GetDescription(),
                Title = item.Title,
                TaxNumber = item.TaxNumber,
                DeliveryTypeDes = item.DeliveryType.GetDescription(),
                IsDefaultDes = item.IsDefault ? "是" : "否",
            });
            return base.JsonResult(VueMsgType.success, "", new { personInvoices = linq.ToArray(), current.IsMain }.Json());
        }

        /// <summary>
        /// 获取单个个人发票信息数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetPersonInvoiceInfo(string id)
        {
            var model = new PersonInvoiceViewModel();
            var onePersonInvoice = Client.Current.MyPersonInvoices.Where(t => t.ID == id).FirstOrDefault();
            if (onePersonInvoice != null)
            {
                model.ID = onePersonInvoice.ID;
                model.IsPersonalVal = onePersonInvoice.IsPersonal ? "1" : "2";
                model.InvoiceTypeInt = Convert.ToString((int)onePersonInvoice.Type);
                model.Title = onePersonInvoice.Title;
                model.TaxNumber = onePersonInvoice.TaxNumber;
                model.RegAddress = onePersonInvoice.RegAddress;
                model.Tel = onePersonInvoice.Tel;
                model.BankName = onePersonInvoice.BankName;
                model.BankAccount = onePersonInvoice.BankAccount;
                model.PostAddress = onePersonInvoice.PostAddress;
                model.PostRecipient = onePersonInvoice.PostRecipient;
                model.PostTel = onePersonInvoice.PostTel;
                model.PostZipCode = onePersonInvoice.PostZipCode;
                model.DeliveryTypeInt = Convert.ToString((int)onePersonInvoice.DeliveryType);
                model.IsDefaultVal = onePersonInvoice.IsDefault ? "1" : "2";
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 编辑个人发票信息页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _ParticalPersonInvoiceEdit(string id)
        {
            var model = new PersonInvoiceViewModel();



            ViewBag.InvoiceTypeOptions = ExtendsEnum.ToDictionary<InvoiceType>().Where(item => item.Key != "0")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            ViewBag.DeliveryTypeOptions = ExtendsEnum.ToDictionary<InvoiceDeliveryType>()
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();

            return PartialView(model);
        }

        /// <summary>
        /// 个人发票信息提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult PersonInvoiceSubmit(PersonInvoiceViewModel model)
        {
            try
            {
                var current = Client.Current;
                var theClient = current.MyClients;

                var personInvoice = new ShencLibrary.SyncvInvoice()
                {
                    ID = model.ID,
                    EnterpriseID = theClient.ID,
                    IsPersonal = (model.IsPersonalVal == "1"),
                    Type = (InvoiceType)(int.Parse(model.InvoiceTypeInt)),
                    Title = model.Title,
                    TaxNumber = model.TaxNumber,
                    RegAddress = model.RegAddress,
                    Tel = model.Tel,
                    BankName = model.BankName,
                    BankAccount = model.BankAccount,
                    PostAddress = model.PostAddress,
                    PostRecipient = model.PostRecipient,
                    PostTel = model.PostTel,
                    PostZipCode = model.PostZipCode,
                    DeliveryType = (InvoiceDeliveryType)int.Parse(model.DeliveryTypeInt),
                    IsDefault = (model.IsDefaultVal == "1"),
                    CreatorID = current.ID,
                };

                //数据持久化
                var isSuccess = new ShencLibrary.DccvInvoice().Enter(personInvoice);
                if (isSuccess)
                {
                    return base.JsonResult(VueMsgType.success, "操作成功", new { }.Json());
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, "操作失败");
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 删除个人发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult DeletePersonInvoice(string id)
        {
            new ShencLibrary.DccvInvoice().Abandon(id);
            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        #endregion

        #region 收货地址
        /// <summary>
        /// 收货地址
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyConsignees()
        {
            var current = Client.Current;
            //收货地址列表
            var linq = current.MyConsignees.ToArray().Select(item => new
            {
                ID = item.ID,
                Title = item.Title,
                Place = string.IsNullOrWhiteSpace(item.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), item.Place)).GetDescription(),
                Address = item.Address,
                Name = item.Name,
                Tel = item.Tel,
                Mobile = item.Mobile,
                Email = item.Email,
                IsDefault = item.IsDefault ? "是" : "否",
            });
            var model = new
            {
                consignees = linq.ToArray(),
                IsMain = current.IsMain,
            };
            return View(model);
        }

        /// <summary>
        ///  获取收货地址信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetConsigneesInfo(string id)
        {
            var model = new ConsigneeViewModel();
            var list = Client.Current.MyConsignees[id];
            if (list != null)
            {
                model.ID = list.ID;
                model.Title = list.Title;
                model.Place = string.IsNullOrWhiteSpace(list.Place) ? "" : ((int)((Origin)Enum.Parse(typeof(Origin), list.Place))).ToString();
                model.Address = list.Place == Origin.CHN.ToString() ? list.Address.ToAddress() : null;
                model.AddressDetail = list.Place == Origin.CHN.ToString() ? list.Address.ToDetailAddress() : list.Address;
                model.Name = list.Name;
                model.Tel = list.Tel;
                model.Mobile = list.Mobile;
                model.Email = list.Email;
                model.IsDefault = list.IsDefault;
                model.IsDefaultVal = list.IsDefault ? "1" : "2";
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _ParticalConsignee(string id)
        {
            var model = new ConsigneeViewModel();

            model.PlaceDefault = (Origin.HKG).GetHashCode().ToString(); //默认国家/地区

            //国家/地区
            Dictionary<string, string> dicPlace = new Dictionary<string, string>();

            var allNeed = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27");

            string[] alwaysUsePlaceName = { "中国香港", "中国台湾", "美国" };

            foreach (var item in alwaysUsePlaceName)
            {
                var thePair = allNeed.Where(t => t.Value == item).FirstOrDefault();
                if (thePair.Key != null)
                {
                    dicPlace.Add(thePair.Key, thePair.Value);
                }
            }

            foreach (var item in allNeed)
            {
                if (!alwaysUsePlaceName.Contains(item.Value))
                {
                    dicPlace.Add(item.Key, item.Value);
                }
            }

            ViewBag.PlaceOptions = dicPlace.Select(item => new { value = item.Key, text = item.Value });

            //ViewBag.PlaceOptions = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
            //    .Select(item => new { value = item.Key, text = item.Value }).ToArray(); ;
            return PartialView(model);
        }

        /// <summary>
        /// 新增收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult ConsigneeSubmit(ConsigneeViewModel model)
        {
            try
            {
                var user = Client.Current;
                var consignee = new ShencLibrary.SyncConsignee()
                {
                    Title = model.Title.Trim(),
                    Place = ((Origin)int.Parse(model.Place)).GetOrigin().Code,
                    Address = (Origin)int.Parse(model.Place) != Origin.CHN ? model.AddressDetail.Trim() : string.Join(" ", model.Address?.Concat(new string[] { model.AddressDetail.Trim() }) ?? Array.Empty<string>()),
                    Postzip = string.Empty,
                    Name = model.Name.Trim(),
                    Tel = model.Tel,
                    Mobile = model.Mobile,
                    Email = model.Email ?? string.Empty,
                    IsDefault = model.IsDefaultVal == "1" ? true : false,
                    DyjCode = string.Empty,
                };
                //数据持久化
                var id = new ShencLibrary.DccConsignee().Enter(user.EnterpriseID, consignee, model.ID);
                return base.JsonResult(VueMsgType.success, "操作成功", id);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMyConsigneesList()
        {
            var current = Client.Current;
            //收货地址列表
            var linq = current.MyConsignees.ToArray().Select(item => new
            {
                ID = item.ID,
                Title = item.Title,
                Place = string.IsNullOrWhiteSpace(item.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), item.Place)).GetDescription(),
                Address = item.Address,
                Name = item.Name,
                Tel = item.Tel,
                Mobile = item.Mobile,
                Email = item.Email,
                IsDefault = item.IsDefault ? "是" : "否",
            });
            return base.JsonResult(VueMsgType.success, "", new { consignees = linq.ToArray(), current.IsMain }.Json());
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult SetDefaultConsignee(string id)
        {
            //数据持久化
            new ShencLibrary.DccConsignee().SetDefault(Client.Current.EnterpriseID, id);
            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult DeleteConsignee(string id)
        {
            new ShencLibrary.DccConsignee().Abandon(Client.Current.EnterpriseID, id);
            return base.JsonResult(VueMsgType.success, "删除成功");
        }
        #endregion


        #region 条款协议（补充协议）
        /// <summary>
        /// 条款协议
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult Agreement()
        {
            var current = Yahv.Client.Current;
            var client = current.MyClients;

            var agree = current.MyAgreement.FirstOrDefault();
            //货款条款
            var productFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 1);
            //税费条款
            var taxFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 2);
            //代理费条款
            var agencyFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 3);
            //杂费条款
            var incidentalFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 4);

            ServiceType serviceType = client.ServiceType;

            CenterFileDescription fileServiceAgreement = null;
            if (agree != null)
            {
                fileServiceAgreement = new CenterFilesView().FirstOrDefault(item => item.ClientID == agree.ClientID && item.Type == (int)FileType.ServiceAgreement);
            }
            CenterFileDescription fileStorageAgreement = null;
            fileStorageAgreement = new CenterFilesView().FirstOrDefault(item => item.ClientID == current.XDTClientID && item.Type == (int)FileType.StorageAgreement);

            #region 计算年份

            var years = agree?.EndDate.Year - agree?.StartDate.Year;
            var y = "";
            //1=壹，2=贰，3=叁，4=肆，5=伍，6=陆，7=柒，8=捌，9=玖，10=拾。
            switch (years)
            {
                case 1:
                    y = "壹";
                    break;
                case 2:
                    y = "贰";
                    break;
                case 3:
                    y = "叁";
                    break;
                case 4:
                    y = "肆";
                    break;
                case 5:
                    y = "伍";
                    break;
                case 6:
                    y = "陆";
                    break;
                case 7:
                    y = "柒";
                    break;
                case 8:
                    y = "捌";
                    break;
                case 9:
                    y = "玖";
                    break;
                case 10:
                    y = "拾";
                    break;
                default:
                    break;
            }

            #endregion

            #region 汇率约定

            //付汇汇率
            var PEIsTen = agree.IsTen == Yahv.PvWsOrder.Services.XDTClientView.PEIsTen.Ten ? "10:00" : "09:30";

            //税费汇率
            var TaxExchangeRateName = "";
            if (taxFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Custom)
            {
                TaxExchangeRateName = "进口当月海关汇率";
            }
            else if (taxFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.RealTime)
            {
                TaxExchangeRateName = "进口当天中国银行10:00之后第一个外汇卖出价";
            }
            else
            {
                TaxExchangeRateName = "双方约定汇率";
            }

            //服务费汇率
            var AgencyExchangeRateName = "";
            if (agencyFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Custom)
            {
                AgencyExchangeRateName = "进口当月海关汇率";
            }
            else if (agencyFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.RealTime)
            {
                AgencyExchangeRateName = "进口当天中国银行10:00之后第一个外汇卖出价";
            }
            else
            {
                AgencyExchangeRateName = "双方约定汇率";
            }

            #endregion

            #region 账期类型额度

            string  GoodsPaymentPeriod,  TaxPaymentPeriod,  AgentPaymentPeriod,  OtherPaymentPeriod, AgencyRate, MinimumAgent  = "";


            var preAgency = (agree.PreAgency.HasValue && agree.PreAgency > 0) ? (agree.PreAgency.Value.ToRound(2).ToString() + "元 + ") : "";
            AgencyRate = preAgency + (agree.AgencyRate * 100M).ToRound(2).ToString() + "%";
            MinimumAgent = agree.MinAgencyFee.ToRound(2).ToString();

            //税款
            if (productFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.PrePaid)
            {
                //GoodsPaymentPre = "☑无信用额度";
                GoodsPaymentPeriod = "无信用额度";
            }
            else if (productFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod)
            {
                //GoodsPaymentPre = "□无信用额度";
                GoodsPaymentPeriod = "信用额度：约定期限，进口" + productFeeClause.DaysLimit.Value.ToString() + "天；额度：" + productFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }
            else
            {
                //GoodsPaymentPre = "□无信用额度";
                GoodsPaymentPeriod = "信用额度：月结，次月" + productFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + productFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }

            //税款
            if (taxFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.PrePaid)
            {
                //TaxPaymentPre = "无信用额度";
                TaxPaymentPeriod = "无信用额度";
            }
            else if (taxFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod)
            {
                //TaxPaymentPre = "□无信用额度";
                TaxPaymentPeriod = "信用额度：约定期限，进口" + taxFeeClause.DaysLimit.Value.ToString() + "天；额度：" + taxFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }
            else
            {
                //TaxPaymentPre = "□无信用额度";
                TaxPaymentPeriod = "信用额度：月结，次月" + taxFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + taxFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }

            //服务费
            if (agencyFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.PrePaid)
            {
                //AgentPaymentPre = "☑无信用额度";
                AgentPaymentPeriod = "无信用额度";
            }
            else if (agencyFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod)
            {
                //AgentPaymentPre = "□无信用额度";
                AgentPaymentPeriod = "信用额度：约定期限，进口" + agencyFeeClause.DaysLimit.Value.ToString() + "天；额度：" + agencyFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }
            else
            {
                //AgentPaymentPre = "□无信用额度";
                AgentPaymentPeriod = "信用额度：月结，次月" + agencyFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + agencyFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }

            //杂费
            if (incidentalFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.PrePaid)
            {
                //OtherPaymentPre = "☑无信用额度";
                OtherPaymentPeriod = "无信用额度";
            }
            else if (incidentalFeeClause.PeriodType == Yahv.PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod)
            {
                //OtherPaymentPre = "□无信用额度";
                OtherPaymentPeriod = "信用额度：约定期限，进口" + incidentalFeeClause.DaysLimit.Value.ToString() + "天；额度：" + incidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }
            else
            {
                //OtherPaymentPre = "□无信用额度";
                OtherPaymentPeriod = "信用额度：月结，次月" + incidentalFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + incidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元";
            }



            #endregion

            var data = new
            {
                ClientName = client.Name,
                ClientRegAddress = client.RegAddress,
                ClientCorporation = client.Corporation,
                StartDate = agree?.StartDate.ToString("yyyy年MM月dd日"),
                EndDate = agree?.EndDate.ToString("yyyy年MM月dd日"),
                Year = y,
                IsTen = PEIsTen,
                InvoiceRateValue = Math.Round((decimal)(1M + agree?.InvoiceTaxRate), 2,MidpointRounding.AwayFromZero).ToString(),
                InvoiceTypeDescription = agree?.InvoiceType == PvWsOrder.Services.XDTClientView.Invoice.Full ? "签署内贸合同，受托方以进口货物价款、关税、增值税、消费税、服务费向委托方开具税率为13%的增值税专用发票" : "受托方向委托方提供海关进口关税专用缴款书、海关进口增值税专用缴款书和报关单，受托方以所收服务费开具税率为6%的增值税专用发票",
                //GoodsPaymentPre,
                GoodsPaymentPeriod,
                //TaxPaymentPre,
                TaxPaymentPeriod,
                //AgentPaymentPre,
                AgentPaymentPeriod,
                //OtherPaymentPre,
                OtherPaymentPeriod,
                AgencyRate,
                MinimumAgent,

                //AgencyRate = agree?.AgencyRate.ToString(CultureInfo.InvariantCulture),
                MinAgencyFee = agree?.MinAgencyFee.ToString(CultureInfo.InvariantCulture),
                IsPrePayExchange = agree == null ? "" : agree.IsPrePayExchange ? "预换汇" : "90天内换汇",

                GoodsPeriodType = productFeeClause?.PeriodType.GetDescription(),
                GoodsExchangeRateType = productFeeClause?.ExchangeRateType.GetDescription(),
                GoodsUpperLimit = productFeeClause?.UpperLimit.ToString(),
                isGoodsPrePaid = productFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid,//是否为预付款
                isGoodsAgreedPeriod = productFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod,//是否为约定期限
                GoodsDaysLimit = productFeeClause?.DaysLimit.ToString(),
                isGoodsMonthly = productFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly,//是否为月结
                GoodsMonthlyDay = productFeeClause?.MonthlyDay.ToString(),
                isGoodsAgreed = productFeeClause?.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed,//是否为约定汇率
                GoodsExchangeRateValue = productFeeClause?.ExchangeRateValue.ToString(),

                TaxPeriodType = taxFeeClause?.PeriodType.GetDescription(),
                TaxExchangeRateType = taxFeeClause?.ExchangeRateType.GetDescription(),
                TaxUpperLimit = taxFeeClause?.UpperLimit.ToString(),
                isTaxPrePaid = taxFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid,//是否为预付款
                isTaxAgreedPeriod = taxFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod,//是否为约定期限
                TaxDaysLimit = taxFeeClause?.DaysLimit.ToString(),
                isTaxMonthly = taxFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly,//是否为月结
                TaxMonthlyDay = taxFeeClause?.MonthlyDay.ToString(),
                isTaxAgreed = taxFeeClause != null && taxFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed,//是否为约定汇率
                TaxExchangeRateValue = taxFeeClause?.ExchangeRateValue.ToString(),
                TaxExchangeRateName = TaxExchangeRateName,//

                AgencyFeePeriodType = agencyFeeClause?.PeriodType.GetDescription(),
                AgencyFeeExchangeRateType = agencyFeeClause?.ExchangeRateType.GetDescription(),
                AgencyFeeUpperLimit = agencyFeeClause?.UpperLimit.ToString(),
                isAgencyPrePaid = agencyFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid,//是否为预付款
                isAgencyAgreedPeriod = agencyFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod,//是否为约定期限
                AgencyDaysLimit = agencyFeeClause?.DaysLimit.ToString(),
                isAgencyMonthly = agencyFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly,//是否为月结
                AgencyMonthlyDay = agencyFeeClause?.MonthlyDay.ToString(),
                isAgencyAgreed = agencyFeeClause?.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed,//是否为约定汇率
                AgencyExchangeRateValue = agencyFeeClause?.ExchangeRateValue.ToString(),
                AgencyExchangeRateName = AgencyExchangeRateName,//

                IncidentalPeriodType = incidentalFeeClause?.PeriodType.GetDescription(),
                IncidentalExchangeRateType = incidentalFeeClause?.ExchangeRateType.GetDescription(),
                IncidentalUpperLimit = incidentalFeeClause?.UpperLimit.ToString(),
                isIncidentalPrePaid = incidentalFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid,//是否为预付款
                isIncidentalAgreedPeriod = incidentalFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod,//是否为约定期限
                IncidentalDaysLimit = incidentalFeeClause?.DaysLimit.ToString(),
                isIncidentalMonthly = incidentalFeeClause?.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly,//是否为月结
                IncidentalMonthlyDay = incidentalFeeClause?.MonthlyDay.ToString(),
                isIncidentalAgreed = incidentalFeeClause?.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed,//是否为约定汇率
                IncidentalExchangeRateValue = incidentalFeeClause?.ExchangeRateValue.ToString(),

                InvoiceType = agree?.InvoiceType.GetDescription(),
                InvoiceRate = agree?.InvoiceTaxRate.ToString(CultureInfo.InvariantCulture),
                InvoiceName = agree?.InvoiceType == PvWsOrder.Services.XDTClientView.Invoice.Full ? "单抬头" : "双抬头",
                ClientID = agree?.ClientID,
                FileUrl = !string.IsNullOrEmpty(fileServiceAgreement?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + fileServiceAgreement?.Url.ToUrl() : "",
                FileName = fileServiceAgreement?.CustomName,

                StorageFileUrl = !string.IsNullOrEmpty(fileStorageAgreement?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + fileStorageAgreement?.Url.ToUrl() : "",
                StorageFileName = fileStorageAgreement?.CustomName,

                ThePageIsCustoms = (serviceType & ServiceType.Customs) == ServiceType.Customs,
                ThePageIsWarehouse = (serviceType & ServiceType.Warehouse) == ServiceType.Warehouse,
                XDTClientID = current.XDTClientID,
            };
            return View(data);
        }

        #endregion


        #region 供应商管理

        #region 页面初始化
        /// <summary>
        /// 供应商列表页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MySuppliers()
        {
            return View();
        }

        /// <summary>
        /// 新增供应商页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult AddSupplier()
        {
            var CurrentUser = Yahv.Client.Current;
            var Client = CurrentUser.MyClients;

            ServiceType serviceType = Client.ServiceType;
            ViewBag.ThePageIsCustoms = ((serviceType & ServiceType.Customs) == ServiceType.Customs) ? "true" : "false"; //有报关服务类型

            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var model = new AddSupplierViewModel();
            model.IsAddress = false;
            model.IsBank = false;
            model.IsContact = false;

            //国家/地区
            Dictionary<string, string> dicPlace = new Dictionary<string, string>();

            var allNeed = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27");

            string[] alwaysUsePlaceName = { "中国香港", "中国台湾", "美国" };

            foreach (var item in alwaysUsePlaceName)
            {
                var thePair = allNeed.Where(t => t.Value == item).FirstOrDefault();
                if (thePair.Key != null)
                {
                    dicPlace.Add(thePair.Key, thePair.Value);
                }
            }

            foreach (var item in allNeed)
            {
                if (!alwaysUsePlaceName.Contains(item.Value))
                {
                    dicPlace.Add(item.Key, item.Value);
                }
            }

            ViewBag.PlaceOptions = dicPlace.Select(item => new { value = item.Key, text = item.Value });

            //ViewBag.PlaceOptions = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
            //    .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            ViewBag.CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return View(model);
        }


        /// <summary>
        /// 供应商详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult SupplierDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var supplier = Client.Current.MySupplier[id];
            var data = new
            {
                supplier.ID,
                Name = supplier.CHNabbreviation,
                supplier.ChineseName,
                supplier.EnglishName,
                supplier.nGrade,
                supplier.RegAddress,
                Place = string.IsNullOrWhiteSpace(supplier.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), supplier.Place)).GetDescription(),
                Banks = supplier.MySupplierPayees.ToArray().Select(item => new
                {
                    item.ID,
                    RealName = item.Contact,
                    item.Account,
                    Place = string.IsNullOrWhiteSpace(item.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), item.Place)).GetDescription(),
                    item.Bank,
                    item.BankAddress,
                    item.SwiftCode,
                }).ToArray(),
                Contacts = supplier.MySupplierContact.ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Fax,
                    item.Email,
                    item.QQ,
                    Status = item.Status.GetDescription(),
                }).ToArray(),
                Address = supplier.MySupplierAddress.ToArray().Select(item => new
                {
                    item.ID,
                    item.Address,
                    item.Contact,
                    item.Mobile,
                    item.Tel,
                    item.Email,
                    item.PostZip,
                }).ToArray(),
            };
            return View(data);
        }


        /// <summary>
        /// 新增供应商银行页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialSupplierBankInfo()
        {
            var CurrentUser = Yahv.Client.Current;
            var Client = CurrentUser.MyClients;

            ServiceType serviceType = Client.ServiceType;
            ViewBag.ThePageIsCustoms = ((serviceType & ServiceType.Customs) == ServiceType.Customs) ? "true" : "false"; //有报关服务类型


            var model = new BeneficiarieInfoViewModel();

            //国家/地区
            Dictionary<string, string> dicPlace = new Dictionary<string, string>();

            var allNeed = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27");

            string[] alwaysUsePlaceName = { "中国香港", "中国台湾", "美国" };

            foreach (var item in alwaysUsePlaceName)
            {
                var thePair = allNeed.Where(t => t.Value == item).FirstOrDefault();
                if (thePair.Key != null)
                {
                    dicPlace.Add(thePair.Key, thePair.Value);
                }
            }

            foreach (var item in allNeed)
            {
                if (!alwaysUsePlaceName.Contains(item.Value))
                {
                    dicPlace.Add(item.Key, item.Value);
                }
            }

            ViewBag.PlaceOptions = dicPlace.Select(item => new { value = item.Key, text = item.Value });

            //ViewBag.PlaceOptions = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
            //    .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            ViewBag.CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0" && item.Key != "1").Select(item => new { value = item.Key, text = item.Value }).ToArray();
            ViewBag.SupplierOptions = Yahv.Client.Current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray();
            return PartialView(model);
        }

        /// <summary>
        /// 新增供应商信息页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialSupplierInfo()
        {
            var model = new SupplierInfoViewModel();

            //国家/地区
            Dictionary<string, string> dicPlace = new Dictionary<string, string>();

            var allNeed = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27");

            string[] alwaysUsePlaceName = { "中国香港", "中国台湾", "美国" };

            foreach (var item in alwaysUsePlaceName)
            {
                var thePair = allNeed.Where(t => t.Value == item).FirstOrDefault();
                if (thePair.Key != null)
                {
                    dicPlace.Add(thePair.Key, thePair.Value);
                }
            }

            foreach (var item in allNeed)
            {
                if (!alwaysUsePlaceName.Contains(item.Value))
                {
                    dicPlace.Add(item.Key, item.Value);
                }
            }

            ViewBag.PlaceOptions = dicPlace.Select(item => new { value = item.Key, text = item.Value });

            //ViewBag.PlaceOptions = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
            //    .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return PartialView(model);
        }

        /// <summary>
        /// 新增供应商提货地址
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialSupplierAddressInfo()
        {
            SupplierAddressesViewModel model = new SupplierAddressesViewModel();
            ViewBag.SupplierOptions = Client.Current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray();
            return PartialView(model);
        }

        /// <summary>
        /// 新增供应商联系人
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult _PartialSupplierContactInfo()
        {
            SupplierContactModel model = new SupplierContactModel();
            return PartialView(model);
        }
        #endregion


        #region 列表查询
        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetSuppliersList(string name)
        {
            name = name.Trim();
            var current = Client.Current;
            var list = current.MySupplier.Where(item => item.ChineseName.Contains(name) || item.EnglishName.Contains(name)).ToList().
                Select(item => new
                {
                    item.ID,
                    Name = item.RealEnterpriseName,
                    item.EnglishName,
                    item.ChineseName,
                    Place = string.IsNullOrWhiteSpace(item.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), item.Place)).GetDescription(),
                    item.RegAddress,
                    item.nGrade
                });
            return JsonResult(VueMsgType.success, "", list.Json());

        }

        /// <summary>
        /// 获取供应商详情数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetSupplierDetail(string id)
        {
            id = id.InputText();
            var supplier = Client.Current.MySupplier[id];
            var data = new
            {
                supplier.ID,
                Name = supplier.RealEnterpriseName,
                supplier.ChineseName,
                supplier.EnglishName,
                supplier.nGrade,
                supplier.RegAddress,
                Place = string.IsNullOrWhiteSpace(supplier.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), supplier.Place)).GetDescription(),
                Banks = supplier.MySupplierPayees.ToArray().Select(item => new
                {
                    item.ID,
                    RealName = item.Contact,
                    item.Account,
                    Place = string.IsNullOrWhiteSpace(item.Place) ? "" : ((Origin)Enum.Parse(typeof(Origin), item.Place)).GetDescription(),
                    item.Bank,
                    item.BankAddress,
                    item.SwiftCode,
                }).ToArray(),
                Contacts = supplier.MySupplierContact.ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.Tel,
                    item.Mobile,
                    item.QQ,
                    item.Email,
                    Status = item.Status.GetDescription(),
                }).ToArray(),
                Address = supplier.MySupplierAddress.ToArray().Select(item => new
                {
                    item.ID,
                    item.Address,
                    item.Contact,
                    item.Mobile,
                    item.Tel,
                    item.PostZip,
                    item.Email,
                }).ToArray(),
            };
            return JsonResult(VueMsgType.success, "", data.Json());
        }
        #endregion


        #region  外部调用
        /// <summary>
        /// 获取供应商收货地址
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetSupplierAddress(string supplier)
        {
            string supplierID = supplier.InputText();
            var ids = supplierID.Split(',');
            var Addresses = new Yahv.PvWsOrder.Services.ClientViews.MySupplierConsignors(Client.Current.EnterpriseID, ids)
                .ToArray().OrderByDescending(item => item.IsDefault).Select(item => new
                {
                    value = item.ID,
                    text = "联系人:" + item.Contact + "    电话:" + item.Mobile + "    地址:" + item.Address,
                }).Json();
            return JsonResult(VueMsgType.success, "", Addresses);
        }

        /// <summary>
        /// 获取收货地址
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetConsigneeAddress()
        {
            var addresses = Client.Current.MyConsignees.ToList().OrderByDescending(item => item.CreateDate).Select(item => new { value = item.ID, text = item.Title, address = item.Address, name = item.Name, mobile = item.Mobile, place = item.Place }).ToArray();
            return JsonResult(VueMsgType.success, "", addresses.Json());
        }
        #endregion


        #region 新增修改删除

        public JsonResult SupplierSubmittest()
        {
            var bank = new ShencLibrary.SyncBeneficiary()
            {
                RealName = null,
                Bank = null,
                BankAddress = null,
                Account = null,
                SwiftCode = null,
                Methord = Methord.Transfer,
                Currency = (Currency)1,
                Place = Origin.CHN.GetOrigin().Code,
                IsDefault = false,
            };
            //供应商银行账户持久化
            new ShencLibrary.DccBeneficiary().Enter("A434BE582059F6C1624549B6231A1502", "nSupplier00021", bank);

            return JsonResult(VueMsgType.success, "新增成功");
        }

        /// <summary>
        /// 新增供应商提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult SupplierSubmit(AddSupplierViewModel model)
        {
            var current = Client.Current;
            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            var supplier = new ShencLibrary.SyncSupplier()
            {
                Chinesename = model.ChineseName?.Trim(),
                EnglishName = PinyinHelper.SBCToDBC(regex.Replace(model.EnglishName, " ").Trim()),
                CHNabbreviation = model.Name,
                Place = ((Origin)int.Parse(model.Place)).GetOrigin().Code,
                Grade = SupplierGrade.Second, //(SupplierGrade)(model.nGrade ?? 9),
                RegAddress = model.RegAddress,
            };

            //如果没填供应商中文名称，则给中文名称填上英文名称
            if (string.IsNullOrEmpty(supplier.Chinesename))
            {
                supplier.Chinesename = supplier.EnglishName;
            }

            //供应商持久化
            var supplierid = new ShencLibrary.DccSupplier().Enter(current.EnterpriseID, supplier);

            var nsupplier = current.MySupplier[supplierid];
            //银行信息
            if (model.IsBank)
            {
                var bank = new ShencLibrary.SyncBeneficiary()
                {
                    RealName = model.RealName,
                    Bank = model.Bank,
                    BankAddress = model.BankAddress,
                    Account = model.Account,
                    SwiftCode = model.SwiftCode,
                    Methord = Methord.Transfer,
                    Currency = Currency.Unknown, //(Currency)int.Parse(model.BankCurrency),
                    Place = ((Origin)int.Parse(model.Place)).GetOrigin().Code,
                    IsDefault = false,
                };
                //供应商银行账户持久化
                new ShencLibrary.DccBeneficiary().Enter(current.EnterpriseID, supplierid, bank);
            }
            //联系人
            if (model.IsContact)
            {
                var contact = new Yahv.Services.Models.wsnContact
                {
                    nSupplierID = nsupplier.ID,
                    OwnID = current.EnterpriseID,
                    RealID = nsupplier.RealEnterpriseID,
                    Name = model.ContactName,
                    Tel = model.ContactTel,
                    Mobile = model.ContactMobile,
                    Email = model.ContactEmail,
                    QQ = model.ContactQQ,
                    Status = GeneralStatus.Normal,
                    CreaterID = current.ID,
                };
                contact.Enter();
            }
            //提货地址
            if (model.IsAddress)
            {
                var supplieraddress = new ShencLibrary.SyncConsignor()
                {
                    Address = string.Join(" ", model.Land.Concat(new string[] { model.DetailAddress.Trim() })),
                    Name = model.AddressName,
                    Tel = model.AddressTel,
                    Mobile = model.AddressMobile,
                    IsDefault = true,
                    Email = model.AddressEmail,
                };
                //供应商提货地址持久化
                new ShencLibrary.DccConsignor().Enter(current.EnterpriseID, supplierid, supplieraddress);
            }
            return JsonResult(VueMsgType.success, "新增成功");
        }


        /// <summary>
        /// POST: 新增修改供应商
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult SupplierInfoSubmit(SupplierInfoViewModel model)
        {
            var current = Client.Current;

            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            var supplier = new ShencLibrary.SyncSupplier()
            {
                Chinesename = model.ChineseName?.Trim(),
                EnglishName = PinyinHelper.SBCToDBC(regex.Replace(model.EnglishName, " ").Trim()),
                CHNabbreviation = model.ChineseSName,
                Place = ((Origin)int.Parse(model.Place)).GetOrigin().Code,
                Grade = SupplierGrade.Second,
                Corporation = model.Corporation,
                Uscc = model.Uscc,
                RegAddress = model.RegAddress,
            };

            //如果没填供应商中文名称，则给中文名称填上英文名称
            if (string.IsNullOrEmpty(supplier.Chinesename))
            {
                supplier.Chinesename = supplier.EnglishName;
            }

            //供应商持久化
            var id = new ShencLibrary.DccSupplier().Enter(current.EnterpriseID, supplier);
            return JsonResult(VueMsgType.success, "保存成功", id);
        }

        /// <summary>
        /// 获取供应商数据源
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetSupplierOptions()
        {
            var options = Client.Current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).Json();
            return JsonResult(VueMsgType.success, "", options);
        }

        /// <summary>
        /// POST: 新增修改供应商银行
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult SupplierBankInfoSubmit(BeneficiarieInfoViewModel data)
        {
            try
            {
                var current = Client.Current;

                //根据 data.RealName 和 Client.Current.EnterpriseID(CRM的ClientID) 查询是否有这个供应商
                //如果没有则新增一个供应商, 并得到供应商的 ID
                //data.SupplierID 不使用
                if (!string.IsNullOrEmpty(data.RealName))
                {
                    data.RealName = data.RealName.Trim();
                }

                string theSupplierID = string.Empty;
                var existSupplier = current.MySupplier.Where(t => t.EnglishName == data.RealName && t.Status != GeneralStatus.Deleted).FirstOrDefault();
                if (existSupplier != null)
                {
                    theSupplierID = existSupplier.ID;
                }
                else
                {
                    Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                    var supplier = new ShencLibrary.SyncSupplier()
                    {
                        Chinesename = null,
                        EnglishName = PinyinHelper.SBCToDBC(regex.Replace(data.RealName, " ").Trim()),
                        CHNabbreviation = null,
                        Place = ((Origin)int.Parse(data.Place)).GetOrigin().Code,
                        Grade = SupplierGrade.Second,
                        RegAddress = null,
                    };
                    //供应商持久化
                    theSupplierID = new ShencLibrary.DccSupplier().Enter(current.EnterpriseID, supplier);
                }

                var bank = new ShencLibrary.SyncBeneficiary()
                {
                    RealName = data.RealName,
                    Bank = data.Bank,
                    BankAddress = data.BankAddress,
                    Account = data.Account,
                    SwiftCode = data.SwiftCode,
                    Methord = Methord.Transfer,
                    Contact = data.RealName,
                    Currency = Currency.Unknown, //(Currency)int.Parse(data.Currency),
                    Place = ((Origin)int.Parse(data.Place)).ToString(),
                    IsDefault = false,
                };
                //供应商银行账户持久化
                var id = new ShencLibrary.DccBeneficiary().Enter(Client.Current.EnterpriseID, theSupplierID, bank, data.ID);
                return JsonResult(VueMsgType.success, "操作成功", id);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// POST: 新增修改供应商提货地址
        /// </summary>
        /// <returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult SupplierAddressInfoSubmit(SupplierAddressesViewModel data)
        {
            try
            {
                var user = Client.Current;
                var supplieraddress = new ShencLibrary.SyncConsignor()
                {
                    Address = data.Address,
                    Postzip = data.Postzip ?? string.Empty,
                    Name = data.Name,
                    Tel = data.Tel,
                    Mobile = data.Mobile ?? string.Empty,
                    IsDefault = data.IsDefault,
                    Email = data.Email,
                };

                supplieraddress.Mobile = supplieraddress.Tel;

                //供应商提货地址持久化
                var id = new ShencLibrary.DccConsignor().Enter(user.EnterpriseID, data.SupplierID, supplieraddress, data.ID);
                return JsonResult(VueMsgType.success, "操作成功", id);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// POST: 新增修改供应商联系人
        /// </summary>
        /// <returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult SupplierContactInfoSubmit(SupplierContactModel data)
        {
            try
            {
                var user = Client.Current;
                var supplier = user.MySupplier[data.SupplierID];
                var contact = new Yahv.Services.Models.wsnContact
                {
                    ID = data.ID,
                    nSupplierID = supplier.ID,
                    OwnID = user.EnterpriseID,
                    RealID = supplier.RealEnterpriseID,
                    Name = data.Name,
                    Tel = data.Tel,
                    Mobile = data.Mobile,
                    Email = data.Email,
                    QQ = data.QQ,
                    Status = GeneralStatus.Normal,
                    CreaterID = user.ID,
                };
                contact.Enter();
                return JsonResult(VueMsgType.success, "操作成功", contact.ID);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// POST: 删除供应商
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public ActionResult DelSupplier(string id)
        {
            //供应商删除
            new ShencLibrary.DccSupplier().Abandon(Client.Current.EnterpriseID, id);
            return JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 删除供应商银行账号
        /// </summary>
        /// <param name="bankid">银行账号ID</param>
        /// <param name="supplierid">供应商ID</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult DeleteSupplierBank(string bankid, string supplierid)
        {
            //供应商银行账户删除
            new ShencLibrary.DccBeneficiary().Abandon(Client.Current.EnterpriseID, supplierid, bankid);
            return JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 删除供应商联系人
        /// </summary>
        /// <param name="addressid">提货地址ID</param>
        /// <param name="supplierid">供应商ID</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult DeleteSupplierContact(string contactid, string supplierid)
        {
            //删除供应商联系人
            var contact = new Yahv.Services.Models.wsnContact
            {
                ID = contactid,
                nSupplierID = supplierid,
            };
            contact.Abandon();
            return base.JsonResult(VueMsgType.success, "删除成功");
        }


        /// <summary>
        /// 删除供应商提货地址
        /// </summary>
        /// <param name="addressid">提货地址ID</param>
        /// <param name="supplierid">供应商ID</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult DeleteSupplierAddress(string addressid, string supplierid)
        {
            //供应商提货地址删除
            new ShencLibrary.DccConsignor().Abandon(Client.Current.EnterpriseID, supplierid, addressid);
            return base.JsonResult(VueMsgType.success, "删除成功");
        }
        #endregion


        #region 数据校验
        /// <summary>
        /// 验证供应商英文名是否重复
        /// </summary>
        /// <param name="EnglishName">英文名</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult CheckSupplierEnglishName(string EnglishName, string ID)
        {
            EnglishName = EnglishName.InputText();
            ID = ID.InputText();
            var view = Client.Current.MySupplier;
            if (view.Count(item => item.ID != ID && item.EnglishName == EnglishName) > 0)
            {
                return base.JsonResult(VueMsgType.error, "该供应商名称已存在！");
            }
            return base.JsonResult(VueMsgType.success, "");
        }
        #endregion 


        #region 详情查询
        /// <summary>
        ///  获取供应商银行信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetSupplierBankInfo(string bankid, string SupplierId)
        {
            bankid = bankid.InputText();
            var model = new BeneficiarieInfoViewModel();
            var list = Client.Current.MySupplier[SupplierId].MySupplierPayees[bankid];
            if (list == null)
            {
                return base.JsonResult(VueMsgType.error, "该银行账户不存在！");
            }
            model.ID = list.ID;
            model.SupplierID = list.nSupplierID;
            model.RealName = list.Contact;
            model.Bank = list.Bank;
            model.BankAddress = list.BankAddress;
            model.Account = list.Account;
            model.SwiftCode = list.SwiftCode;
            model.Currency = ((int)list.Currency).ToString();
            model.Place = string.IsNullOrWhiteSpace(list.Place) ? "" : ((int)((Origin)Enum.Parse(typeof(Origin), list.Place))).ToString();
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }


        /// <summary>
        ///  获取供应商联系人
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetSupplierContactInfo(string contactid, string SupplierId)
        {
            contactid = contactid.InputText();
            var model = new SupplierContactModel();
            var list = Client.Current.MySupplier[SupplierId].MySupplierContact[contactid];
            if (list == null)
            {
                return base.JsonResult(VueMsgType.error, "该联系人不存在！");
            }
            model.ID = list.ID;
            model.SupplierID = list.nSupplierID;
            model.Name = list.Name;
            model.Mobile = list.Mobile;
            model.Tel = list.Tel;
            model.Email = list.Email;
            model.QQ = list.QQ;
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        ///  获取供应商提货地址信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetSupplierAddressInfo(string addressid, string SupplierId)
        {
            addressid = addressid.InputText();
            var model = new SupplierAddressesViewModel();
            var list = Client.Current.MySupplier[SupplierId].MySupplierAddress[addressid];
            if (list == null)
            {
                return base.JsonResult(VueMsgType.error, "提货地址不存在！");
            }
            model.ID = list.ID;
            model.SupplierID = list.nSupplierID;
            model.Name = list.Contact;
            model.Mobile = list.Mobile;
            model.Address = list.Address;
            model.Postzip = list.PostZip;
            model.Tel = list.Tel;
            model.Email = list.Email;
            model.IsDefault = list.IsDefault;
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }
        #endregion
        #endregion


        #region 账号安全设置：修改登录名、修改手机、邮箱、密码

        /// <summary>
        /// 账户安全中心
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult SecurityCenter()
        {
            return View();
        }

        /// <summary>
        /// 修改手机
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult Mobile()
        {
            MobileViewModel model = new MobileViewModel();
            if (!string.IsNullOrWhiteSpace(Client.Current.Mobile))
            {
                model.Phone = Regex.Replace(Client.Current.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                model.Phone2 = Client.Current.Mobile;
            }
            return View(model);
        }

        /// <summary>
        /// 提交修改手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult Mobile(MobileViewModel model)
        {

            //if (Session[model.NewPhone] != null && Session[model.NewPhone].ToString() == model.Code)
            var code = GetSession(model.NewPhone);
            if (code != null && code.ToString() == model.Code)
            {
                try
                {
                    Client.Current.ChangeMobile(model.NewPhone);
                }
                catch (Exception e)
                {
                    return base.JsonResult(VueMsgType.error, e.Message);
                }
                return base.JsonResult(VueMsgType.success, "手机号码修改成功");
            }
            return base.JsonResult(VueMsgType.error, "验证码错误");

        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>

        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult SendCode(string phone)
        {
            Random ran = new Random();
            string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            App_Utils.SmsService.Send(phone, string.Format(SmsContents.ChangeMobile, messageCode));
            //Session[phone] = messageCode;  //存入session
            SetSession(phone, messageCode, DateTime.Now.AddMinutes(3));
            return JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 发送手机验证码(忘记密码)
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>

        [UserAuthorize(UserAuthorize = false)]
        public JsonResult SendCode1(string phone)
        {
            Random ran = new Random();
            string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            App_Utils.SmsService.Send(phone, string.Format(SmsContents.ChangePassword, messageCode));
            //Session[phone] = messageCode;  //存入session
            SetSession(phone, messageCode, DateTime.Now.AddMinutes(3));
            return JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 忘记密码检查用户名和手机
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false)]
        public JsonResult CheckUserAndMobile(string phone, string code, string name)
        {
            //if (Session[phone] != null && Session[phone].ToString() == code)
            var codeInSession = GetSession(phone);
            if (codeInSession != null && codeInSession.ToString() == code)
            {
                var user = new Views.UsersAlls().FirstOrDefault(item => item.Mobile == phone && item.UserName == name);
                if (user == null)
                {
                    return base.JsonResult(VueMsgType.error, "用户名或手机号不存在");
                }
                return base.JsonResult(VueMsgType.success, "");
            }
            return base.JsonResult(VueMsgType.error, "验证码错误");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false, MobileLog = true)]
        public JsonResult ResetPassword(string name, string phone, string pwd)
        {
            var user = new Views.UsersAlls().FirstOrDefault(item => item.Mobile == phone && item.UserName == name);
            if (user == null)
            {
                return base.JsonResult(VueMsgType.error, "用户名或密码不存在");
            }
            user.ResetPassWord(pwd);
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 检查手机号码是否重复
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult CheckMobile(string phone)
        {
            phone = phone.InputText();
            var user = Client.Current;
            if (phone == user.Mobile)
            {
                return JsonResult(VueMsgType.error, "手机号码不能与原号码相同");
            }
            if (Alls.Current.AllUsers.UserPhoneIsExist(user.ID, phone))
            {
                return JsonResult(VueMsgType.error, "手机号[" + phone + "]已经绑定了华芯通账户");
            }
            return JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult Email()
        {
            EmailViewModel model = new EmailViewModel();
            if (!string.IsNullOrWhiteSpace(Client.Current.Email))
            {
                model.Email = Regex.Replace(Client.Current.Email, @"\w{3}(?=@\w+?.com)", "****");
                model.Email2 = Client.Current.Email;
            }
            return View(model);
        }

        /// <summary>
        /// 修改邮箱账号发送URL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult Email(EmailViewModel model)
        {
            //if (Session[model.NewEmail] == null || Session[model.NewEmail].ToString() != model.Code)
            var code = GetSession(model.NewEmail);
            if (code == null || code.ToString() != model.Code)
            {
                return JsonResult(VueMsgType.error, "验证码错误");
            }
            try
            {
                Client.Current.ChangeEmail(model.NewEmail);
            }
            catch (Exception e)
            {
                return JsonResult(VueMsgType.error, e.Message);
            }
            return JsonResult(VueMsgType.success, "邮箱绑定成功");
        }

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult CheckEmail(string email)
        {
            email = email.InputText();
            var user = Client.Current;
            if (email == user.Email)
            {
                return base.JsonResult(VueMsgType.error, "邮箱账号不能与原邮箱账号相同");
            }
            if (Alls.Current.AllUsers.UserEmailIsExist(user.ID, email))
            {
                return JsonResult(VueMsgType.error, "邮箱账号[" + email + "]已经绑定了华芯通账户");
            }
            return JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult SendMail(string email)
        {
            email = email.InputText();
            var ran = new Random();
            var messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            //Session[email] = messageCode;  //存到Session里
            SetSession(email, messageCode, DateTime.Now.AddMinutes(3));
            //string picUrl = "http://wl.net.cn/images/logo.png";  //logo的地址
            string picUrl = "../Content/img/logo.png";  //logo的地址
            //邮件内容
            StringBuilder sb = new StringBuilder();
            sb.Append("<div  style='WIDTH: 700px; PADDING-BOTTOM: 50px'>");
            sb.Append("	<div  style='HEIGHT: 40px; PADDING-LEFT: 30px'>");
            sb.AppendFormat("	<h2><img  src='{0}'></h2></div>", picUrl);
            sb.Append("	<div  style=' border: 1px solid #a7c5e2;PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 100px; PADDING-RIGHT: 55px'>");
            sb.Append("<div style='MARGIN-TOP: 25px; FONT: bold 16px/40px arial'>请查收您的验证码，完成邮箱绑定！ <span style='COLOR: #cccccc'>(请在30分钟内完成)：</span> </div>");
            sb.AppendFormat("	<div style='Color:red;HEIGHT: 50px; WIDTH: 170px; TEXT-ALIGN: center; FONT: bold 18px/36px arial; MARGIN: 25px 0px 0px 140px'>验证码：{0}</div>", messageCode);
            sb.AppendFormat("</div></div>");

            SmtpContext.Current.Send(email, "华芯通绑定邮箱服务", sb.ToString());  //发送邮件
            return JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult Password()
        {
            MobileViewModel model = new MobileViewModel();
            return View(model);
        }

        /// <summary>
        /// 修改密码保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult Password(PasswordViewModel model)
        {
            if (model.Password == model.NewPassword)
            {
                return JsonResult(VueMsgType.error, "新密码不能与原密码相同");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return JsonResult(VueMsgType.error, "两次密码输入不同");
            }
            try
            {
                Client.Current.ChangePassWord(model.Password, model.NewPassword);
            }
            catch (Exception e)
            {
                return JsonResult(VueMsgType.error, e.Message);
            }
            return JsonResult(VueMsgType.success, "密码修改成功！");
        }
        #endregion

        #region 消息设置

        /// <summary>
        /// 获取消息开关状态
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMsgStatus()
        {
            var current = Client.Current;
            var isMsgOpen = current.MyMsgConfig.GetMsgStatus();

            var data = new
            {
                MsgStatus = isMsgOpen ? "1" : "0",
            };

            return JsonResult(VueMsgType.success, "", data.Json());
        }

        /// <summary>
        /// 设置消息开关状态
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, MobileLog = true)]
        public JsonResult SetMsgStatus()
        {
            try
            {
                var msgStatus = Request.Form["msgStatus"];
                var msgStatusOn = msgStatus == "1";

                var current = Client.Current;
                current.MyMsgConfig.SetMsgStatus(msgStatusOn);

                return JsonResult(VueMsgType.success, "");
            }
            catch (Exception e)
            {
                return JsonResult(VueMsgType.error, "发生错误", e.Json());
            }
        }

        #endregion


        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult StorageInfo()
        {  
            return View();  
        }
    }
}