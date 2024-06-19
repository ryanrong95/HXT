using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.DateTimeStr = DateTime.Now.ToString("yyyy-MM-dd");
            var current = Client.Current;
            ViewBag.UserName = current?.UserName;
            ViewBag.USARate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode("USD")?.Rate;
            ViewBag.HKDRate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode("HKD")?.Rate;
            //业务员
            ViewBag.ServiceManagerName = current?.ServiceManager?.RealName;
            ViewBag.ServiceManagerPhone = current?.ServiceManager?.Mobile;
            ViewBag.ServiceManagerEmail = current?.ServiceManager?.Email;

            //跟单员
            ViewBag.MerchandiserName = current?.Merchandiser?.RealName;
            ViewBag.MerchandiserPhone = current?.Merchandiser?.Mobile;
            ViewBag.MerchandiserEmail = current?.Merchandiser?.Email;

            if (IsMobileLogin())
            {
                var customerServiceIds = new List<string>();
                customerServiceIds.Add(current?.ServiceManager?.ID);
                customerServiceIds.Add(current?.Merchandiser?.ID);
                customerServiceIds = customerServiceIds.Where(t => !string.IsNullOrEmpty(t)).ToList();

                string ServiceManagerCustomerServiceLink = null, MerchandiserCustomerServiceLink = null;
                if (customerServiceIds.Any())
                {
                    var customerServiceLinks = new CustomerServiceLinksView().Where(t => customerServiceIds.Contains(t.ID)).ToList();
                    ServiceManagerCustomerServiceLink = customerServiceLinks.Where(t => t.ID == current?.ServiceManager?.ID).FirstOrDefault()?.Link;
                    MerchandiserCustomerServiceLink = customerServiceLinks.Where(t => t.ID == current?.Merchandiser?.ID).FirstOrDefault()?.Link;
                }
                ViewBag.CustomerServiceId = ConfigurationManager.AppSettings["CustomerServiceId"];
                ViewBag.ServiceManagerCustomerServiceLink = ServiceManagerCustomerServiceLink;
                ViewBag.MerchandiserCustomerServiceLink = MerchandiserCustomerServiceLink;
            }

            //经理
            ViewBag.ManagerName = "张三";
            ViewBag.ManagerPhone = "13590372947";
            ViewBag.ManagerEmail = "";

            var client = current?.MyClients;

            //服务类型
            bool isCustoms = false;
            bool isWarehouse = false;
            bool hasExport = false;
            if (client != null)
            {
                isCustoms = (client.ServiceType & ServiceType.Customs) == ServiceType.Customs;
                isWarehouse = (client.ServiceType & ServiceType.Warehouse) == ServiceType.Warehouse;

                isCustoms = isCustoms && client.IsDeclaretion;
                isWarehouse = isWarehouse && client.IsStorageService;
                hasExport = client.HasExport.Value;
            }
            ViewBag.IsCustoms = isCustoms;
            ViewBag.IsWarehouse = isWarehouse;
            ViewBag.HasExport = hasExport;

        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            this.OnResultExecutedFunction(filterContext);
        }

        protected void OnResultExecutedFunction(ResultExecutedContext filterContext)
        {
            var mobile = filterContext.HttpContext.Request.Headers.Get("mobile");
            if (string.IsNullOrEmpty(mobile) || mobile != "1")
            {
                // 不是手机端
                base.OnResultExecuted(filterContext);
                return;
            }

            // 是手机端

            var contentType = filterContext.HttpContext.Response.ContentType;
            if (contentType == "application/json")
            {
                // 返回的本身就是 json 格式
                base.OnResultExecuted(filterContext);
                return;
            }
            else if (contentType == "text/html")
            {
                // 返回的本身就是 html 文本格式

                //var output = filterContext.HttpContext.Response.Output.ToString();
                //if (!output.StartsWith("mobile"))
                //{
                //    filterContext.HttpContext.Response.ClearContent();
                //    filterContext.HttpContext.Response.Write("mobile");
                //}

                filterContext.HttpContext.Response.ClearContent();

                var tempModelInTextHtml = new
                {
                    ViewData = filterContext.Controller.ViewData,
                    Model = filterContext.Result.GetType().GetProperty("Model")?.GetValue(filterContext.Result, null),
                };
                filterContext.HttpContext.Response.Write(tempModelInTextHtml.Json());

                base.OnResultExecuted(filterContext);
                return;
            }

            // 默认执行
            base.OnResultExecuted(filterContext);
        }

        /// <summary>
        /// 判断是否是手机登录
        /// </summary>
        /// <returns></returns>
        public bool IsMobileLogin()
        {
            var mobile = HttpContext.Request.Headers.Get("mobile");
            return !string.IsNullOrEmpty(mobile) && mobile == "1";
        }

        /// <summary>
        /// 获取手机token
        /// </summary>
        /// <returns></returns>
        public string GetMobileToken()
        {
            return HttpContext.Request.Headers.Get("token");
        }

        public object GetSession(string key)
        {
            if (!IsMobileLogin())
            {
                // PC端
                return Session[key];
            }
            else
            {
                // 手机端
                string tokenMobile = GetMobileToken();
                return HttpContext.Cache[tokenMobile + "_" + key];
            }
        }

        public void SetSession(string key, object value, DateTime? expirationTime = null)
        {
            if (!IsMobileLogin())
            {
                // PC端
                Session[key] = value;
            }
            else
            {
                // 手机端
                string tokenMobile = GetMobileToken();
                if (expirationTime == null)
                {
                    HttpContext.Cache[tokenMobile + "_" + key] = value;
                }
                else
                {
                    HttpContext.Cache.Insert(tokenMobile + "_" + key, value, null, expirationTime.Value, Cache.NoSlidingExpiration);
                }
            }
        }

        public JsonResult JsonResult(VueMsgType type, string message)
        {
            return Json(new { type = type.ToString(), msg = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResult(VueMsgType type, string message, string data)
        {
            return Json(new { type = type.ToString(), msg = message, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Paging<T>(IEnumerable<T> queryable, int total, Func<T, object> converter = null)
        {
            if (converter == null)
            {
                return JsonResult(VueMsgType.success, "", new { list = queryable.ToArray(), total }.Json());
            }
            else
            {
                return JsonResult(VueMsgType.success, "", new { list = queryable.Select(converter).ToArray(), total }.Json());
            }
        }

        public JsonResult Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                return JsonResult(VueMsgType.success, "", new { list = query.ToArray(), total }.Json());
            }
            else
            {
                return JsonResult(VueMsgType.success, "", new { list = query.Select(converter).ToArray(), total }.Json());
            }
        }

        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="operation">操作内容</param>
        /// <param name="summary">备注</param>
        public void OperationLog(string id, string operation, string summary = "")
        {
            Client.Current.OrderOperateLog.Log(new Services.Models.OperatingLog
            {
                Operation = operation,
                MainID = id,
                Summary = summary
            });
        }

        /// <summary>
        /// 错误操作日志
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="operation">操作内容</param>
        /// <param name="summary">备注</param>
        public void ErrorOperationLog(string id, string operation, string summary = "")
        {
            Client.Current.Errorlog.Log(new Services.Models.OperatingLog
            {
                Operation = operation,
                MainID = id,
                Summary = summary
            });
        }
    }

}