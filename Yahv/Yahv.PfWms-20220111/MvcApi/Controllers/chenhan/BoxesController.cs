using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class BoxesController : Controller
    {
        // GET: Boxes
        public ActionResult Index(string warehouseid, int status = 200)
        {
            return Json(Yahv.Erp.Current.WareHourse.Boxes().Where(item => item.WarehouseID.ToUpper() == warehouseid.ToUpper() && item.Status == (BoxesStatus)status), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Enter(JPost obj)
        {
            try
            {
                var boxes = obj.ToObject<Boxes>();
                Yahv.Erp.Current.WareHourse.BoxEnter(boxes);
                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;

                return Json(new JMessage { success = false, code = 404, data = "保存失败!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Dates()
        {
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 箱号产品列表
        /// </summary>
        /// <param name="whid">库房编号</param>
        /// <param name="all">1 显示所有,0 只显示自己</param>
        /// <param name="status">状态：</param>
        /// <param name="key">查询：箱号</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pagesize">页大小</param>
        /// <returns></returns>
        public ActionResult BoxProducts(string whid, int all, int status = 0, string key = null, int pageindex = 1, int pagesize = 20)
        {
            return Json(Yahv.Erp.Current.WareHourse.BoxProducts(whid, all, status, key, pageindex, pagesize), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 变更箱号
        /// </summary>
        /// <param name="whid">库房编号</param>
        /// <param name="oldCode">旧箱号</param>
        /// <param name="newCode">新箱号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeBoxCode(string whid, string oldCode, string newCode)
        {
            try
            {

                Yahv.Erp.Current.WareHourse.ChangeBoxCode(whid, oldCode, newCode);
                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;

                return Json(new JMessage { success = false, code = 400, data = "保存失败!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult WaybillBoxEnter(JPost obj)
        {
            throw new Exception();
            //try
            //{
            //    Yahv.Erp.Current.WareHourse.EnterWaybillBox(obj.ToObject<WaybillBox>());
            //    return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return Json(new JMessage { success = false, code = 400, data = "保存失败!" + ex.Message}, JsonRequestBehavior.AllowGet);
            //}
        }


        public ActionResult List()
        {
            return null;
        }

    }
}