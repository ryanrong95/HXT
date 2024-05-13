using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services;
using Wms.Services.Models;

namespace MvcApi.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            var data = new InvoiceServieces().GetInvoices();
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNoticeFiles(InvoiceNoticeFiles[] files)
        {
            try
            {
                var warehouse = Yahv.Erp.Current.WareHourse;
                warehouse.AddNoticeFiles(files);
                return Json(new { val = 0, msg = "添加成功" });
            }
            catch 
            {

                return Json(new { val = 1, msg = "添加失败" });
            }
          
        }

        public ActionResult DeleteNoticeFiles(string[] invoiceNoticeFileIDs)
        {
            try
            {
                var warehouse = Yahv.Erp.Current.WareHourse;
                warehouse.DeleteNoticeFiles(invoiceNoticeFileIDs);
                return Json(new { val = 0, msg = "删除成功" });
            }
            catch
            {

                return Json(new { val = 1, msg = "删除失败" });
            }
        }
    }
}