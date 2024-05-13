using Needs.Linq.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class NoticeDetailController : Controller
    {
        // GET: NoticeDetail
        /// <summary>
        /// 根据运单得到分拣详情
        /// </summary>
        /// <param name="wayBillID">运单编号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <url>/api/noticedetail</url>
        /// <returns></returns>
        public ActionResult Index(string wayBillID,int pageIndex = 1, int pageSize = 20)
        {
            return Json(new { obj = new NoticesView().AsEnumerable().Where(item=>item.WaybillID==wayBillID).Page(pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);

        }
    }
}