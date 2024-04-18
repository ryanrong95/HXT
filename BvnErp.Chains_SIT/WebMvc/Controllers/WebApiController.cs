using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Serializers;
using Needs.Wl.Logs.Services;
using Needs.Wl.Web.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    //完全是匿名访问。安全
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class WebApiController : Controller
    {
        /// <summary>
        /// 服务申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Apply(WebApiApply model)
        {
            try
            {
                ServiceApplies apply = new ServiceApplies();
                apply.Address = model.Address.InputText();
                apply.CompanyName = model.Company_Name.InputText();
                apply.Contact = model.Contacts.InputText();
                apply.Email = model.Email.InputText();
                apply.Mobile = model.Contacts_Moblie.InputText();
                apply.Tel = model.Phone.InputText();
                apply.Enter();
                var result = new { Type = "success", Msg = "提交成功" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };  //返回 jsonp 数据，输出回调函数
            }
            catch (Exception ex)
            {
                ex.Log();
                var result = new { Type = "error", Msg = "提交失败" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };
            }
        }

        /// <summary>
        /// 分类查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Query(string query)
        {
            try
            {
                query = query.InputText().ToUpper();
                //TODO:Needs.Wl.User.Plat.UserPlat.Api.ApiClassifyProducts
                var classify = new Needs.Ccs.Services.Views.WebApiClassifyView();
                classify.Predicate = c => c.Model.ToUpper().Contains(query) || c.Name.ToUpper().Contains(query);
                classify.AllowPaging = false;
                var list = classify.ToList().Select(item => new WebApiQuery
                {
                    HSCode = item.HSCode,
                    ProName = item.Name,
                    ProModel = item.Model,
                    FirstLegalUnit = item.Unit1,
                    SecondLegalUnit = item.Unit2,
                    monCon = item.RegulatoryCode,
                    InsQua = item.CIQCode
                });

                var result = new { Type = "success", Msg = "", Data = list.Json() };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };  //返回 jsonp 数据，输出回调函数
            }
            catch (Exception ex)
            {
                ex.Log();
                var result = new { Type = "error", Msg = "查询失败" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };
            }
        }

        /// <summary>
        /// 分类详情查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult QueryDetail(string query)
        {
            try
            {
                query = query.InputText();
                var model = new WebApiQuery();
                var classify = new Needs.Ccs.Services.Views.WebApiClassifyView();
                classify.Predicate = c => c.HSCode == query;
                var entity = classify.FirstOrDefault();

                if (entity != null)
                {
                    model.MFN = entity.MFN.ToString("0.00");
                    model.General = entity.General.ToString("0.00");
                    model.AddedValue = entity.AddedValue.ToString("0.00");
                    model.Consume = entity.Consume.Value.ToString("0.00");
                    model.IsCCC = entity.IsCCC ? "是" : "否";
                    model.Elements = entity.Elements.Split(';');
                    model.ProName = entity.Name;
                    model.HSCode = entity.HSCode;
                    model.CIQCode = entity.CIQCode;
                }
                var result = new { Type = "success", Msg = "", Data = model.Json() };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };  //返回 jsonp 数据，输出回调函数
            }
            catch (Exception ex)
            {
                ex.Log();
                var result = new { Type = "error", Msg = "查询失败" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };
            }
        }

        /// <summary>
        /// 提交建议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Suggestions(Suggestions model)
        {
            try
            {
                Suggestions suggestion = new Suggestions();
                suggestion.Name = model.Name;
                suggestion.Phone = model.Phone;
                suggestion.Summary = model.Summary;
                suggestion.Enter();
                var result = new { Type = "success", Msg = "提交成功" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };  //返回 jsonp 数据，输出回调函数

            }
            catch (Exception ex)
            {
                ex.Log();
                var result = new { Type = "error", Msg = "提交失败" };
                return new Needs.Wl.Web.Mvc.Utils.CustomJsonResult { Data = result.Json() };
            }
        }
    }
}