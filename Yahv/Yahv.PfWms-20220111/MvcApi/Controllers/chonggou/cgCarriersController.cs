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
    public class cgCarriersController : ClientController
    {
        static cgCarriersController()
        {

        }

        /// <summary>
        /// 获取承运商
        /// </summary>
        /// <param name="whCode">库房标识</param>
        /// <returns></returns>
        /// <remarks>
        /// http://hv.warehouse.b1b.com/wmsapi/cgCarriers/Alls/?whCode=HK
        /// http://hv.warehouse.b1b.com/wmsapi/cgCarriers/Alls/?whCode=SZ
        /// </remarks>
        public ActionResult Alls(string whCode)
        {
            string origin = null;

            using (Wms.Services.chonggous.Views.CarriersTopView view = new Wms.Services.chonggous.Views.CarriersTopView())
            {
                IQueryable<Wms.Services.chonggous.Views.Carrier> iquery = null;
                if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.HK)))
                {
                    origin = Origin.HKG.ToString();
                    iquery = view.Where(item => item.Place == origin
                      || item.ID == Yahv.Services.Models.Carrier.Personal.ID);
                }
                if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.SZ)))
                {
                    origin = Origin.CHN.ToString();
                    iquery = view.Where(item => item.Place == origin
                       || item.ID == Yahv.Services.Models.Carrier.Personal.ID
                       || item.ID == Yahv.Services.Models.Carrier.XdtPCL.ID);
                }

                var arry = iquery.Select(item => new
                {
                    item.ID,
                    item.Name
                }).ToArray();

                return Json(arry, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取承运商
        /// </summary>
        /// <param name="waybillType">运单类型</param>
        /// <param name="whCode">库房标识</param>
        /// <param name="noticeType">运单类型的通知类型</param>
        /// <returns></returns>
        /// <remarks>
        /// http://hv.warehouse.b1b.com/wmsapi/cgCarriers/Show/?waybillType=4
        /// </remarks>
        public ActionResult Show(int waybillType, string whCode, int noticeType = 100)
        {
            string origin = null;

            if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.HK)))
            {
                origin = Origin.HKG.ToString();
            }
            if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.SZ)))
            {
                origin = Origin.CHN.ToString();
            }
            //Yahv.Services.Enums.CgNoticeType noticeType = Yahv.Services.Enums.CgNoticeType.Enter;

            var type = (WaybillType)waybillType;
            using (Wms.Services.chonggous.Views.CarriersTopView view = new Wms.Services.chonggous.Views.CarriersTopView())
            {
                IQueryable<Wms.Services.chonggous.Views.Carrier> iquery;
                switch (type)
                {
                    case WaybillType.PickUp:
                        if (whCode.StartsWith(nameof(Yahv.Services.WhSettings.SZ)))
                        {
                            iquery = view.Where(item => item.Type == CarrierType.Logistics || item.ID == Yahv.Services.Models.Carrier.XdtPCL.ID);
                        }
                        else
                        {
                            iquery = view.Where(item => item.Type == CarrierType.Logistics);
                        }
                        break;
                    case WaybillType.DeliveryToWarehouse:
                        iquery = view;
                                                
                        break;
                    case WaybillType.LocalExpress:
                        iquery = view.Where(item => item.Type == CarrierType.Express);
                        break;
                    case WaybillType.InternationalExpress:
                        iquery = view.Where(item => item.Type == CarrierType.Express && item.IsInternational == true);
                        break;
                    default:
                        throw new NotSupportedException("不支持指定的类型:" + type.ToString());
                }


                if (type == WaybillType.PickUp)
                {
                    iquery = iquery.Where(item => item.Place == origin || item.ID == Yahv.Services.Models.Carrier.Personal.ID);
                }
                else if (noticeType == (int)Yahv.Services.Enums.CgNoticeType.Out && whCode.StartsWith(nameof(Yahv.Services.WhSettings.HK)))
                {
                    iquery = view.Where(item => item.Place == origin || item.ID == Yahv.Services.Models.Carrier.Personal.ID);
                }
                else
                {
                    iquery = iquery.Where(item => item.Place == origin);
                }

                var arry = iquery.Select(item => new
                {
                    item.ID,
                    item.Name
                }).ToArray();

                return Json(arry, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 特殊获取芯达通物流部
        /// </summary>
        /// <returns></returns>
        public ActionResult Xdt()
        {

            using (Yahv.Services.Views.CarriersTopView view = new Yahv.Services.Views.CarriersTopView())
            {
                return Json(view.Where(item => item.ID == Yahv.Services.Models.Carrier.XdtPCL.ID).Select(item => new
                {
                    item.ID,
                    item.Name
                }).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Drivers(string id)
        {
            using (Yahv.Services.Views.TransportTopView tview = new Yahv.Services.Views.TransportTopView())
            using (Yahv.Services.Views.DriversTopView dview = new Yahv.Services.Views.DriversTopView())
            {
                var transports = tview.Where(item => item.EnterpriseID == id).ToArray();
                var drivers = dview.Where(item => item.EnterpriseID == id).ToArray();

                return Json(new
                {
                    Transports = transports.Select(item => new
                    {
                        item.ID,
                        item.CarNumber1,
                        item.CarNumber2,
                        item.Type,
                        item.Weight,
                    }),
                    Drivers = drivers.Select(item => new
                    {
                        item.ID,
                        item.Name,
                        item.Mobile,
                    }),
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}