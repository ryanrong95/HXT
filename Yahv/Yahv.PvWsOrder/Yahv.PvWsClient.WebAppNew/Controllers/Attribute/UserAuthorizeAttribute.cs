using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Utils.Extends;

namespace Yahv.PvWsClient.WebAppNew.Controllers.Attribute
{
    public class UserAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        public bool UserAuthorize = true;

        /// <summary>
        /// 非正式会员
        /// </summary>
        public bool InformalMembership = false;

        /// <summary>
        /// 到完善信息页面
        /// </summary>
        public bool ToCompleteInfo = false;

        /// <summary>
        /// token超时检查
        /// </summary>
        public bool TokenOverdueCheck = true;

        /// <summary>
        /// 是否手机端日志
        /// </summary>
        public bool MobileLog = false;

        public static string jumpto = "jumpto";

        public static string changet = "changet";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //InformalMembership == true 现在的意思是 直接访问
            //InformalMembership == false 有 ServiceType、IsXXX 的条件
            var mobile = filterContext.HttpContext.Request.Headers.Get("mobile");
            var isMobile = !string.IsNullOrEmpty(mobile) && mobile == "1";
            var current = Yahv.Client.Current;

            //if (isMobile && MobileLog && current != null && !string.IsNullOrEmpty(current.Token))
            //{
            //    try
            //    {
            //        StreamReader stream = new StreamReader(filterContext.HttpContext.Request.InputStream);
            //        string body = stream.ReadToEndAsync().GetAwaiter().GetResult();

            //        //Yahv.PvWsOrder.Services.ClientModels.MobileLog.InsertLog(current.Token);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

            if (this.UserAuthorize&&!InformalMembership) //验证是否登录并有 ServiceType、IsXXX 的条件
            {
                if (current == null)
                {

                    filterContext.Result = new RedirectResult("/Home/Login");
                }
                else
                {
                    var client = current.MyClients;
                    Yahv.Underly.ServiceType serviceType = client.ServiceType;
                    bool isCustoms = (serviceType & Underly.ServiceType.Customs) == Underly.ServiceType.Customs;
                    bool isWarehouse = (serviceType & Underly.ServiceType.Warehouse) == Underly.ServiceType.Warehouse;

                    //isCustoms = isCustoms & client.IsDeclaretion;
                    //isWarehouse = isWarehouse & client.IsStorageService;
                    //if ((isCustoms == false) && (isWarehouse == false))
                    //{
                    //    if (ToCompleteInfo)
                    //    {
                    //        filterContext.Result = new RedirectResult("/Home/MemberInfo");
                    //    }
                    //    else
                    //    {
                    //        filterContext.Result = new RedirectResult("/Account/CompleteBaseInfoTip");
                    //    }
                    //}

                    bool isCustomsInfoOK = isCustoms & client.IsDeclaretion;
                    bool isWarehouseInfoOK = isWarehouse & client.IsStorageService;

                    //ToCompleteInfo 表示要访问 不可编辑的用户基本信息页面

                    if ((isCustoms == false) && (isWarehouse == false)) //没有业务类型
                    {
                        if (ToCompleteInfo)
                        {
                            if (!isMobile)
                            {
                                filterContext.Result = new RedirectResult("/Home/MemberInfo"); //进入基本信息编辑页面
                            }
                            filterContext.HttpContext.Response.Headers.Add(jumpto, JumpToPage.Home_MemberInfo.ToString());
                        }
                        else
                        {
                            //否则是下单页面, 因为 InformalMembership == false
                            if (!isMobile)
                            {
                                filterContext.Result = new RedirectResult("/Account/CompleteBaseInfoTip");
                            }
                            filterContext.HttpContext.Response.Headers.Add(jumpto, JumpToPage.Account_CompleteBaseInfoTip.ToString());
                        }
                    }
                    else //有业务类型
                    {
                        if ((isCustomsInfoOK == false) && (isWarehouseInfoOK == false)) //客户信息没有完善
                        {
                            if (ToCompleteInfo)
                            {
                                if (!isMobile)
                                {
                                    filterContext.Result = new RedirectResult("/Home/MemberInfo"); //进入基本信息编辑页面
                                }
                                filterContext.HttpContext.Response.Headers.Add(jumpto, JumpToPage.Home_MemberInfo.ToString());
                            }
                            else
                            {
                                //下单页面
                                if (!isMobile)
                                {
                                    filterContext.Result = new RedirectResult("/Account/CompleteBaseInfoTip");
                                }
                                filterContext.HttpContext.Response.Headers.Add(jumpto, JumpToPage.Account_CompleteBaseInfoTip.ToString());
                            }
                        }
                        else //客户信息完善
                        {
                            if (client.Grade >= Underly.ClientGrade.Sixth)
                            {
                                if (!isMobile)
                                {
                                    filterContext.Result = new RedirectResult("/Account/CannotOrderTip");
                                }
                                filterContext.HttpContext.Response.Headers.Add(jumpto, JumpToPage.Account_CannotOrderTip.ToString());
                            }
                        }
                    }

                }

            }

            if (InformalMembership) //直接访问
            {
                if (current == null)
                {
                    if (!isMobile)
                    {
                        //是网页端
                        filterContext.Result = new RedirectResult("/Home/Login");
                    }
                    else
                    {
                        //是手机端
                        filterContext.HttpContext.Response.Clear();
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
            }

            // 是手机端，并且登录状态下
            // 当token生成时间超过Consts.TokenOverdueHour小时，提示401，要求重新登录
            // 当token生成时间超过2小时，提示更换token
            if (TokenOverdueCheck)
            {
                if (isMobile && current != null && current.TokenCreateDate != null)
                {
                    if (current.TokenCreateDate?.AddHours(Consts.TokenOverdueHour) < DateTime.Now)
                    {
                        filterContext.HttpContext.Response.Clear();
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                    else if (current.TokenCreateDate?.AddHours(Consts.TokenChangeHour) < DateTime.Now)
                    {
                        filterContext.HttpContext.Response.Headers.Add(changet, "1");
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }

    public enum JumpToPage
    {
        Home_MemberInfo,

        Account_CompleteBaseInfoTip,

        Account_CannotOrderTip,

        Home_Index,
    }

}