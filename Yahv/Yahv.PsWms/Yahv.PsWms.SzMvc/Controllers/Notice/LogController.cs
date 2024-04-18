using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class NoticeController : BaseController
    {
        #region 页面

        /// <summary>
        /// 日志
        /// </summary>
        /// <returns></returns>
        public ActionResult Log() { return View(); }

        #endregion

        public JsonResult GetLogList(GetLogListSearchModel searchModel)
        {
            using (var query = new LogListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(searchModel.ActionNameInt))
                {
                    string[] arr = searchModel.ActionNameInt.Split(',');

                    List<string> actionNameList = new List<string>();

                    foreach (var actionNameInt in arr)
                    {
                        if (!string.IsNullOrEmpty(actionNameInt))
                        {
                            actionNameList.Add(((LogAction)Convert.ToInt32(actionNameInt)).GetDescription());
                        }
                    }

                    if (actionNameList != null && actionNameList.Count > 0)
                    {
                        view = view.SearchByActionNames(actionNameList.ToArray());
                    }
                }

                if (!string.IsNullOrEmpty(searchModel.MainID))
                {
                    view = view.SearchByMainID(searchModel.MainID);
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateBegin))
                {
                    DateTime begin = DateTime.Parse(searchModel.CreateDateBegin);
                    view = view.SearchByCreateDateBegin(begin);
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateEnd))
                {
                    DateTime end = DateTime.Parse(searchModel.CreateDateEnd);
                    view = view.SearchByCreateDateEnd(end);
                }

                Func<LogListViewModel, GetLogListReturnModel> convert = item => new GetLogListReturnModel
                {
                    LogID = item.LogID,
                    ActionName = item.ActionName,
                    MainID = item.MainID,
                    Url = item.Url,
                    CreateDateDes = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                };

                var viewData = view.ToMyPage(convert, searchModel.page, searchModel.rows);

                return Json(new { type = "success", msg = "", data = new { list = viewData.Item1, total = viewData.Item2 } }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetLogDetail(GetLogDetailSearchModel searchModel)
        {
            using (var query = new LogListView())
            {
                var view = query;

                view = view.SearchByLogID(searchModel.LogID);

                Func<LogListViewModel, GetLogDetailReturnModel> convert = item => new GetLogDetailReturnModel
                {
                    LogID = item.LogID,
                    ActionName = item.ActionName,
                    MainID = item.MainID,
                    Url = item.Url,
                    Content = item.Content,
                    CreateDateDes = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                };

                var viewData = view.ToMyPage(convert);

                return Json(new { type = "success", msg = "", data = viewData.Item1[0] }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}