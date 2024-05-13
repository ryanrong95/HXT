using Yahv.Utils.EventExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Services.Views;
using MvcApi.Models;
using Yahv.Utils.Kdn;
using Kdn.Library;
using Layers.Data.Sqls;

namespace MvcApp.Controllers
{
    public class KdnController : Controller
    {
        public ActionResult Index()
        {
            return Json(new
            {
                Message = "快递鸟接口",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckAddress(FormCollection forms)
        {
            string address = forms["address"];

            KdnAddressError error;

            if (address.TryAddress(out error))
            {
                return Json(new
                {
                    Success = false,
                    Message = error.Error,
                    Values = error.Values
                });
            }

            return Json(new
            {
                Success = true,
                Message = "验证成功",
            });
        }

        [HttpPost]
        public ActionResult Enter(KdnEntity entity)
        {
            using (KdnRequestTopView requestView = new KdnRequestTopView())
            using (KdnResultTopView resultView = new KdnResultTopView())
            {
                requestView.Enter(entity.Request);
                resultView.Enter(entity.Result);
            }

            return Json(new
            {
                Success = true,
                Message = "成功录入",
            });
        }


        /// <summary>
        /// 根于 query ShipperCode 获取快递类型
        /// </summary>
        /// <returns>返回指定快递公司的全部的快递类型</returns>
        /// <example>http://hv.warehouse.b1b.com/wmsapi/Kdn/GetExpTypes?shipperCode=sf</example>
        [HttpGet]
        public ActionResult GetExpTypes()
        {
            string shipperCode = Request.QueryString["shipperCode"];

            var types = new[] { typeof(SfExpType), typeof(KysyExpType), typeof(EmsExpType) };
            var type = types.SingleOrDefault(item => item.Name.StartsWith(shipperCode,
                StringComparison.OrdinalIgnoreCase));
            var ins = Activator.CreateInstance(type) as CodeType;
            return Json(ins, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取快递公司编码
        /// </summary>
        /// <returns>返回当前可用的快递公司编码</returns>
        /// <remarks>
        /// 暂时只支持两个：顺丰、跨越速运
        /// </remarks>
        /// <example>http://hv.warehouse.b1b.com/wmsapi/Kdn/GetShipperCodes</example>
        [HttpGet]
        public ActionResult GetShipperCodes()
        {
            var arry = new ShipperCode().ToArray();
            return Json(arry, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取计费方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPayTypes()
        {
            List<CodeValue<PayType>> payTypeList = new List<CodeValue<PayType>>();

            foreach (PayType item in Enum.GetValues(typeof(PayType)))
            {

                payTypeList.Add(new CodeValue<PayType>(item.GetDescription(), item));
            }
            return Json(payTypeList, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetPayTypesForEMS()
        {
            List<CodeValue<PayTypeForEMS>> payTypeList = new List<CodeValue<PayTypeForEMS>>();
            foreach (PayTypeForEMS item in Enum.GetValues(typeof(PayTypeForEMS)))
            {
                payTypeList.Add(new CodeValue<PayTypeForEMS>(item.GetDescription(), item));
            }
            return Json(payTypeList, JsonRequestBehavior.AllowGet);               
        }

        /// <summary>
        /// 获取面单内容
        /// </summary>
        /// <returns>面单Html内容</returns>
        [HttpGet]
        public ActionResult GetFaceSheetHtml()
        {
            string logisticCode = Request.QueryString["logisticCode"];

            using (var pvcenter = new PvCenterReponsitory())
            {
                var resultview = new KdnResultTopView().ToArray();
                var requestView = new KdnRequestTopView().ToArray();
                var linq = from request in requestView
                           join result in resultview on request.ID equals result.ID
                           where result.LogisticCode == logisticCode
                           select new
                           {
                               ID = request.ID,
                               LogisticCode = result.LogisticCode,
                               ShipperCode = request.ShipperCode,
                               Html = result.Html
                           };

                var entity = linq.SingleOrDefault();
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            //using (var resultview = new KdnResultTopView())
            //using (var requestView = new KdnRequestTopView())
            //{

            //    var linq = from request in requestView
            //               join result in resultview on request.ID equals result.ID
            //               where result.LogisticCode == logisticCode
            //               select new
            //               {
            //                   ID = request.ID,
            //                   LogisticCode = result.LogisticCode,
            //                   ShipperCode = request.ShipperCode,
            //                   Html = result.Html
            //               };

            //    var entity = linq.SingleOrDefault();
            //    return Json(entity);
            //}
        }
    }
}