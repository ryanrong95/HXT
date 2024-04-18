using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public class LayoutController : BaseController
    {
        /// <summary>
        /// 顶部标题栏
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeHeader() { return PartialView(); }

        /// <summary>
        /// 客服侧边栏
        /// </summary>
        /// <returns></returns>
        public ActionResult StaffSideBar() { return PartialView(); }

        /// <summary>
        /// 主页面底部
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeFooter() { return PartialView(); }

        /// <summary>
        /// 左侧菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult LeftMenu() { return PartialView(); }

        /// <summary>
        /// 面包屑导航
        /// </summary>
        /// <returns></returns>
        public ActionResult BreadcrumbNav() { return PartialView(); }
    }
}