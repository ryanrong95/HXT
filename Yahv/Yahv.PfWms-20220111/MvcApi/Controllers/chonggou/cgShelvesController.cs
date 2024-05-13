using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;

namespace MvcApi.Controllers
{
    public class cgShelvesController : Controller
    {
        //static cgShelvesController()
        //{
        //    ShelveManage.Current.ShelveLeased += Current_ShelveLeased;
        //}



        /// <summary>
        /// 设置库区
        /// </summary>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult SetRegion(JPost jpost)
        {
            var arguments = new
            {
                whCode = jpost["whCode"]?.Value<string>(),//库房编号
                region = jpost["region"]?.Value<string>(),//库区编号
                name = jpost["name"]?.Value<string>()//库区名称
            };

            ShelveManage.Current.SetRegion(arguments.whCode, arguments.region, arguments.name);

            return Json(new JMessage
            {
                success = true,
                code = 200,
                data = "保存成功!"
            });
        }

        /// <summary>
        /// 设置库位
        /// </summary>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult SetPlace(JPost jpost)
        {
            var arguments = new
            {
                whCode = jpost["whCode"]?.Value<string>(),
                place = jpost["place"]?.Value<string>()
            };

            var istrue = ShelveManage.Current.SetPlace(arguments.whCode, arguments.place);
            if (istrue == true)
            {
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "保存成功!"
                });
            }
            else
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "库位已存在，保存失败!"
                });
            }

        }

        /// <summary>
        /// 设置卡板
        /// </summary>
        /// <param name="whCode">库房编号</param>
        /// <param name="pallets">卡板名称</param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult SetPallet(JPost jpost)
        {
            var arguments = new
            {
                whCode = jpost["whCode"]?.Value<string>(),
                pallet = jpost["pallet"].Value<string>(),
            };

            var istrue = ShelveManage.Current.SetPallet(arguments.whCode, arguments.pallet);

            if (istrue == true)
            {
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "保存成功!"
                });
            }
            else
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "卡板已存在，保存失败!"
                });
            }

        }

        /// <summary>
        /// 获得所有库区
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowRegions(string whid, int pageIndex = 1, int pageSize = 20)
        {
            return Json(new { obj = ShelveManage.Current.ToRegions(whid, pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得所有卡板
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowPallets(string whid, int pageIndex = 1, int pageSize = 20)
        {
            return Json(new { obj = ShelveManage.Current.ToPallets(whid, pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有库位
        /// </summary>
        /// <param name="whid"></param>
        /// <returns></returns>
        public ActionResult ShowPlaces(string whid, int pageIndex = 1, int pageSize = 20)
        {

            return Json(new { obj = ShelveManage.Current.ToPlaces(whid, pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ForSelect()
        //{

        //    return null;
        //}

        /// <summary>
        /// 获取可用库位号
        /// </summary>
        /// <param name="whid">库房ID</param>
        /// <returns></returns>
        public ActionResult GetUsableShelves(string whid)
        {
            //?不用获得哪个客户的吗
            return Json(new { obj = ShelveManage.Current[whid] }, JsonRequestBehavior.AllowGet);
        }

        //int code = 200;
        //string message = "删除成功";

        /// <summary>
        /// 删除库区、卡板、库位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {

            try
            {
                var current = ShelveManage.Current;
                //current.ShelveLeased += Current_ShelveLeased;
                current.Delete(id);

                //if (code == 200)
                //{
                return Json(new JMessage
                {
                    success = true,
                    code = /*code*/200,
                    data = "删除成功!"
                });
                //}
                //else
                //{
                //    return Json(new JMessage
                //    {
                //        success = true,
                //        code = code,
                //        data = message
                //    });
                //}

            }
            catch (ShelveLeasedException ex)
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = ex.Message
                });
            }

        }

        /// <summary>
        /// 深圳库位号的展示
        /// </summary>
        public ActionResult SZShow(string whCode)
        {
            return Json(ShelveManage.Current.SZShow(whCode), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 深圳乱入库位
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult SZEnter(JPost jpost)
        {
            var arguments = new
            {
                whCode = jpost["whCode"]?.Value<string>(),
                place = jpost["place"]?.Value<string>()
            };

           ShelveManage.Current.SZEnter(arguments.whCode, arguments.place);
            return Json(new 
            {
                code = 200
            });
        }

        //private  void Current_ShelveLeased(object sender, EventArgs e)
        //{
        //    code = 400;
        //    message = sender.ToString();
        //}

        //public ActionResult Index(string index)
        //{

        //}

    }
}