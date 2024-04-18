using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Http;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 入库分拣通知
    /// </summary>
    public class InNoticesController : NoticesController
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WarehouseID"]?.Value<string>(),
                status = jpost["Status"]?.Value<int?>(),
                partnumber = jpost["Partnumber"]?.Value<string>(),
                orderID = jpost["FormID"]?.Value<string>(),
                start = jpost["Start"]?.Value<string>(),
                end = jpost["End"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int>(),
                pageSize = jpost["PageSize"]?.Value<int>(),
            };

            using (var view = new NoticesInView())
            {
                var data = view;
                if (!string.IsNullOrWhiteSpace(arguments.whid))
                {
                    data = data.SearchByWarehouseID(arguments.whid.ToLower());
                }

                if (arguments.status.HasValue)
                {
                    if (arguments.status.Value == 1) // 待入库
                    {
                        data = data.SearchByStatus(new NoticeStatus[] { NoticeStatus.Arranging, NoticeStatus.Processing, NoticeStatus.Reviewing, NoticeStatus.Stocking });
                    }
                    else // 已入库
                    {
                        data = data.SearchByStatus(new NoticeStatus[] { NoticeStatus.Closed, NoticeStatus.Completed, NoticeStatus.Rejected });
                    }
                }

                if (!string.IsNullOrWhiteSpace(arguments.partnumber))
                {
                    data = data.SearchByPartnumber(arguments.partnumber);
                }

                if (!string.IsNullOrWhiteSpace(arguments.orderID))
                {
                    data = data.SearchByOrderID(arguments.orderID);
                }

                if (!string.IsNullOrWhiteSpace(arguments.start) && !string.IsNullOrWhiteSpace(arguments.end))
                {
                    data = data.SearchByDate(DateTime.Parse(arguments.start), DateTime.Parse(arguments.end));
                }

                if (!string.IsNullOrWhiteSpace(Request.QueryString["FormID"]))
                {
                    data = data.SearchByNotice(item => item.FormID == Request.QueryString["FormID"].Trim());
                }

                var result = data.ToMyPage(arguments.pageIndex, arguments.pageSize);
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID值为NoticeID</param>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Detail(string id)
        {
            using (var view = new NoticesInView())
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    Response.StatusCode = 500;
                    return Json(new JMessage
                    {
                        success = false,
                        data = "查询的参数不能为null或空字符串",

                    }, JsonRequestBehavior.DenyGet);
                }

                var result = view[id];
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 获取NoticeID下面的所有NoticeItems
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NoticeItems(JPost jpost)
        {
            var arguments = new
            {
                id = jpost["ID"].Value<string>().Trim(),
                pageIndex = jpost["PageIndex"]?.Value<int?>(),
                pageSize = jpost["PageSize"]?.Value<int?>(),
            };

            if (string.IsNullOrEmpty(arguments.id))
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = "参数ID(通知ID)不能为Null或空字符串",
                });
            }

            using (var view = new NoticeItemsView())
            {
                var data = view;
                data = view.SearchByNoticeID(arguments.id);
                var result = data.ToMyPage(arguments.pageIndex, arguments.pageSize);
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 打印入库标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrintNoticeItemLabel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = "参数不正确, ID不能为Null或空字符串"
                });
            }

            using (var view = new NoticeItemsView())
            {
                var data = view;
                data = view.SearchByID(id);
                var result = data.Single();
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.DenyGet);                
            }
        }

        /// <summary>
        /// 入库复核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ActionResult Review(JPost jpost)
        {
            var arguments = new
            {
                noticeID = jpost["NoticeID"].Value<string>().Trim(),
                reviewerID = jpost["ReviewerID"].Value<string>().Trim(),
            };

            try
            {
                using (var view = new NoticesInView())
                {
                    if (string.IsNullOrEmpty(arguments.noticeID) || string.IsNullOrEmpty(arguments.reviewerID))
                    {
                        Response.StatusCode = 500;
                        return Json(new
                        {
                            success = false,
                            data = "通知NoticeID 以及 ReviewerID不能为空"
                        });
                    }

                    // 入库复核
                    view.Review(arguments.noticeID, arguments.reviewerID);

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 入库通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Notice(JPost jpost)
        {
            try
            {
                string noticeID = null;

                // 根据陈经理的要求当前文件,传递的文件已经不再入库通知里传送, 而是单独传送, 并且异步处理
                // var fileItems = jpost["Files"];

                using (NoticesInView view = new NoticesInView())
                {
                    noticeID = view.Notice(jpost);
                    //List<FileHelper> fileHelpList = JsonConvert.DeserializeObject<List<FileHelper>>(fileItems.ToString());

                    //foreach (var fileItem in fileHelpList)
                    //{
                    //    fileItem.MainID = noticeID;
                    //}

                    //if (fileItems != null && fileItems.Count() > 0)
                    //{
                    //    string url = Services.Enums.FromType.FileUpload.GetDescription();
                    //    ApiHelper.Current.JPost(url, new { Files = fileHelpList });
                    //}

                    Response.StatusCode = 200;

                    return Json(new
                    {
                        success = true,
                        data = $"保存的NoticeID为:{noticeID}"
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = ex.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 入库分拣保存修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Sorting(JPost jpost)
        {
            try
            {
                using (NoticesInView view = new NoticesInView())
                {
                    view.Sorting(jpost);
                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = string.Empty
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = ex.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 修改入库通知详情中的承运商信息，运单号,以及异常备注
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Update(JPost jpost)
        {
            try
            {
                using (var view = new NoticesInView())
                {
                    view.Update(jpost);

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = string.Empty,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 通知项心跳
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Hearting(JPost jpost)
        {
            var arguments = new
            {
                adminID = jpost["AdminID"].Value<string>().Trim(),
                noticeItemID = jpost["NoticeItemID"].Value<string>().Trim(),
            };

            using (var sessionView = new NoticeItemSessionView())
            {
                bool isLock = sessionView.Session(arguments.noticeItemID, arguments.adminID);

                return Json(new
                {
                    success = true,
                    data = isLock,
                }, JsonRequestBehavior.DenyGet);
            }
        }
        
        /// <summary>
        /// 获取新增通知项ID
        /// </summary>
        /// <returns></returns>
        public string GetNoticeItemID()
        {
            using (var view = new NoticesInView())
            {
                return view.GetNoticeItemID();
            }
        }

        /// <summary>
        /// 根据通知项ID删除通知项及其分拣
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteNoticeItem(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = "请求参数id 不能为null"
                }, JsonRequestBehavior.DenyGet);
            }

            using (var view = new NoticeItemsView())
            {
                view.delete(id.Trim());
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = string.Empty
                }, JsonRequestBehavior.DenyGet);
            }
        }

        private class FileHelper
        {
            public string ID { get; set; }
            public string Url { get; set; }
            public int Type { get; set; }

            public string CustomName { get; set; }
            public string AdminID { get; set; }
            public string SiteuserID { get; set; }
            public string MainID { get; set; }
        }
    }
}