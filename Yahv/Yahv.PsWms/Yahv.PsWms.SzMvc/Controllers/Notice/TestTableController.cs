using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;

namespace Yahv.PsWms.SzMvc.Controllers
{
    [LoginCheck(IsNeedCheck = false)]
    public partial class TestTableController : Controller
    {
        public JsonResult InsertTEntity(NewTabModel model)
        {
            try
            {
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert<Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar>(new Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar
                    {
                        MainID0 = model.MainID0,
                        ColVarchar = model.ColVarchar,
                        ColVarcharNull = model.ColVarcharNull,
                        ColNvarchar = model.ColNvarchar,
                        ColNvarcharNull = model.ColNvarcharNull,
                        ColInt = model.ColInt,
                        ColIntNull = model.ColIntNull,
                        ColDatetime = model.ColDatetimeReal,
                        ColDatetimeNull = model.ColDatetimeNullReal,
                        ColDecimal = model.ColDecimal,
                        ColDecimalNull = model.ColDecimalNull,
                        ColBit = model.ColBit,
                        ColBitNull = model.ColBitNull,
                        ColDate = model.ColDateReal,
                        ColDateNull = model.ColDateNullReal,
                        ColBigint = model.ColBigint,
                        ColBigintNull = model.ColBigintNull,
                    });
                }

                return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message + " ---- " + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertTEntities(List<NewTabModel> models)
        {
            try
            {
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert<Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar>(
                        models.Select(model => new Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar
                        {
                            MainID0 = model.MainID0,
                            ColVarchar = model.ColVarchar,
                            ColVarcharNull = model.ColVarcharNull,
                            ColNvarchar = model.ColNvarchar,
                            ColNvarcharNull = model.ColNvarcharNull,
                            ColInt = model.ColInt,
                            ColIntNull = model.ColIntNull,
                            ColDatetime = model.ColDatetimeReal,
                            ColDatetimeNull = model.ColDatetimeNullReal,
                            ColDecimal = model.ColDecimal,
                            ColDecimalNull = model.ColDecimalNull,
                            ColBit = model.ColBit,
                            ColBitNull = model.ColBitNull,
                            ColDate = model.ColDateReal,
                            ColDateNull = model.ColDateNullReal,
                            ColBigint = model.ColBigint,
                            ColBigintNull = model.ColBigintNull,
                        }).ToArray());
                }

                return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message + " ---- " + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertIEnumerableTEntities(List<NewTabModel> models)
        {
            try
            {
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert<Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar>(
                        models.Select(model => new Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar
                        {
                            MainID0 = model.MainID0,
                            ColVarchar = model.ColVarchar,
                            ColVarcharNull = model.ColVarcharNull,
                            ColNvarchar = model.ColNvarchar,
                            ColNvarcharNull = model.ColNvarcharNull,
                            ColInt = model.ColInt,
                            ColIntNull = model.ColIntNull,
                            ColDatetime = model.ColDatetimeReal,
                            ColDatetimeNull = model.ColDatetimeNullReal,
                            ColDecimal = model.ColDecimal,
                            ColDecimalNull = model.ColDecimalNull,
                            ColBit = model.ColBit,
                            ColBitNull = model.ColBitNull,
                            ColDate = model.ColDateReal,
                            ColDateNull = model.ColDateNullReal,
                            ColBigint = model.ColBigint,
                            ColBigintNull = model.ColBigintNull,
                        }));
                }

                return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message + " ---- " + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateObjectEntity(UpdateModel updateModel)
        {
            try
            {
                var newTabObj = updateModel.NewTabObj;

                var newTabModel = new //Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar
                {
                    ColVarchar = newTabObj.ColVarchar,
                    ColVarcharNull = newTabObj.ColVarcharNull,
                    ColNvarchar = newTabObj.ColNvarchar,
                    ColNvarcharNull = newTabObj.ColNvarcharNull,
                    ColInt = newTabObj.ColInt,
                    ColIntNull = newTabObj.ColIntNull,
                    ColDatetime = newTabObj.ColDatetimeReal,
                    ColDatetimeNull = newTabObj.ColDatetimeNullReal,
                    ColDecimal = newTabObj.ColDecimal,
                    ColDecimalNull = newTabObj.ColDecimalNull,
                    ColBit = newTabObj.ColBit,
                    ColBitNull = newTabObj.ColBitNull,
                    ColDate = newTabObj.ColDateReal,
                    ColDateNull = newTabObj.ColDateNullReal,
                    ColBigint = newTabObj.ColBigint,
                    ColBigintNull = newTabObj.ColBigintNull,
                };

                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    foreach (var priKeyObj in updateModel.PriKeyObjs)
                    {
                        repository.Update<Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar>(newTabModel, t => t.MainID0 == priKeyObj.MainID0);
                    }
                }

                return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message + " ---- " + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(List<PriKeyModel> priKeyObjs)
        {
            try
            {
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    foreach (var priKeyObj in priKeyObjs)
                    {
                        repository.Delete<Layers.Data.Sqls.PsOrder.MonoPriNoIncVarchar>(t => t.MainID0 == priKeyObj.MainID0);
                    }
                }

                return Json(new { type = "success", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message + " ---- " + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}