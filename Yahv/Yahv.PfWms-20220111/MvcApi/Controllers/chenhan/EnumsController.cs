using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Wms.Services.Enums;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace MvcApi.Controllers
{
    public class EnumsController : Controller
    {

        public ActionResult Index()
        {
            LitTools.Current["生成香港出库通知"].Log("开始调用：AutoHkExit");

            return Content("测试日志");
        }

        public ActionResult SortingExcuteStatus()
        {
            //return Json(new { obj = Enum.GetValues(typeof(CgSortingExcuteStatus)).Cast<CgSortingExcuteStatus>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
            //return Json(new { obj = Enum.GetValues(typeof(CgSortingExcuteStatus)).Cast<CgSortingExcuteStatus>().Except<CgSortingExcuteStatus>(new CgSortingExcuteStatus[] { CgSortingExcuteStatus.Completed }).Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);

            List<NewSource<CgSortingExcuteStatus>> newSortingExcuteStatusList = new List<NewSource<CgSortingExcuteStatus>>();
            foreach (CgSortingExcuteStatus item in Enum.GetValues(typeof(CgSortingExcuteStatus)))
            {
                if (item == CgSortingExcuteStatus.Completed)
                {
                    continue;
                }
                newSortingExcuteStatusList.Add(new NewSource<CgSortingExcuteStatus>(item, item.GetDescription()));
            }
            return Json(new { obj = newSortingExcuteStatusList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PickingExcuteStatus()
        {
            //return Json(new { obj = Enum.GetValues(typeof(CgPickingExcuteStatus)).Cast<CgPickingExcuteStatus>().Except<CgPickingExcuteStatus>(new CgPickingExcuteStatus[] { CgPickingExcuteStatus.Completed }).Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);

            List<NewSource<CgPickingExcuteStatus>> newCgPickingExcuteStatusList = new List<NewSource<CgPickingExcuteStatus>>();
            foreach (CgPickingExcuteStatus item in Enum.GetValues(typeof(CgPickingExcuteStatus)))
            {
                if (item == CgPickingExcuteStatus.Completed)
                {
                    continue;
                }
                newCgPickingExcuteStatusList.Add(new NewSource<CgPickingExcuteStatus>(item, item.GetDescription()));
            }
            return Json(new { obj = newCgPickingExcuteStatusList }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TempStockExcuteStatus()
        {
            return Json(new { obj = Enum.GetValues(typeof(TempStockExcuteStatus)).Cast<TempStockExcuteStatus>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ExcuteStatus()
        {
            return Json(new { obj = Enum.GetValues(typeof(ExcuteStatus)).Cast<ExcuteStatus>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        }

        private class NewSource<T>
        {
            public T value { get; set; }
            public string name { get; set; }

            public NewSource(T value, string name)
            {
                this.value = value;
                this.name = name;
            }
        }

        public ActionResult NoticeSource()
        {
            //return Json(new { obj = Enum.GetValues(typeof(CgNoticeSource)).Cast<CgNoticeSource>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
            
            List<NewSource<CgNoticeSource>> newNoticeSourceList = new List<NewSource<CgNoticeSource>>();
            foreach (CgNoticeSource item in Enum.GetValues(typeof(CgNoticeSource)))
            {
                newNoticeSourceList.Add(new NewSource<CgNoticeSource>(item, item.GetDescription()));
            }
            return Json(new { obj = newNoticeSourceList }, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult Currency()
        {
            return Json(new { obj = Enum.GetValues(typeof(Currency)).Cast<Currency>().Where(item => item != 0).Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult WaybillType()
        {
            //return Json(new { obj = Enum.GetValues(typeof(WaybillType)).Cast<WaybillType>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);

            List<NewSource<WaybillType>> newWaybillTypeList = new List<NewSource<WaybillType>>();
            foreach (WaybillType item in Enum.GetValues(typeof(WaybillType)))
            {
                newWaybillTypeList.Add(new NewSource<WaybillType>(item, item.GetDescription()));
            }
            return Json(new { obj = newWaybillTypeList }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult BoxingSpecs()
        {
            return Json(new { obj = Enum.GetValues(typeof(BoxingSpecs)).Cast<BoxingSpecs>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult NoticeType()
        //{
        //    return Json(new { obj = Enum.GetValues(typeof(NoticeType)).Cast<NoticeType>().Select(item => new { value = item, name = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        //}
    }
}