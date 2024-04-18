using Layers.Data.Sqls;
using Layers.Data.Sqls.PsOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class TestTableController : Controller
    {
        public ActionResult DoForeignData()
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                //原动作：
                //MonoPriA MonoPriB (MonoPriA 依赖 MonoPriB)

                //Insert Insert
                repository.Insert<MonoPriB>(new MonoPriB { B1 = "b11", B2 = "b22" });
                repository.Insert<MonoPriA>(new MonoPriA { A1 = "a11", A2 = "b11" });

                repository.Insert<MonoPriB>(new MonoPriB { B1 = "b33", B2 = "b44" });
                repository.Insert<MonoPriA>(new MonoPriA { A1 = "a33", A2 = "b33" });

                repository.Insert<MonoPriB>(new MonoPriB { B1 = "b55", B2 = "b66" });
                repository.Insert<MonoPriA>(new MonoPriA { A1 = "a55", A2 = "b55" });

                //Delete Delete
                repository.Delete<MonoPriA>(t => t.A1 == "a55");
                repository.Delete<MonoPriB>(t => t.B1 == "b55");


                //Delete Insert
                repository.Delete<MonoPriA>(t => t.A1 == "a33");
                repository.Insert<MonoPriB>(new MonoPriB { B1 = "b55", B2 = "b66" });
                ////Insert Delete
                //repository.Insert<MonoPriA>(new MonoPriA { A1 = "", A2 = "" });
                //repository.Delete<MonoPriB>(t => t.B1 == "");

                ////Delete Update
                //repository.Delete<MonoPriA>(t => t.A1 == "");
                //repository.Update<MonoPriB>(new { B1 = "", B2 = "" }, t => t.B1 == "");
                ////Insert Update
                //repository.Insert<MonoPriA>(new MonoPriA { A1 = "", A2 = "" });
                //repository.Update<MonoPriB>(new { B1 = "", B2 = "" }, t => t.B1 == "");

                ////Update Delete
                //repository.Update<MonoPriA>(new { A1 = "", A2 = "" }, t => t.A1 == "");
                //repository.Delete<MonoPriB>(t => t.B1 == "");
                ////Update Insert
                //repository.Update<MonoPriA>(new { A1 = "", A2 = "" }, t => t.A1 == "");
                //repository.Insert<MonoPriB>(new MonoPriB { B1 = "", B2 = "" });

                ////Update Update
                //repository.Update<MonoPriA>(new { A1 = "", A2 = "" }, t => t.A1 == "");
                //repository.Update<MonoPriB>(new { B1 = "", B2 = "" }, t => t.B1 == "");
            }

            return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WriteRandomData()
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert<MonoPriB>(new MonoPriB { B1 = Guid.NewGuid().ToString("N"), B2 = Guid.NewGuid().ToString("N") });
            }

            return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}