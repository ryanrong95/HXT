//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Controllers;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.ClientViews;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsClient.Model;
//using Yahv.Utils.Http;
//using Yahv.Underly;
//using Yahv.PvWsOrder.Services.ClientModels;

//namespace Yahv.PvUser.WebApp.Controllers
//{
//    public class HomeController : UserController
//    {
//        #region 首页
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Index()
//        {
//            var CurrentClient = Client.Current;

//            var noticeList = new NoticeAlls().Take(4).ToList().Select(item => new
//            {
//                Title = item.Name,
//                URL = item.URL,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd")
//            }).ToArray();

//            //按科目的信用数据
//            var myCreaditData = CurrentClient.MyCredits.Where(item=>item.Currency == Currency.CNY).Select(item => new
//            {
//                item.Business,
//                item.Catalog,
//                item.Total,
//                item.Cost,
//                Left = item.Total - item.Cost,
//            });
//            var goods = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "货款").FirstOrDefault();
//            var tax = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "税款").FirstOrDefault();
//            var agent = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "代理费").FirstOrDefault();
//            var other = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "杂费").FirstOrDefault();
//            var entrygoods = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "货款").FirstOrDefault();
//            var entrytax = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "税款").FirstOrDefault();
//            var entryagent = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "代理费").FirstOrDefault();
//            var entryother = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "杂费").FirstOrDefault();

//            decimal? TotalCredit = myCreaditData.Sum(item => (decimal?)item.Total);
//            decimal? TotalCost = myCreaditData.Sum(item => (decimal?)item.Cost);
//            decimal? TotalLeft = myCreaditData.Sum(item => (decimal?)item.Left);
//            //我的总信用
//            var mytotalCredit = new
//            {
//                TotalCredit = string.Format("{0:N}", TotalCredit.GetValueOrDefault()),
//                TotalCost = string.Format("{0:N}", TotalCost.GetValueOrDefault()),
//                TotalLeft = string.Format("{0:N}", TotalLeft.GetValueOrDefault()),
//            };

//            //订单进展
//            var orderOpertions = CurrentClient.MyOrderOperations.Take(10);
//            var orderList = orderOpertions.ToList().Select(item => new
//            {
//                item.Order.Type,
//                TypeName = item.Order.Type.GetDescription(),
//                OrderID = item.MainID,
//                CreateDate = item.CreateDate.ToString("yyyy年MM月dd日 HH:mm:ss"),
//                Logs = item.Operation,
//                IsConfirm = item.OrderPaymentStatus == OrderPaymentStatus.Confirm
//            }).ToArray();

//            //本港货款货款
//            var myPayable = CurrentClient.MyPayable.Where(item => item.Business == "代仓储" && item.Currency == Currency.CNY);
//            //本港应收
//            var goodsVouchersHK = (myPayable.FirstOrDefault(item => item.Catalog == "货款")?.Price).GetValueOrDefault();
//            var taxVouchersHK = (myPayable.FirstOrDefault(item => item.Catalog == "税款")?.Price).GetValueOrDefault();
//            var agentVouchersHK = (myPayable.FirstOrDefault(item => item.Catalog == "代理费")?.Price).GetValueOrDefault();
//            var otherVouchersHK = (myPayable.FirstOrDefault(item => item.Catalog == "杂费")?.Price).GetValueOrDefault();
//            var TotalVouchersHK = myPayable.Sum(item => (decimal?)item.Price).GetValueOrDefault();

//            var myEntryPayable = CurrentClient.MyPayable.Where(item => item.Business == "代报关");
//            var EntrygoodsVouchers = (myEntryPayable.FirstOrDefault(item => item.Catalog == "货款")?.Price).GetValueOrDefault();
//            var EntrytaxVouchers = (myEntryPayable.FirstOrDefault(item => item.Catalog == "税款")?.Price).GetValueOrDefault();
//            var EntryagentVouchers = (myEntryPayable.FirstOrDefault(item => item.Catalog == "代理费")?.Price).GetValueOrDefault();
//            var EntryotherVouchers = (myEntryPayable.FirstOrDefault(item => item.Catalog == "杂费")?.Price).GetValueOrDefault();
//            var EntryTotalVouchers = myEntryPayable.Sum(item => (decimal?)item.Price).GetValueOrDefault();
//            var TotalPayable = EntryTotalVouchers + TotalVouchersHK;

//            //余额
//            var myBalance_CNY = CurrentClient.MyBalance.Where(item => item.Currency == Currency.CNY).Sum(item => (decimal?)item.Price).GetValueOrDefault();
//            var MyBalance_HK = CurrentClient.MyBalance.Where(item => item.Currency == Currency.HKD).Sum(item => (decimal?)item.Price).GetValueOrDefault();

//            var client = CurrentClient.MyClients;
//            var data = new
//            {
//                OrderList = orderList,
//                EnterCode = client?.EnterCode,
//                Mobile = CurrentClient.Mobile,
//                Email = CurrentClient.Email,
//                Company = CurrentClient.RealName,
//                NoticeList = noticeList,
//                ServiceManager = new
//                {
//                    Name = CurrentClient.ServiceManager?.RealName,
//                    Phone = CurrentClient.ServiceManager?.Mobile,
//                    Email = CurrentClient.ServiceManager?.Email,
//                },
//                Merchandiser = new
//                {
//                    Name = CurrentClient.Merchandiser?.RealName,
//                    Phone = CurrentClient.Merchandiser?.Mobile,
//                    Email = CurrentClient.Merchandiser?.Email
//                },
//                MytotalCredit = mytotalCredit,
//                Manager = new
//                {
//                    Name = CurrentClient.Manager?.RealName,
//                    Phone = CurrentClient.Manager?.Mobile,
//                    Email = CurrentClient.Manager?.Email
//                },
//                myCreaditData = new
//                {
//                    GoodsData = new
//                    {
//                        total = string.Format("{0:N}", (goods?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (goods?.Left).GetValueOrDefault()),
//                    },
//                    TaxData = new
//                    {
//                        total = string.Format("{0:N}", (tax?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (tax?.Left).GetValueOrDefault()),
//                    },
//                    AgentData = new
//                    {
//                        total = string.Format("{0:N}", (agent?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (agent?.Left).GetValueOrDefault()),
//                    },
//                    OtherData = new
//                    {
//                        total = string.Format("{0:N}", (other?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (other?.Left).GetValueOrDefault()),
//                    },
//                    EntryGoods = new
//                    {
//                        total = string.Format("{0:N}", (entrygoods?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entrygoods?.Left).GetValueOrDefault()),
//                    },
//                    EntryTax = new
//                    {
//                        total = string.Format("{0:N}", (entrytax?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entrytax?.Left).GetValueOrDefault()),
//                    },
//                    EntryAgent = new
//                    {
//                        total = string.Format("{0:N}", (entryagent?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entryagent?.Left).GetValueOrDefault()),
//                    },
//                    EntryOther = new
//                    {
//                        total = string.Format("{0:N}", (entryother?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entryother?.Left).GetValueOrDefault()),
//                    },
//                },
//                HKReceive = new
//                {
//                    goodsVouchersHK = string.Format("{0:N}", goodsVouchersHK),
//                    taxVouchersHK = string.Format("{0:N}", taxVouchersHK),
//                    agentVouchersHK = string.Format("{0:N}", agentVouchersHK),
//                    otherVouchersHK = string.Format("{0:N}", otherVouchersHK),
//                    TotalVouchersHK = string.Format("{0:N}", TotalVouchersHK),
//                },
//                EntryReceive = new
//                {
//                    EntrygoodsVouchers = string.Format("{0:N}", EntrygoodsVouchers),
//                    EntrytaxVouchers = string.Format("{0:N}", EntrytaxVouchers),
//                    EntryagentVouchers = string.Format("{0:N}", EntryagentVouchers),
//                    EntryotherVouchers = string.Format("{0:N}", EntryotherVouchers),
//                    EntryTotalVouchers = string.Format("{0:N}", EntryTotalVouchers),
//                },
//                TotalPayable = string.Format("{0:N}", TotalPayable),
//                MyBalance_CNY = string.Format("{0:N}", myBalance_CNY),
//                MyBalance_HK = string.Format("{0:N}",MyBalance_HK),
//            };
//            return View(data);
//        }
//        #endregion

//        #region 登录
//        /// <summary>
//        /// 登录
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = false)]
//        public ActionResult Login()
//        {
//            LoginViewModel model = new LoginViewModel();

//            if (Cookies.Supported)
//            {
//                var User = Yahv.Client.Current;
//                if (User != null && User.IsRemeber)
//                {
//                    model.UserName = User.UserName;
//                    model.Password = Guid.NewGuid().ToString();
//                    model.RemberMe = true;
//                }
//            }
//            return View(model);
//        }

//        /// <summary>
//        /// 登录
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = false)]
//        public ActionResult Login(LoginViewModel model)
//        {
//            ClientUser User = new ClientUser()
//            {
//                UserName = model.UserName,
//                Password = model.Password,
//                IsRemeber = model.RemberMe,
//            };
//            var current = Yahv.Client.Current;
//            if (current != null && current.UserName.Equals(model.UserName))
//            {
//                User = current;
//                User.IsRemeber = model.RemberMe;
//            }
//            try
//            {
//                User.Login();
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.warning, ex.Message);
//            }
//            return base.JsonResult(VueMsgType.success, string.Empty);
//        }
//        #endregion

//        #region 退出
//        /// <summary>
//        /// 用户退出
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = false)]
//        public ActionResult LoginOut()
//        {
//            //退出登录
//            Yahv.Client.Current.LoginOut();
//            return Redirect("/Home/Login");
//        }
//        #endregion

//        #region 投诉与建议
//        /// <summary>
//        /// 投诉建议
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = false)]
//        public JsonResult Suggestions(SuggestionViewModel model)
//        {
//            try
//            {
//                Suggestion suggestion = new Suggestion();
//                suggestion.Name = model.name.InputText();
//                suggestion.Phone = model.phone.InputText();
//                suggestion.Summary = model.summary.InputText();
//                suggestion.Enter();
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "提交失败");
//            }
//            return base.JsonResult(VueMsgType.success, "提交成功");
//        }
//        #endregion
//    }
//}