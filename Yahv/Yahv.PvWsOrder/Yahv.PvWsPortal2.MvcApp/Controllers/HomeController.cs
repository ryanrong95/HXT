using Layers.Data.Sqls.PvData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.Views.Rolls.Document;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.PvWsPortal.Services;
using Yahv.PvWsPortal.Services.Models;
using Yahv.PvWsPortal.Services.Views;
using Yahv.PvWsPortal2.MvcApp.App_Utils;
using Yahv.Underly;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsPortal2.MvcApp.Controllers
{
    public class HomeController : BaseController
    {
        string DomainClient = ConfigurationManager.AppSettings["DomainClient"];

        public ActionResult Index(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        /// <summary>
        /// 获取实时汇率数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetRateData()
        {
            try
            {
                var xdt2 = ConfigurationManager.AppSettings["xdt2"];
                if (string.IsNullOrEmpty(xdt2))
                {
                    var rateList = new List<object>();
                    var ferobocs = new FerobocView().ToArray(); //查询外汇牌价
                    string tenAmChineseBank = ExchangeType.TenAmChineseBank.ToString(); //10点汇率
                    string tenAmDesc = ((ExchangeType)Enum.Parse(typeof(ExchangeType), tenAmChineseBank)).GetDescription();
                    var tenAmUSDRate = ferobocs.FirstOrDefault(item => item.Type == tenAmChineseBank && item.Currency == (int)Currency.USD); //美元的上午10点汇率
                    if (tenAmUSDRate != null)
                    {
                        bool isPublished = tenAmUSDRate.PublishDate >= DateTime.Today.AddHours(10); //10点汇率是否已经发布
                        if (isPublished)
                        {
                            rateList.Add(new
                            {
                                ID = tenAmUSDRate.ID,
                                Type = tenAmDesc,
                                Currency = ((Currency)tenAmUSDRate.Currency).GetDescription(),
                                Xhmr = tenAmUSDRate.Xhmr.Value.ToRound(2),
                                Xcmr = tenAmUSDRate.Xcmr.Value.ToRound(2),
                                Xhmc = tenAmUSDRate.Xhmc.Value.ToRound(2),
                                Xcmc = tenAmUSDRate.Xcmc.Value.ToRound(2),
                                Zhzsj = tenAmUSDRate.Zhzsj.Value.ToRound(2),
                                PublishDate = tenAmUSDRate.PublishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            });
                        }
                        else
                        {
                            rateList.Add(new
                            {
                                ID = tenAmUSDRate.ID,
                                Type = tenAmDesc,
                                Currency = ((Currency)tenAmUSDRate.Currency).GetDescription(),
                                Xhmr = "--",
                                Xcmr = "--",
                                Xhmc = "--",
                                Xcmc = "--",
                                Zhzsj = "--",
                                PublishDate = "待发布",
                            });
                        }
                    }

                    string floating = ExchangeType.Floating.ToString(); //实时汇率
                    string floatingDesc = ((ExchangeType)Enum.Parse(typeof(ExchangeType), floating)).GetDescription();
                    var floatingRates = ferobocs.Where(item => item.Type == floating).Select(item => new
                    {
                        ID = item.ID,
                        Type = floatingDesc,
                        Currency = ((Currency)item.Currency).GetDescription(),
                        Xhmr = item.Xhmr.Value.ToRound(2),
                        Xcmr = item.Xcmr.Value.ToRound(2),
                        Xhmc = item.Xhmc.Value.ToRound(2),
                        Xcmc = item.Xcmc.Value.ToRound(2),
                        Zhzsj = item.Zhzsj.Value.ToRound(2),
                        PublishDate = item.PublishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    }).Take(7).ToList();

                    rateList.AddRange(floatingRates);

                    return base.JsonResult(VueMsgType.success, "", rateList.Json());
                }
                else
                {
                    var xdt2url = ConfigurationManager.AppSettings["xdt2url"];
                    var rateList = new List<object>();

                    string tenAmChineseBank = ExchangeType.TenAmChineseBank.ToString(); //10点汇率
                    string tenAmDesc = ((ExchangeType)Enum.Parse(typeof(ExchangeType), tenAmChineseBank)).GetDescription();

                    var chinaRateTenClock = RequestHelper.PostUrl<Xdt2Response<BankChinaRateForClientVO>>(xdt2url + "/system/bankChinaRate/bankChinaRateTenClockForClient", new {}.Json());
                    if (chinaRateTenClock != null && chinaRateTenClock.success && chinaRateTenClock.data != null)
                    {
                        rateList.Add(new
                        {
                            ID = "USD10",
                            Type = tenAmDesc,
                            Currency = Currency.USD.GetDescription(),
                            Xhmr = chinaRateTenClock.data.buyingRate.ToRound(2), //tenAmUSDRate.Xhmr.Value.ToRound(2),
                            Xcmr = chinaRateTenClock.data.cashPurchase.ToRound(2), //tenAmUSDRate.Xcmr.Value.ToRound(2),
                            Xhmc = chinaRateTenClock.data.spotSelling.ToRound(2), //tenAmUSDRate.Xhmc.Value.ToRound(2),
                            Xcmc = chinaRateTenClock.data.cashSelling.ToRound(2), //tenAmUSDRate.Xcmc.Value.ToRound(2),
                            Zhzsj = chinaRateTenClock.data.bocConversion.ToRound(2), //tenAmUSDRate.Zhzsj.Value.ToRound(2),
                            PublishDate = chinaRateTenClock.data.publishDate.ToString("yyyy-MM-dd HH:mm:ss"), //tenAmUSDRate.PublishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        });
                    }
                    else
                    {
                        rateList.Add(new
                        {
                            ID = "USD10",
                            Type = tenAmDesc,
                            Currency = Currency.USD.GetDescription(),
                            Xhmr = "--",
                            Xcmr = "--",
                            Xhmc = "--",
                            Xcmc = "--",
                            Zhzsj = "--",
                            PublishDate = "待发布",
                        });
                    }

                    var chinaRateList = RequestHelper.PostUrl<Xdt2Response<List<BankChinaRateForClientVO>>>(xdt2url + "/system/bankChinaRate/bankChinaRateForClient", new { }.Json());
                    if (chinaRateList != null && chinaRateList.success && chinaRateList.data != null && chinaRateList.data.Any())
                    {
                        string floating = ExchangeType.Floating.ToString(); //实时汇率
                        string floatingDesc = ((ExchangeType)Enum.Parse(typeof(ExchangeType), floating)).GetDescription();
                        var floatingRates = chinaRateList.data.Select(item => new
                        {
                            ID = item.currencyId,
                            Type = floatingDesc,
                            Currency = item.currencyName, //((Currency)item.Currency).GetDescription(),
                            Xhmr = item.buyingRate.ToRound(2), //item.Xhmr.Value.ToRound(2),
                            Xcmr = item.cashPurchase.ToRound(2), //item.Xcmr.Value.ToRound(2),
                            Xhmc = item.spotSelling.ToRound(2), //item.Xhmc.Value.ToRound(2),
                            Xcmc = item.cashSelling.ToRound(2), //item.Xcmc.Value.ToRound(2),
                            Zhzsj = item.bocConversion.ToRound(2), //item.Zhzsj.Value.ToRound(2),
                            PublishDate = item.publishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        }).ToList();

                        rateList.AddRange(floatingRates);
                    }

                    return base.JsonResult(VueMsgType.success, "", rateList.Json());
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "查询失败");
            }
        }

        /// <summary>
        /// 获取汇率
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRate()
        {
            var rUSD = Yahv.Payments.ExchangeRates.Current[ExchangeType.TenAmChineseBank][Currency.USD, Currency.CNY]; //上午10点后的第一个美金实时汇率
            var cUSD = new CustomExchangeRatesView().FindByCode("USD")?.Rate; //美金海关汇率
            var data = new
            {
                rUSD,
                cUSD
            };
            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        /// <summary>
        /// 归类查询
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult QueryClassfiy(string query)
        {
            query = query.InputText();

            var xdt2 = ConfigurationManager.AppSettings["xdt2"];
            if (string.IsNullOrEmpty(xdt2))
            {
                var classify = new ClassifiedProductViews().Where(item => item.PartNumber.Contains(query)).Take(5).ToArray();

                var data = classify.Select(item => new
                {
                    item.ID,
                    item.HSCode,
                    item.Name,
                    item.PartNumber,
                    LegalUnit1 = item.LegalUnit1 != null ? (item.LegalUnit1.ToLegalUnit()).GetDescription() : string.Empty,
                    LegalUnit2 = item.LegalUnit2 != null ? (item.LegalUnit2.ToLegalUnit()).GetDescription() : string.Empty,
                    item.SupervisionRequirements,
                    item.CIQCode,
                    item.ImportPreferentialTaxRate,
                    Expansion = false,
                    item.ImportGeneralTaxRate,
                    item.VATRate,
                    item.ExciseTaxRate,
                    Ccc = item.Ccc ? "是" : "否",
                    item.Elements,
                });
                SearchCount = SearchCount + 1;
                if (Yahv.Client.Current != null)
                {
                    SearchCount = 0;
                }
                return base.JsonResult(VueMsgType.success, "", new { data = data, count = SearchCount }.Json());
            }
            else
            {
                var xdt2url = ConfigurationManager.AppSettings["xdt2url"];
                //List<KeyValuePair<string, string>> queryData = new List<KeyValuePair<string, string>>();
                //queryData.Add(new KeyValuePair<string, string>("model", query));

                var classify = RequestHelper.PostUrl<Xdt2Response<List<StandardClassifyMaterialVO>>>(xdt2url + "/scm/standardClassifyMaterial/getDatasNoToken", new { model = query, }.Json());

                if (classify != null && classify.success && classify.data != null && classify.data.Any())
                {
                    var data = classify.data.Select(item => new
                    {
                        ID = item.id,
                        HSCode = item.hsCode,
                        Name = item.name,
                        PartNumber = item.model,
                        LegalUnit1 = item.unit1 != null ? (item.unit1.ToLegalUnit()).GetDescription() : string.Empty,
                        LegalUnit2 = item.unit2 != null ? (item.unit2.ToLegalUnit()).GetDescription() : string.Empty,
                        SupervisionRequirements = item.superRequirements,
                        CIQCode = item.ciqCode,
                        ImportPreferentialTaxRate = item.preferentialRate,
                        Expansion = false,
                        ImportGeneralTaxRate = item.importGeneralTaxRate,
                        VATRate = item.addedTaxRate,
                        ExciseTaxRate = item.exciseTaxRate,
                        Ccc = item.ccc ? "是" : "否",
                        Elements = item.elements,
                    });
                    SearchCount = SearchCount + 1;
                    if (Yahv.Client.Current != null)
                    {
                        SearchCount = 0;
                    }
                    return base.JsonResult(VueMsgType.success, "", new { data = data, count = SearchCount }.Json());
                }
                else
                {
                    return base.JsonResult(VueMsgType.success, "", new { data = new List<object>(), count = SearchCount }.Json());
                }
            }
        }

        /// <summary>
        /// 归类查询
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult QueryClassfiyByType(string query)
        {
            query = query.InputText();
            var detail = new ClassifiedProductViews().GetDetailByType(query);
            var obj = new
            {
                detail?.HSCode,
                detail?.ImportPreferentialTaxRate, //关税
                detail?.ImportGeneralTaxRate,  //普通税率
                detail?.VATRate,  //增值税
                detail?.ExciseTaxRate, //消费税率
                Ccc = detail == null ? "" : detail.Ccc ? "是" : "否",
                detail?.Name,
                CIQ = detail == null ? "" : detail.CIQ ? "是" : "否",
                Embargo = detail == null ? "" : detail.Embargo ? "是" : "否",
            };
            SearchCount = SearchCount + 1;
            if (Yahv.Client.Current != null)
            {
                SearchCount = 0;
            }
            return base.JsonResult(VueMsgType.success, "", new { data = obj, count = SearchCount }.Json());
        }

        /// <summary>
        /// 归类详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult QueryClassfiyDetail(string id)
        {
            id = id.InputText();
            if (string.IsNullOrWhiteSpace(id))
            {
                return base.JsonResult(VueMsgType.error, "");
            }
            var detail = new ClassifiedProductViews().GetDetailByID(id);
            var obj = new
            {
                detail.HSCode,
                detail.ImportPreferentialTaxRate,
                detail.ImportGeneralTaxRate,
                detail.VATRate,
                detail.ExciseTaxRate,
                Ccc = detail.Ccc ? "是" : "否",
                detail.CIQCode,
                detail.Name,
            };
            return base.JsonResult(VueMsgType.success, "", obj.Json());
        }

        /// <summary>
        /// 保存建议
        /// </summary>
        /// <returns></returns>
        public JsonResult SuggestionSubmit(string Name, string City, string Contact, string Suggestion)
        {
            Suggestion suggestion = new Suggestion()
            {
                Name = Name.InputText(),
                Place = City.InputText(),
                Phone = Contact.InputText(),
                Summary = Suggestion.InputText(),
            };
            suggestion.Enter();

            return base.JsonResult(VueMsgType.success, "", "保存成功");
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public JsonResult Login(string UserName, string Password, bool IsRemember)
        {
            Yahv.PvWsClient.Model.ClientUser User = new Yahv.PvWsClient.Model.ClientUser()
            {
                UserName = UserName.InputText(),
                Password = Password.InputText(),
                IsRemeber = IsRemember,
            };

            if (Yahv.Client.Current != null)
            {
                User = Yahv.Client.Current;
                User.IsRemeber = IsRemember;
            }
            try
            {
                User.Login();
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.warning, ex.Message);
            }

            return base.JsonResult(VueMsgType.success, string.Empty, new { userurl = DomainClient, }.Json());
        }


        public JsonResult LoginOut()
        {
            //退出登录
            Yahv.Client.Current.LoginOut();
            return base.JsonResult(VueMsgType.success, string.Empty);
        }

        /// <summary>
        /// 注册申请
        /// </summary>
        /// <returns></returns>
        public JsonResult RegisterApply2(string User, string Password, string Phone, string Code)
        {
            var xdt2 = ConfigurationManager.AppSettings["xdt2"];
            if (string.IsNullOrEmpty(xdt2))
            {
                var data = new
                {
                    CustomsCode = "",
                    Enterprise = new
                    {
                        Name = "reg-" + User,
                        Uscc = "",
                        Corporation = "",
                        RegAddress = "",
                    },
                    SiteUser = new
                    {
                        UserName = User,
                        PassWord = Password,
                        RealName = User,
                        Mobile = Phone,
                    },
                };
                Yahv.PvWsPortal.Services.CrmNoticeExtends.ClientInfo(data.Json());
                return base.JsonResult(VueMsgType.success, "", "保存成功");
            }
            else
            {
                try
                {
                    var xdt2url = ConfigurationManager.AppSettings["xdt2url"];
                    RequestHelper.PostUrl<Xdt2Response<OfficialRegisterVO>>(xdt2url + "/scm/client/person/officialRegister", new
                    {
                        userName = User,
                        phoneNo = Phone,
                        validateCode = Code,
                        passWord = Password,
                    }.Json());
                    return base.JsonResult(VueMsgType.success, "", "保存成功");
                }
                catch (Exception e)
                {
                    return base.JsonResult(VueMsgType.error, "发生错误");
                }
            }
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public JsonResult SendCode(string phone)
        {
            var xdt2 = ConfigurationManager.AppSettings["xdt2"];
            if (string.IsNullOrEmpty(xdt2))
            {
                Random ran = new Random();
                string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
                App_Utils.SmsService.Send(phone, string.Format(SmsContents.Register, messageCode));
                Session[phone] = messageCode;  //存入session
                return JsonResult(VueMsgType.success, "提交成功", messageCode);
            }
            else
            {
                try
                {
                    var xdt2url = ConfigurationManager.AppSettings["xdt2url"];
                    RequestHelper.PostUrl<Xdt2Response<SendVerificationCodeVO>>(xdt2url + "/scm/auth/sendVerificationCode", new
                    {
                        phoneNo = phone,
                    }.Json());
                    return JsonResult(VueMsgType.success, "提交成功", "88888888");
                }
                catch (Exception e)
                {
                    return base.JsonResult(VueMsgType.error, "发生错误");
                }
            }
        }

        /// <summary>
        /// 获取当前登录名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetName()
        {
            var current = Yahv.Client.Current;

            return base.JsonResult(VueMsgType.success, "", current?.UserName);
        }


        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public JsonResult GetInfo(string token)
        {
            string clientRealName = string.Empty;

            var clientUser = new Yahv.PvWsPortal.Services.Views.ClientUserView().GetByToken(token);
            if (clientUser != null)
            {
                clientRealName = clientUser.Name;
            }

            return base.JsonResult(VueMsgType.success, Client.Current.MyClients?.Name, clientRealName);
        }


        #region 验证企业名称和用户名

        /// <summary>
        /// 检查企业名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CheckCompanyName(string name)
        {
            name = name.InputText();

            if (Alls.Current.WsClients.Any(item => item.Name == name))
            {
                return base.JsonResult(VueMsgType.error, "企业[" + name + "]已注册，不可重复注册!");
            }
            return base.JsonResult(VueMsgType.success, "");
        }


        /// <summary>
        /// 检查登录名是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CheckUserName(string name)
        {
            name = name.InputText();

            var xdt2 = ConfigurationManager.AppSettings["xdt2"];
            if (string.IsNullOrEmpty(xdt2))
            {
                if (Yahv.Alls.Current.AllUsers.Any(item => item.UserName == name))
                {
                    return base.JsonResult(VueMsgType.error, "用户名[" + name + "]已注册，不可使用!");
                }
                return base.JsonResult(VueMsgType.success, "");
            }
            else
            {
                try
                {
                    var xdt2url = ConfigurationManager.AppSettings["xdt2url"];
                    var checkResult = RequestHelper.PostUrl<Xdt2Response<OfficialRegisterCheckVO>>(xdt2url + "/scm/client/person/officialRegisterCheck",
                    new
                    {
                        checkType = "login_account",
                        checkContent = name,
                    }.Json());

                    if (checkResult == null || !checkResult.success || checkResult.data == null)
                    {
                        return base.JsonResult(VueMsgType.error, "发生错误2");
                    }

                    if (checkResult.data.isDuplicate)
                    {
                        return base.JsonResult(VueMsgType.error, "用户名[" + name + "]已注册，不可使用!");
                    }
                    return base.JsonResult(VueMsgType.success, "");
                }
                catch (Exception e)
                {
                    return base.JsonResult(VueMsgType.error, "发生错误1");
                }
            }
        }
        #endregion


        #region 获取所有新闻数据
        /// <summary>
        /// 获取新闻数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDocuments(int page, int rows, string catalogID)
        {
            var documents = new vDocumentsRoll().Where(item => true);
            if (!string.IsNullOrWhiteSpace(catalogID))
            {
                documents = documents.Where(item => item.CatalogID == catalogID);
            }

            int total = documents.Count();
            var data = documents.Skip(rows * (page - 1)).Take(rows).ToArray().
                OrderByDescending(item => item.CreateDate).Select(item => new
                {
                    ID = item.ID,
                    title = item.Title,
                    createDate = item.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    catalog = item.CatalogName,
                    IsShow = false,
                    noticeText = item.Context.Substring(0, item.Context.IndexOf("</p>") + 4),
                    navStatus = item.CatalogID,
                    content = item.Context,
                });

            return base.JsonResult(VueMsgType.success, "", new { list = data.ToArray(), total }.Json());
        }


        /// <summary>
        /// 获取新闻数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDocumentDetail(string id)
        {
            var document = new vDocumentsRoll()[id];

            var data = new
            {
                document.Title,
                document.CatalogName,
                document.Context,
                createDate = document.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
            };
            return base.JsonResult(VueMsgType.success, "", data.Json());
        }
        #endregion

        /// <summary>
        /// 读取配置的会员中心域名信息（html页面中调用）
        /// </summary>
        /// <returns></returns>
        public string GetDomainClient()
        {
            return DomainClient;
        }

    }
}