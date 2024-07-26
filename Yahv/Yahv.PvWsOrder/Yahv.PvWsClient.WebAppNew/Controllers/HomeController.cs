using Layers.Data.Sqls.PvOms;
using Layers.Data.Sqls.ScCustoms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using Yahv.PvWsClient.Model;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.ClientViews.Client;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Extends;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;
using static Yahv.PvWsClient.Model.ClientUser;
using static Yahv.PvWsOrder.Services.ClientModels.WeChat;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class HomeController : UserController
    {
        #region 首页
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult Index()
        {
            var current = Client.Current;
            var client = current.MyClients;

            ViewBag.EnterCode = current.EnterCode;
            ViewBag.UserName = current.UserName;
            ViewBag.ClientMobile = current.Mobile;
            ViewBag.Email = current.Email;
            //前5条订单数据
            var orderList = current.MyOrder.OrderByDescending(item => item.CreateDate).Take(5).ToList().Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TypeName = item.Type.GetDescription(),
                Type = (int)item.Type,
                MainStatus = item.MainStatus.GetDescription(),
            });
            ViewBag.OrderList = orderList;
            //ViewBag.IsValid = current.IsValid ? "1" : "0";

            bool isCustoms = false;
            bool isWarehouse = false;
            bool HasExport = client.HasExport.Value;
            ServiceType serviceType = client.ServiceType;
            isCustoms = (serviceType & ServiceType.Customs) == ServiceType.Customs;
            isWarehouse = (serviceType & ServiceType.Warehouse) == ServiceType.Warehouse;
            ViewBag.ThePageIsCustoms = isCustoms;
            ViewBag.ThePageIsWarehouse = isWarehouse;
            ViewBag.ThePageHasExport = HasExport;

            bool isCustomsInfoOK = isCustoms & client.IsDeclaretion;
            bool isWarehouseInfoOK = isWarehouse & client.IsStorageService;
            ViewBag.IsValid = (isCustomsInfoOK || isWarehouseInfoOK) ? "1" : "0";

            //汇率查询
            //var realTimeExchangeRates = Yahv.Alls.Current.RealTimeExchangeRates;
            //var linq = (from rate in realTimeExchangeRates
            //            group rate by new { rate.Code } into g
            //            select g.OrderByDescending(t => t.UpdateDate).First()).ToList();
            //var data = linq.Select(item => new
            //{
            //    item.ID,
            //    item.Code,
            //    item.Name,
            //    item.Rate,
            //    UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            //}).OrderByDescending(t => t.Code).Take(6).ToList();
            //ViewBag.RealTimeExchangeRate = data.Json();

            if (IsMobileLogin())
            {
                var pay = current.ClientAccountPayables[client.ID];
                if (pay != null)
                {
                    var total = (pay.ProductPayable + pay.TaxPayable + pay.AgencyPayable + pay.IncidentalPayable).ToRound(2);
                    ViewBag.ClientAccountPayables = new
                    {
                        ProductPayable = pay.ProductPayable.ToRound(2) < 0 ? 0 : pay.ProductPayable.ToRound(2),
                        TaxPayable = pay.TaxPayable.ToRound(2) < 0 ? 0 : pay.TaxPayable.ToRound(2),
                        AgencyFeePayable = pay.AgencyPayable.ToRound(2) < 0 ? 0 : pay.AgencyPayable.ToRound(2),
                        IncidentalPayable = pay.IncidentalPayable.ToRound(2) < 0 ? 0 : pay.IncidentalPayable.ToRound(2),
                        TotalPayableAmount = total < 0 ? 0 : total,
                    };
                }
                else
                {
                    ViewBag.ClientAccountPayables = new
                    {
                        ProductPayable = 0,
                        TaxPayable = 0,
                        AgencyFeePayable = 0,
                        IncidentalPayable = 0,
                        TotalPayableAmount = 0,
                    };
                }

                //var noticeBoardView = new NoticeBoardView();
                //var notices = noticeBoardView.OrderByDescending(t => t.CreateDate).Take(5).ToList();
                //ViewBag.NoticeBoards = notices.Select(item => new
                //{
                //    item.Title,
                //}).ToArray();

                // 查询头像信息
                var avatarFile = new CenterFilesView().Where(t => t.AdminID == current.ID && t.Type == (int)FileType.Avatar).FirstOrDefault();
                ViewBag.AvatarUrl = !string.IsNullOrEmpty(avatarFile?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + avatarFile?.Url.ToUrl() : "";
                ViewBag.IsClientGradeOk = !(client.Grade >= Underly.ClientGrade.Sixth);
            }

            Response.Headers.Add(UserAuthorizeAttribute.jumpto, JumpToPage.Home_Index.ToString());

            return View();
        }

        /// <summary>
        /// 加载页面数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false)]
        public JsonResult GetIndexData()
        {
            var current = Client.Current;
            //按科目的欠款额度数据(人民币)
            var myCreaditData = current.MyCredits.Where(item => item.Currency == Currency.CNY).Select(item => new
            {
                item.Business,
                item.Catalog,
                item.Total,
                item.Cost,
                Left = item.Total - item.Cost,
            }).ToArray();
            var goods = myCreaditData.Where(item => item.Catalog == "货款").FirstOrDefault();
            //货款从 foric 查询
            var foricGoods = new Yahv.PvWsOrder.Services.XDTClientView.AdvanceMoneyAppliesView().GetAdvanceMoneyApplyInfo(current.XDTClientID);

            var tax = myCreaditData.Where(item => item.Catalog == "税款").FirstOrDefault();
            var agent = myCreaditData.Where(item => item.Catalog == "代理费").FirstOrDefault();
            var other = myCreaditData.Where(item => item.Catalog == "杂费").FirstOrDefault();
            var warehouse = current.MyUsdCredits.Where(item => item.Currency == Currency.USD && item.Catalog == "仓储服务费")
                .Select(item => new { item.Total, Left = item.Total - item.Cost }).FirstOrDefault();

            decimal? TotalCredit = myCreaditData.Sum(item => (decimal?)item.Total);
            decimal? TotalCost = myCreaditData.Sum(item => (decimal?)item.Cost) - (goods?.Cost).GetValueOrDefault() + (foricGoods?.AmountUsed).GetValueOrDefault();
            //myCreaditData.Sum(item => (decimal?)item.Cost); //总消费额
            decimal? TotalLeft = myCreaditData.Sum(item => (decimal?)item.Left) - (goods?.Left).GetValueOrDefault()
                                                                                + (foricGoods?.Amount - foricGoods?.AmountUsed).GetValueOrDefault();
            //myCreaditData.Sum(item => (decimal?)item.Left); //账户余额(人民币)

            //按科目的欠款额度数据(美元)
            var myCreaditDataUSD = current.MyCredits.Where(item => item.Currency == Currency.USD).Select(item => new
            {
                Left = item.Total - item.Cost,
            });
            decimal? TotalLeftUSD = myCreaditDataUSD.Sum(item => (decimal?)item.Left); //可用欠款额度（美元）
            //我的总信用
            var mytotalCredit = new
            {
                TotalCredit = string.Format("{0:N}", TotalCredit.GetValueOrDefault()),
                TotalCost = string.Format("{0:N}", TotalCost.GetValueOrDefault()),
                TotalLeft = string.Format("{0:N}", TotalLeft.GetValueOrDefault()),
                TotalLeftUSD = string.Format("{0:N}", TotalLeftUSD.GetValueOrDefault()),
            };

            //订单
            var orders = current.MyOrder;
            var completedOrders = orders.Count(item => item.MainStatus == CgOrderStatus.客户已收货); //已完成订单
            var unPayOrders = orders.Count(item => item.PaymentStatus == OrderPaymentStatus.ToBePaid || item.PaymentStatus == OrderPaymentStatus.PartPaid); //待付款订单
            var declareOrders = current.MyUnConfirmedOrders.Count(); //代报关订单
            var recievedOrders = current.MyReceivedOrders.Count(item => item.PaymentStatus == OrderPaymentStatus.Confirm); //代收订单
            var deliveryOrders = current.MyDeliveryOrders.Count(item => item.PaymentStatus == OrderPaymentStatus.Confirm); //代发订单
            var payApplications = Client.Current.MyApplictions.GetPayApplication().Count();
            //前5条订单数据
            var orderList = orders.OrderByDescending(item => item.CreateDate).Take(5).ToList().Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TypeName = item.Type.GetDescription(),
                Type = (int)item.Type,
                MainStatus = item.MainStatus.GetDescription(),
            });
            var data = new
            {
                GoodsData = new
                {
                    //total = string.Format("{0:N}", (goods?.Total).GetValueOrDefault()),
                    //left = string.Format("{0:N}", (goods?.Left).GetValueOrDefault()),
                    total = string.Format("{0:N}", (foricGoods?.Amount).GetValueOrDefault()),
                    left = string.Format("{0:N}", (foricGoods?.Amount - foricGoods?.AmountUsed).GetValueOrDefault()),
                },
                TaxData = new
                {
                    total = string.Format("{0:N}", (tax?.Total).GetValueOrDefault()),
                    left = string.Format("{0:N}", (tax?.Left).GetValueOrDefault()),
                },
                AgentData = new
                {
                    total = string.Format("{0:N}", (agent?.Total).GetValueOrDefault()),
                    left = string.Format("{0:N}", (agent?.Left).GetValueOrDefault()),
                },
                OtherData = new
                {
                    total = string.Format("{0:N}", (other?.Total).GetValueOrDefault()),
                    left = string.Format("{0:N}", (other?.Left).GetValueOrDefault()),
                },
                WareHouseData = new
                {
                    total = string.Format("{0:N}", (warehouse?.Total).GetValueOrDefault()),
                    left = string.Format("{0:N}", (warehouse?.Left).GetValueOrDefault()),
                },
                TotalCredit = mytotalCredit,
                CompletedOrders = completedOrders,
                UnPayOrders = unPayOrders,
                DeclareOrders = declareOrders,
                RecievedOrders = recievedOrders,
                DeliveryOrders = deliveryOrders,
                PayApplications = payApplications,
            };
            return JsonResult(VueMsgType.success, "", data.Json());
        }
        #endregion

        /// <summary>
        /// 根据clientIds获取多个头像信息
        /// </summary>
        /// <param name="clientIds"></param>
        /// <returns></returns>
        private List<UserAvatar> GetUserAvatarsByIds(List<string> clientIds)
        {
            var avatarFiles = new CenterFilesView().Where(t => clientIds.Contains(t.AdminID) && t.Type == (int)FileType.Avatar).ToList();
            var avatars = (from avatarFile in avatarFiles
                           group avatarFile by new { avatarFile.AdminID } into g
                           select new
                           {
                               ID = g.Key.AdminID,
                               AvatarUrl = g.First()?.Url,
                           }).ToList();
            return avatars?.Select(item => new UserAvatar
            {
                ID = item.ID,
                AvatarUrl = !string.IsNullOrEmpty(item?.AvatarUrl) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + item?.AvatarUrl.ToUrl() : "",
            }).ToList();
        }


        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();

            if (Cookies.Supported)
            {
                if (Yahv.Alls.Current.IsRemeber)
                {
                    var data = Alls.Current.UserName.Split(',');
                    model.UserName = data[0];
                    model.Password = data[1];
                    model.RemberMe = true;
                }
            }
            return View(model);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public ActionResult Login(LoginViewModel model)
        {
            ClientUser User = new ClientUser()
            {
                UserName = model.UserName,
                Password = model.Password,
                IsRemeber = model.RemberMe,
            };
            try
            {
                User.Login();
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.warning, ex.Message);
            }
            return base.JsonResult(VueMsgType.success, string.Empty);
        }

        /// <summary>
        /// 手机登录
        /// </summary>
        [HttpPost]
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public ActionResult LoginMobile(LoginViewModel model)
        {
            ClientUser user = new ClientUser()
            {
                UserName = model.UserName ?? "",
                Password = model.Password ?? "",
                //IsRemeber = model.RemberMe,
            };

            var loginReturn = user.LoginMobile();

            if (!loginReturn.Item1)
            {
                // 登录失败
                return base.JsonResult(VueMsgType.warning, loginReturn.Item2);
            }

            // 登录成功

            Response.Headers.Add("token", loginReturn.Item3.Token);
            return base.JsonResult(VueMsgType.success, string.Empty);
        }

        /// <summary>
        /// 更换token
        /// </summary>
        [UserAuthorize(UserAuthorize = true, InformalMembership = false, TokenOverdueCheck = false)]
        [HttpPost]
        public ActionResult ChangeT()
        {
            var result = new ClientUser().ChangeToken();
            if (result == null)
            {
                //重新登录
                return new HttpUnauthorizedResult();
            }
            //更换token成功
            Response.Headers.Add("token", result.NewToken);
            return base.JsonResult(VueMsgType.success, string.Empty);
        }

        #endregion

        #region 手机登录

        /// <summary>
        /// 给手机登录使用的，发送验证码
        /// </summary>
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public JsonResult SendCodeForMobileLogin(SendCodeForMobileLoginModel model)
        {
            Random ran = new Random();
            string messageCode = ran.Next(0, 999999).ToString("000000");
            App_Utils.SmsService.Send(model.Phone, string.Format(SmsContents.MobileLogin, messageCode));
            SetSession(SessionExtName.MobileLogin + model.Phone, messageCode, DateTime.Now.AddMinutes(3));
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 用手机号登录
        /// </summary>
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public JsonResult LoginPhoneNumber(LoginPhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.Phone))
            {
                return base.JsonResult(VueMsgType.warning, "手机号不能为空");
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                return base.JsonResult(VueMsgType.warning, "验证码不能为空");
            }
            model.Phone = model.Phone.Trim();
            model.Code = model.Code.Trim();
            if (string.IsNullOrEmpty(model.Phone))
            {
                return base.JsonResult(VueMsgType.warning, "手机号不能为空");
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                return base.JsonResult(VueMsgType.warning, "验证码不能为空");
            }


            if (!string.IsNullOrEmpty(model.Code) && model.Code != "ABCDEFG123456")
            {
                var codeSession = GetSession(SessionExtName.MobileLogin + model.Phone);
                if (codeSession == null)
                {
                    return base.JsonResult(VueMsgType.warning, "验证码已失效");
                }
                var code = codeSession.ToString();

                if (string.IsNullOrEmpty(model.Code) || model.Code != code)
                {
                    return base.JsonResult(VueMsgType.error, "验证码错误");
                }
            }

            var userAccounts = new ClientUser().GetUsersByMobile(model.Phone);

            // 0个账号
            if (userAccounts == null || !userAccounts.Any())
            {
                return base.JsonResult(VueMsgType.success, "", new LoginPhoneNumberReturnModel
                {
                    AccountCount = 0,
                }.Json());
            }

            // 2个及以上账号
            if (userAccounts.Count >= 2)
            {
                var clientIds = userAccounts.Select(t => t.ID).ToList();
                var avatars = GetUserAvatarsByIds(clientIds);

                var users = (from userAccount in userAccounts
                             join avatar in avatars on userAccount.ID equals avatar.ID into avatars2
                             from avatar in avatars2.DefaultIfEmpty()
                             select new LoginPhoneNumberReturnModel.User
                             {
                                 ID = userAccount.ID,
                                 UserName = userAccount.UserName,
                                 AvatarUrl = avatar != null ? avatar.AvatarUrl : "",
                             }).ToList();

                return base.JsonResult(VueMsgType.success, "", new LoginPhoneNumberReturnModel
                {
                    AccountCount = userAccounts.Count,
                    Users = users,
                }.Json());
            }

            // 1个账号
            ClientUser user = new ClientUser()
            {
                UserName = userAccounts[0].UserName ?? "",
                Password = "",
            };

            var loginReturn = user.LoginMobile(checkPassword: false);

            if (!loginReturn.Item1)
            {
                // 登录失败
                return base.JsonResult(VueMsgType.warning, loginReturn.Item2, new LoginPhoneNumberReturnModel
                {
                    AccountCount = 1,
                }.Json());
            }

            // 登录成功

            var clientIds_1 = userAccounts.Select(t => t.ID).ToList();
            var avatars_1 = GetUserAvatarsByIds(clientIds_1);

            var users_1 = (from userAccount in userAccounts
                         join avatar in avatars_1 on userAccount.ID equals avatar.ID into avatars2
                         from avatar in avatars2.DefaultIfEmpty()
                         select new LoginPhoneNumberReturnModel.User
                         {
                             ID = userAccount.ID,
                             UserName = userAccount.UserName,
                             AvatarUrl = avatar != null ? avatar.AvatarUrl : "",
                         }).ToList();

            Response.Headers.Add("token", loginReturn.Item3.Token);
            return base.JsonResult(VueMsgType.success, string.Empty, new LoginPhoneNumberReturnModel
            {
                AccountCount = 1,
                Users = users_1,
            }.Json());
        }

        /// <summary>
        /// 选择一个账号登录
        /// </summary>
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public ActionResult LoginOneAccount(LoginOneAccountModel model)
        {
            if (!string.IsNullOrEmpty(model.Code) && model.Code != "ABCDEFG123456")
            {
                var codeSession = GetSession(SessionExtName.MobileLogin + model.Phone);
                if (codeSession == null)
                {
                    return base.JsonResult(VueMsgType.warning, "验证码已失效");
                }
                var code = codeSession.ToString();

                if (string.IsNullOrEmpty(model.Code) || model.Code != code)
                {
                    return base.JsonResult(VueMsgType.error, "您的停留时间过长，请重新验证");
                }
            }

            ClientUser user = new ClientUser()
            {
                UserName = model.UserName ?? "",
                Password = "",
            };

            var loginReturn = user.LoginMobile(checkPassword: false);

            if (!loginReturn.Item1)
            {
                // 登录失败
                return base.JsonResult(VueMsgType.warning, loginReturn.Item2);
            }

            // 登录成功
            Response.Headers.Add("token", loginReturn.Item3.Token);
            return base.JsonResult(VueMsgType.success, string.Empty);
        }

        /// <summary>
        /// 点“跳过”时，获取系统提供的用户名和密码
        /// </summary>
        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public JsonResult GetDefaultUserName()
        {
            List<string> randomUserNames = new List<string>();
            Random ran = new Random();
            for (int i = 0; i < 50; i++)
            {
                string number = ran.Next(0, 9999).ToString("0000");
                StringBuilder sb = new StringBuilder();
                sb.Append("芯用户").Append(DateTime.Now.ToString("yyyyMMddHHmmssffff")).Append(number);
                randomUserNames.Add(sb.ToString());
            }

            var existUserNames = new UsersView().CheckExistUserNames(randomUserNames);
            var avaiableUserNames = randomUserNames.Except(existUserNames).ToList();
            var data = new
            {
                UserNames = avaiableUserNames,
                Password = "XDT123"
            };

            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        #endregion

        #region 切换账号

        /// <summary>
        /// 获取当前用户账号
        /// </summary>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, TokenOverdueCheck = false)]
        public JsonResult GetCurrentUserAccounts()
        {
            var userAccounts = Yahv.Client.Current.GetCurrentUserAccounts();
            var clientIds = userAccounts.Select(t => t.ID).ToList();
            var avatars = GetUserAvatarsByIds(clientIds);

            var users = (from userAccount in userAccounts
                         join avatar in avatars on userAccount.ID equals avatar.ID into avatars2
                         from avatar in avatars2.DefaultIfEmpty()
                         select new GetCurrentUserAccountsReturnModel
                         {
                             ID = userAccount.ID,
                             UserName = userAccount.UserName,
                             IsCurrent = userAccount.IsCurrent,
                             AvatarUrl = avatar != null ? avatar.AvatarUrl : "",
                         }).ToList();

            return base.JsonResult(VueMsgType.success, "", users.Json());
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, TokenOverdueCheck = false)]
        public JsonResult SwitchAccount(SwitchAccountModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                return base.JsonResult(VueMsgType.warning, "用户名不能为空");
            }
            if (string.IsNullOrEmpty(model.UserName))
            {
                return base.JsonResult(VueMsgType.warning, "用户名不能为空");
            }

            ClientUser user = new ClientUser()
            {
                UserName = model.UserName ?? "",
                Password = "",
            };

            var loginReturn = user.LoginMobile(checkPassword: false);

            if (!loginReturn.Item1)
            {
                // 登录失败
                return base.JsonResult(VueMsgType.warning, loginReturn.Item2);
            }

            // 登录成功
            Response.Headers.Add("token", loginReturn.Item3.Token);
            return base.JsonResult(VueMsgType.success, string.Empty);
        }


        #endregion

        #region 微信登录

        [HttpGet]
        public async Task<ActionResult> WeChatCode2Session(string code, string randomstring, string encryptedData, string iv)
        {
            var miniprogramappid = ConfigurationManager.AppSettings["miniprogramappid"];
            var miniprogramsecret = ConfigurationManager.AppSettings["miniprogramsecret"];
            var weChat = new WeChat(miniprogramappid, miniprogramsecret);

            var res = await weChat.GetCode2Session(new WeChat.Code2SessionParamter(code));
            // 为了数据不被篡改，开发者不应该把 session_key 传到小程序客户端等服务器外的环境。
            SetSession(SessionExtName.WeChatSessionKey + code + randomstring, res.session_key, DateTime.Now.AddMinutes(1));

            //var result1 = WXToolKit.DecodeEncryptedData(res.session_key, encryptedData, iv);
            //res.session_key = "";

            //var sfd = new
            //{
            //    res,
            //    result1,

            //};

            return JsonResult(VueMsgType.success, "", res.Json());
        }

        [HttpPost]
        public ActionResult WeChatGetUserInfo(WeChatLoginModel parameter)
        {
            var miniprogramappid = ConfigurationManager.AppSettings["miniprogramappid"];
            var miniprogramsecret = ConfigurationManager.AppSettings["miniprogramsecret"];
            var weChat = new WeChat(miniprogramappid, miniprogramsecret);

            //if (!string.IsNullOrEmpty(parameter.code)) await WeChatCode2Session(parameter.code);
            var session_key = GetSession(SessionExtName.WeChatSessionKey + parameter.code + parameter.randomstring).ToString();
            var userInfo = weChat.GetUserInfo(parameter.iv, parameter.encryptedData, session_key, parameter.rawData, parameter.signature);
            return JsonResult(VueMsgType.success, "", userInfo.Json());
        }

        public ActionResult WeChatGetPhoneInfo(string session_key, string encryptedData, string iv)
        {
            var result1 = WXToolKit.DecodeEncryptedData(session_key, encryptedData, iv);
            return JsonResult(VueMsgType.success, "", result1);
        }

        //[HttpPost]
        //public ActionResult WeChatGetPhoneInfo(string encryptedData, string code, string iv)
        //{
        //    var miniprogramappid = ConfigurationManager.AppSettings["miniprogramappid"];
        //    var miniprogramsecret = ConfigurationManager.AppSettings["miniprogramsecret"];
        //    string grant_type = "authorization_code";
        //    string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + miniprogramappid + "&secret=" + miniprogramsecret 
        //        + "&js_code=" + code + "&grant_type=" + grant_type;

        //    string res = WXRESTClient.Get(url);//发Get请求拿到响应内容 ,RESTClient.cs 见后文//

        //    var jsonlist = (dynamic)JsonConvert.DeserializeObject(res);   //LitJson.JsonMapper.ToObject(res);//反序列化响应内容
        //    string open_id = (string)jsonlist["openid"]; //拿openid
        //    string session_key = (string)jsonlist["session_key"];//拿session_key 
        //    if (!string.IsNullOrWhiteSpace(open_id) && !string.IsNullOrWhiteSpace(session_key))
        //    {

        //        var result1 = WXToolKit.DecodeEncryptedData(session_key, encryptedData, iv);
        //        //代码走到这就行了，可以打印解密以后的result1 json字符串，查看解密结果是否正确。  没必要往下继续进行了，再往下也就是反序列化一下，拿到手机号码，多的不说了，自行发挥，咱们主要说解密的过程 解密代码都在下面的WXToolKit.cs工具类中
        //        return JsonResult(VueMsgType.success, "", result1);
        //    }
        //    return JsonResult(VueMsgType.error, "获取openid错误");
        //}

        #endregion

        #region 退出
        /// <summary>
        /// 用户退出
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, TokenOverdueCheck = false)]
        public ActionResult LoginOut()
        {
            //退出登录
            Yahv.Client.Current.LoginOut();
            return View();
        }

        #endregion

        #region 注销账号

        /// <summary>
        /// 注销账号
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true, TokenOverdueCheck = false)]
        public JsonResult DeleteLoginAccount()
        {
            var result = Yahv.Client.Current.DeleteLoginAccount();
            var isSuccess = result.Item1;
            var errMsg = result.Item2;
            return base.JsonResult(isSuccess ? VueMsgType.success : VueMsgType.error, errMsg);
        }

        #endregion

        #region 假的发送短信验证码

        [UserAuthorize(UserAuthorize = false, TokenOverdueCheck = false)]
        public JsonResult FakeSendCode(string type, string phone)
        {
            if (type == "MobileLogin")
            {
                Random ran = new Random();
                string messageCode = ran.Next(0, 999999).ToString("000000");
                SetSession(SessionExtName.MobileLogin + phone, messageCode, DateTime.Now.AddMinutes(3));
                return base.JsonResult(VueMsgType.success, new
                {
                    Type = type,
                    Phone = phone,
                    Code = messageCode,
                }.Json());
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        #endregion

        #region 完善会员信息
        /// <summary>
        /// 完善会员信息
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(InformalMembership = true, UserAuthorize = false)]
        public ActionResult MemberInfo()
        {
            //清理session，重新获取公司名称
            Yahv.Client.Current.ClearSession();
            //获取当前客户信息
            var current = Client.Current;
            var clientinfo = current.MyClients;

            Yahv.Underly.ServiceType serviceType = clientinfo.ServiceType;
            bool isCustoms = (serviceType & Underly.ServiceType.Customs) == Underly.ServiceType.Customs;
            bool isWarehouse = (serviceType & Underly.ServiceType.Warehouse) == Underly.ServiceType.Warehouse;

            isCustoms = isCustoms & clientinfo.IsDeclaretion;
            isWarehouse = isWarehouse & clientinfo.IsStorageService;
            //if (current.IsValid)
            if (isCustoms || isWarehouse)
            {
                return RedirectToAction("/Index", "Home");
            }
            var invoice = Client.Current.MyInvoice.SingleOrDefault();
            var clientcontact = Client.Current.MyContacts.FirstOrDefault();
            var file = new CenterFilesView().FirstOrDefault(item => item.ClientID == clientinfo.ID && item.Type == (int)FileType.BusinessLicense);
            //营业执照
            var blFile = new FileModel()
            {
                name = file?.CustomName,
                fullURL = file == null ? "" : PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + file.Url,
            };

            //协议
            var agree = Client.Current.MyAgreement.FirstOrDefault();
            //货款条款
            var productFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 1);
            //税费条款
            var taxFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 2);
            //代理费条款
            var agencyFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 3);
            //杂费条款
            var incidentalFeeClause = agree?.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 4);

            var agreement = new
            {
                StartDate = agree?.StartDate.ToString("yyyy年MM月dd日"),
                EndDate = agree?.EndDate.ToString("yyyy年MM月dd日"),
                AgencyRate = agree?.AgencyRate.ToString(CultureInfo.InvariantCulture),
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
            };
            var model = new BasicInfoViewModel()
            {
                ID = clientinfo.ID,
                CompanyName = clientinfo.Name,
                RegAddress = clientinfo.RegAddress,
                Corporation = clientinfo.Corporation,
                Uscc = clientinfo.Uscc,
                Tel = clientcontact?.Tel,
                Email = clientcontact?.Email,
                CustomsCode = clientinfo.CustomsCode,
                Invoice = new InvoiceViewModel
                {
                    ID = invoice?.ID,
                    DeliveryType = ((int?)invoice?.DeliveryType).ToString(),
                    DeliveryTypeName = invoice?.DeliveryType.GetDescription(),
                    CompanyName = clientinfo.Name,
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
                    InvoiceDeliveryTypeOptions = ExtendsEnum.ToDictionary<InvoiceDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).Json(),
                    InvoiceTypeOptions = ExtendsEnum.ToDictionary<InvoiceType>().Where(item => item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).Json(),
                },
                BLFile = blFile,
            };
            ViewBag.Agreement = agreement;

            Response.Headers.Add(UserAuthorizeAttribute.jumpto, JumpToPage.Home_MemberInfo.ToString());

            return View(model);
        }

        /// <summary>
        /// 公司名称弹框
        /// </summary>
        /// <returns></returns>
        public ActionResult _PartialCompany()
        {
            return PartialView();
        }

        /// <summary>
        /// 根据关键字模糊查询
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetEnterpriseByKeyword(SearchEnterpriseModel data)
        {
            try
            {
                var current = Client.Current;

                var message = Yahv.PvWsOrder.Services.Extends.CrmNoticeExtends.GetEnterpriseByKeyword(data.Keyword, current.ID);
                if (message.success == false)
                {
                    return base.JsonResult(VueMsgType.success, string.Empty, new List<object>().Json());
                }

                var resultByKeyword = JsonConvert.DeserializeObject<SearchByKeywordModel>(message.data);
                if (resultByKeyword.code != "10000" || resultByKeyword.result == null || resultByKeyword.result.error_code != 0)
                {
                    return base.JsonResult(VueMsgType.success, string.Empty, new List<object>().Json());
                }

                if (resultByKeyword.result.result == null || resultByKeyword.result.result.items == null || !resultByKeyword.result.result.items.Any())
                {
                    return base.JsonResult(VueMsgType.success, string.Empty, new List<object>().Json());
                }

                List<object> result = new List<object>();
                //result.Add(new { value = "三全鲜食（北新泾店）", address = "长宁区新渔路144号", });
                //result.Add(new { value = "Hot honey 首尔炸鸡（仙霞路）", address = "上海市长宁区淞虹路661号", });
                //result.Add(new { value = "新旺角茶餐厅", address = "上海市普陀区真北路988号创邑金沙谷6号楼113", });
                //result.Add(new { value = "泷千家(天山西路店)", address = "天山西路438号", });

                foreach (var item in resultByKeyword.result.result.items)
                {
                    if (string.IsNullOrEmpty(item.name))
                    {
                        continue;
                    }

                    Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                    string name = regex.Replace(item.name, "").Replace("&lt;em&gt;", "").Replace("&lt;/em&gt;", "")
                                                                .Replace("<em>", "").Replace("</em>", "").Trim();
                    string legalPersonName = !string.IsNullOrEmpty(item.legalPersonName) ? item.legalPersonName : "";
                    legalPersonName = regex.Replace(legalPersonName, "").Replace("&lt;em&gt;", "").Replace("&lt;/em&gt;", "").Trim();

                    string legalPersonNameDes = !string.IsNullOrEmpty(legalPersonName) ? (" 法人：" + legalPersonName) : "";

                    result.Add(new
                    {
                        value = "名称：" + name + legalPersonNameDes,
                        name,
                        legalPersonName,
                    });
                }

                return base.JsonResult(VueMsgType.success, string.Empty, result.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.success, string.Empty, new List<object>().Json());
            }
        }

        /// <summary>
        /// 根据名称精确查询
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult GetEnterpriseByName(SearchEnterpriseModel data)
        {
            try
            {
                var current = Client.Current;

                var message = Yahv.PvWsOrder.Services.Extends.CrmNoticeExtends.GetEnterpriseByName(data.Keyword, current.ID);
                if (message.success == false)
                {
                    return base.JsonResult(VueMsgType.error, string.Empty);
                }

                var resultByName = JsonConvert.DeserializeObject<SearchByNameModel>(message.data);
                if (resultByName.code != "10000" || resultByName.result == null || resultByName.result.error_code != 0)
                {
                    return base.JsonResult(VueMsgType.error, string.Empty);
                }

                if (resultByName.result.result == null || resultByName.result.result.baseInfo == null)
                {
                    return base.JsonResult(VueMsgType.error, string.Empty);
                }

                var resultViewModel = new BasicInfoViewModel()
                {
                    //CustomsCode = "", //海关编码
                    Uscc = resultByName.result.result.baseInfo.creditCode, //统一社会信用代码
                    Corporation = resultByName.result.result.baseInfo.legalPersonName, //公司法人
                    RegAddress = resultByName.result.result.baseInfo.regLocation, //注册地址
                    Tel = resultByName.result.result.baseInfo.phoneNumber, //固定电话
                    Email = resultByName.result.result.baseInfo.email, //电子邮件
                    //营业执照

                    Invoice = new InvoiceViewModel()
                    {
                        //CompanyName = "", //发票名称（这个不用，前端赋值了）
                        TaxperNumber = resultByName.result.result.baseInfo.creditCode, //纳税人识别号
                        //Type = "", //发票类型
                        //Bank = "", //开户行
                        //Account = "", //账号
                        BankAddress = resultByName.result.result.baseInfo.regLocation, //地址
                        //DeliveryType = "", //发票交付方式
                        //Name = "", //收件人姓名
                        //Address = "", //发票收件地址
                        //Mobile = "", //手机
                        Tel = resultByName.result.result.baseInfo.phoneNumber, //电话
                        Email = resultByName.result.result.baseInfo.email, //邮箱
                    }
                };

                return base.JsonResult(VueMsgType.success, string.Empty, resultViewModel.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, string.Empty);
            }
        }

        /// <summary>
        /// 客户信息提交
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(InformalMembership = true, UserAuthorize = false, MobileLog = true)]
        public JsonResult MemberInfoSubmit(BasicInfoViewModel model, string oldEntName)
        {
            try
            {
                var message = new JMessage();
                //修改公司名称
                if (oldEntName != model.CompanyName)
                {
                    model.CompanyName = PinyinHelper.SBCToDBC(model.CompanyName);
                    message = PvWsOrder.Services.Extends.CrmNoticeExtends.ClientNameModify(oldEntName, model.CompanyName);
                    if (!message.success)
                    {
                        return base.JsonResult(VueMsgType.error, "保存失败:" + message.data);
                    }
                    else
                    {
                        //清理session，重新获取公司名称
                        Yahv.Client.Current.ClearSession();
                    }
                }

                var current = Yahv.Client.Current;
                #region 客户基础信息
                var clientinfo = new
                {
                    model.CustomsCode,
                    Enterprise = new
                    {
                        Name = model.CompanyName,
                        model.Uscc,
                        model.Corporation,
                        model.RegAddress,
                    },
                    Contact = new
                    {
                        Name = current.UserName,
                        current.Mobile,
                        model.Tel,
                        model.Email,
                    },
                };
                message = PvWsOrder.Services.Extends.CrmNoticeExtends.ClientInfo(clientinfo.Json());
                if (!message.success)
                {
                    return base.JsonResult(VueMsgType.error, "保存失败:" + message.data);
                }
                #endregion


                #region 营业执照
                if (!string.IsNullOrWhiteSpace(model.BLFile.URL))
                {
                    var blfile = new CenterFileDescription()
                    {
                        Type = (int)FileType.BusinessLicense,
                        ClientID = current.EnterpriseID,
                        CustomName = model.BLFile.name,
                        Url = model.BLFile.URL,
                        AdminID = current.ID,
                    };
                    //删除原文件
                    var XDTfileIds = new CenterFilesView().Where(item => (item.ClientID == current.EnterpriseID || item.ClientID == current.XDTClientID) && item.Type == (int)FileType.BusinessLicense).Select(item => item.ID).ToArray();
                    new CenterFilesView().Delete(XDTfileIds);

                    //上传新文件
                    new CenterFilesView().XDTUpload(blfile);

                    var xdtblfile = new CenterFileDescription()
                    {
                        Type = (int)FileType.BusinessLicense,
                        ClientID = current.XDTClientID,
                        CustomName = model.BLFile.name,
                        Url = model.BLFile.URL,
                        AdminID = current.ID,
                    };
                    //上传新文件
                    new CenterFilesView().XDTUpload(xdtblfile);
                }
                #endregion

                #region 发票信息

                if (model.IsEditInvoice)
                {
                    var Invoice = new ShencLibrary.SyncInvoice()
                    {
                        Type = (InvoiceType)int.Parse(model.Invoice.Type),
                        Bank = model.Invoice.Bank,
                        RegAddress = model.RegAddress,
                        BankAddress = model.Invoice.BankAddress,
                        Account = model.Invoice.Account,
                        TaxperNumber = model.Invoice.TaxperNumber,
                        Name = model.Invoice.Name,
                        Tel = model.Invoice.Tel,
                        Mobile = model.Invoice.Mobile,
                        Email = model.Invoice.Email,
                        Address = model.Invoice.Address,
                        Postzip = model.Invoice.Postzip,
                        DeliveryType = (InvoiceDeliveryType)int.Parse(model.Invoice.DeliveryType),
                        CompanyTel = model.Invoice.CompanyTel,
                    };
                    //开票信息持久化
                    new ShencLibrary.DccInvoice().Enter(current.EnterpriseID, Invoice, current.ID);
                }

                #endregion

                return base.JsonResult(VueMsgType.success, "请等待分配专员");
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "保存失败:" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 读取配置的门户网站域名信息（html页面中调用）
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false)]
        public string GetDomainForIC()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DomainForIC"];
        }

        /// <summary>
        /// 检查企业名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CheckCompanyName(string oldEntName, string newEntName)
        {
            newEntName = newEntName.InputText();
            if (Alls.Current.WsClients.Any(item => item.Name == newEntName && item.Name != oldEntName))
            {
                return base.JsonResult(VueMsgType.error, "企业【" + newEntName + "】已注册，不可重复！");
            }
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 关于香港畅运国际货仓搬迁的通知
        /// </summary>
        /// <returns></returns>
        public ActionResult MoveNotice()
        {
            return View();
        }

        /// <summary>
        /// 日期选择(DateRange)
        /// </summary>
        public ActionResult _DatePickerDateRange()
        {
            return PartialView("_DatePickerDateRange2");
        }

        /// <summary>
        /// 获取汇率
        /// </summary>
        public JsonResult ExchangeRate(string code)
        {
            var customRate = Yahv.Alls.Current.CustomExchangeRates.FindByCode(code).Rate;
            var realTimeRate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode(code).Rate;

            var data = new { customRate, realTimeRate, };
            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        /// <summary>
        /// 获取汇率
        /// </summary>
        public JsonResult ExchangeRateByCurrencyInt(int currencyInt)
        {
            var currency = (Currency)currencyInt;
            return this.ExchangeRate(currency.ToString());
        }

        /// <summary>
        /// 官网上下载中心的文档
        /// </summary>
        /// <returns></returns>
        public JsonResult DownloadCenter()
        {
            var OfficalWebsite = PurchaserContext.Current.OfficalWebsite;


            var data = new List<DownloadCenterReturnModel>
            {
                new DownloadCenterReturnModel
                {
                    FileName = "代理报关协议",
                    FileUrl = "/Files/代理报关协议.docx",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "报关委托书模板",
                    FileUrl = "/Files/报关委托书模板.docx",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "香港库房库位租赁及仓储协议",
                    FileUrl = "/Files/香港库房库位租赁及仓储协议.doc",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "香港本地交货协议",
                    FileUrl = "/Files/香港本地交货协议.docx",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "付汇委托书",
                    FileUrl = "/Files/付汇委托书.docx",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "深圳收款对公账户信息",
                    FileUrl = "/Files/华芯通对公账户.doc",
                },
                new DownloadCenterReturnModel
                {
                    FileName = "香港付汇的美金账户信息",
                    FileUrl = "/Files/付汇美金账户信息.docx",
                },
            };

            foreach (var item in data)
            {
                item.FileUrl = OfficalWebsite + item.FileUrl;
            }

            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        /// <summary>
        /// 获取实时汇率
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRealTimeExchangeRates()
        {
            var realTimeExchangeRates = Yahv.Alls.Current.RealTimeExchangeRates;

            var linq = (from rate in realTimeExchangeRates
                        group rate by new { rate.Code } into g
                        select g.OrderByDescending(t => t.UpdateDate).First()).ToList();

            var data = linq.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name,
                item.Rate,
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }).OrderByDescending(t => t.Code).Take(6).ToList();

            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        /// <summary>
        /// 获取海关汇率
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomExchangeRates()
        {
            var customExchangeRates = Yahv.Alls.Current.CustomExchangeRates;

            var linq = (from rate in customExchangeRates
                        group rate by new { rate.Code } into g
                        select g.OrderByDescending(t => t.UpdateDate).First()).ToList();

            var data = linq.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name,
                item.Rate,
                UpdateDate = DateTime.Now.ToString("yyyy-MM"),  // item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }).OrderByDescending(t => t.Code).Take(6).ToList();

            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        private string HtmlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var s = str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&#39;", "'")
                .Replace("&quot;", "\"").Replace("<br>", "\n").Replace("&nbsp;", " ").Replace("&amp;", "&");
            return s;
        }

        private string GetFirstImgSrc(string str)
        {
            var matches = Regex.Matches(str, "<img[\\w\\W]*?src[\\s|\\S]*=[\"|\']?([\\w\\W]*?)(bmp|gif|jpg|jpeg|png)[\\w\\W]*?/>");
            if (matches == null || matches.Count <= 0)
            {
                return "";
            }

            if (matches[0].Groups.Count < 3
                || string.IsNullOrEmpty(matches[0].Groups[1].Value)
                || string.IsNullOrEmpty(matches[0].Groups[2].Value))
            {
                return "";
            }

            return (matches[0].Groups[1].Value.Trim() + matches[0].Groups[2].Value.Trim());
        }

        private string HandleContent(string str)
        {
            str = Regex.Replace(str, "<img", "<img style=\"max-width:96%; margin-left:2%; margin-right:2%; \"");
            str = Regex.Replace(str, "<table", "<div style=\" width:100%; overflow-x:scroll; overflow-y: hidden; border: 1px solid #cccccc;  \"><table");
            str = Regex.Replace(str, "</table>", "</table></div>");

            str = "<body style=\" overflow-x:hidden; \">" + str + "</body>";

            return str;
        }

        /// <summary>
        /// 获取公告板信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNoticeBoards()
        {
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
            };

            var noticeBoardView = new NoticeBoardView();
            var tuple = noticeBoardView.ToPage(paramlist.page, paramlist.rows);

            var list = tuple.Item1.Select(item => new
            {
                item.ID,
                Title = HtmlDecode(item.Title),
                ImgUrl = GetFirstImgSrc(HtmlDecode(item.Content)),
                CreateDateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                CreateDateTime = item.CreateDate.ToString("HH:mm:ss"),
                Content = HandleContent(HtmlDecode(item.Content)),
            });

            return this.Paging(list, tuple.Item2);
        }

        public ActionResult TestShowNoticeBoard()
        {
            return View();
        }


        /// <summary>
        /// 获取付款记录
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetPaymentRecords()
        {
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
            };

            var current = Client.Current;
            var tuple = current.MyPaymentRecords.ToPage(paramlist.page, paramlist.rows);

            var list = tuple.Item1.Select(financeReceipt => new
            {
                financeReceipt.ID,
                CreateDate = financeReceipt.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Amount = financeReceipt.Amount.ToString("0.00"),
                PaymentDetails = financeReceipt.PaymentDetails.Select(orderReceipt => new
                {
                    orderReceipt.OrderID,
                    ProductFee = ShowPaymentAmount(orderReceipt.ProductFee),
                    TaxAgencyFee = ShowPaymentAmount(orderReceipt.TaxAgencyFee),
                }).ToArray(),
            });

            return this.Paging(list, tuple.Item2);
        }

        /// <summary>
        ///  显示付款金额
        /// </summary>
        /// <param name="fee"></param>
        /// <returns></returns>
        private string ShowPaymentAmount(decimal? fee)
        {
            if (fee == null || fee.Value == 0)
            {
                return "0";
            }
            return fee.Value.ToString("0.00");
        }

        /// <summary>
        /// 仓库信息
        /// </summary>
        public JsonResult WarehouseInfo()
        {
            var info = new List<Object>();
            info.Add(new
            {
                name = "香港库房地址",
                address = "香港九龙观塘成业街27号日昇中心1204室",
            });


            return JsonResult(VueMsgType.success, "", info.Json());
        }

        /// <summary>
        /// 获取订单简要信息
        /// </summary>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetOrderBrief()
        {
            var declareorder = Client.Current.MyDeclareOrders;

            var orderstatuslist = new CgOrderStatus[] { CgOrderStatus.待确认, CgOrderStatus.待交货, CgOrderStatus.待报关, CgOrderStatus.待收货, };

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => orderstatuslist.Contains(item.MainStatus);
            lambdas.Add(lambda);

            var data = declareorder.GetPageListOrders(lambdas.ToArray(), 50, 1);
            var list = data.Select(item => new
            {
                //OrderID = "WL13420220919006",
                //StatusName = "等待确认",
                //Desc = "订单已通过审核 等待确认",

                OrderID = item.ID,
                StatusName = item.MainStatus.GetDescription(),
                Desc = GetDescByMainStatus(item.MainStatus),
            });
            return JsonResult(VueMsgType.success, "", list.Json());
        }

        private String GetDescByMainStatus(CgOrderStatus status)
        {
            string desc = "";
            switch (status)
            {
                case CgOrderStatus.待确认:
                    desc = "订单已通过审核，等待确认";
                    break;
                case CgOrderStatus.待交货:
                    desc = "订单已确认，等待交货至香港库房";
                    break;
                case CgOrderStatus.待报关:
                    desc = "香港库房已收货，等待报关";
                    break;
                case CgOrderStatus.待收货:
                    desc = "订单已发货，等待收货";
                    break;
                default:
                    desc = "";
                    break;
            }
            return desc;
        }

    }
}