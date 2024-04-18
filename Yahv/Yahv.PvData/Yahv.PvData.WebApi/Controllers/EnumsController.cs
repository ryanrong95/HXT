using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PvData.WebApi.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.PvData.Services;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Interfaces;
using YaHv.PvData.Services.Models;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApi.Controllers
{
    /// <summary>
    /// 枚举数据返回
    /// </summary>
    public class EnumsController : ClientController
    {
        public ActionResult Index()
        {
            return Content("固定枚举数据");
        }


        /// <summary>
        /// 获取询报价的业务类型
        /// </summary>
        /// <returns></returns>
        public ActionResult RqfBussinessType()
        {
            var result = ExtendsEnum.ToDictionary<RqfBussinessType>();
            return Json(result.Select(item => new
            {
                ID = int.Parse(item.Key),
                Name = item.Value,
            }), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 所在地[大陆香港]
        /// </summary>
        /// <returns></returns>
        public ActionResult Range()
        {
            var result = ExtendsEnum.ToDictionary<Range>();
            return Json(result.Select(item => new
            {
                ID = int.Parse(item.Key),
                Name = item.Value,
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 所在地[大陆香港]
        /// </summary>
        /// <returns></returns>
        public ActionResult RqfInvoiceType()
        {
            return Json(Enum.GetValues(typeof(InvoiceType)).Cast<InvoiceType>().
                Where(item => item != InvoiceType.Unkonwn && item != InvoiceType.Customs).
                OrderBy(item => item.GetDescriptions()[1])

                .Select(item => new
                {
                    ID = (int)item,
                    Name = item.GetDescription(),
                }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用量说明
        /// </summary>
        /// <returns></returns>
        public ActionResult QuantityRemark()
        {
            var result = ExtendsEnum.ToDictionary<QuantityRemark>();
            return Json(result.Select(item => new
            {
                ID = int.Parse(item.Key),
                Name = item.Value.ToString(),
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 货期说明
        /// </summary>
        /// <returns></returns>
        public ActionResult TradeType()
        {
            var result = ExtendsEnum.ToDictionary<TradeType>();
            return Json(result.Select(item => new
            {
                ID = int.Parse(item.Key),
                Name = item.Value.ToString(),
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 渠道类型
        /// </summary>
        /// <remarks>
        /// 也是供应商类型
        /// </remarks>
        /// <returns></returns>
        public ActionResult SupplierType()
        {
            var result = ExtendsEnum.ToDictionary<SupplierType>();
            return Json(result.Select(item => new
            {
                ID = int.Parse(item.Key),
                Name = item.Value.ToString(),
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 地区说明
        /// </summary>
        /// <returns></returns>
        public ActionResult AreaType()
        {
            return Json(Enum.GetValues(typeof(AreaType)).Cast<AreaType>().
                OrderByDescending(item => (int)item).
                Select(item => new
                {
                    ID = (int)item,
                    Name = item.GetDescription(),
                }), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 盖章说明
        /// </summary>
        /// <returns></returns>
        public ActionResult SealType()
        {
            return Json(Enum.GetValues(typeof(SealType)).Cast<SealType>().
                Where(item => item != YaHv.PvData.Services.SealType.Unknown)
                .Select(item => new
                {
                    ID = (int)item,
                    Name = item.GetDescription(),
                }), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 盖章说明
        /// </summary>
        /// <returns></returns>
        public ActionResult RqfDyjPayMethord()
        {
            return Json(ExtendsEnum.ToArray(DyjPayMethord.Unkonwn).Select(item => new
            {
                ID = (int)item,
                Name = item.GetDescription()
            }), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 盖章说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Origins()
        {

            return Json(Enum.GetValues(typeof(Origin)).Cast<Origin>().Where(item => item != Origin.NG).Select(item =>
           {
               var origin = item.GetOrigin();
               return new
               {
                   origin.Code,
                   origin.ChineseName,
                   origin.EnglishName,
                   Format = $"({origin.Code}){origin.ChineseName}",
                   OrderIndex = item == Origin.Unknown ? 0 : 1
               };
           }).OrderBy(item => item.OrderIndex).ThenBy(item => item.Code).Select(origin => new
           {
               origin.Code,
               origin.ChineseName,
               origin.EnglishName,
               origin.Format
           }), JsonRequestBehavior.AllowGet);

            //return Json(Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item =>
            //{
            //    var origin = item.GetOrigin();
            //    return new
            //    {
            //        origin.Code,
            //        origin.ChineseName,
            //        origin.EnglishName,
            //        Format = $"({origin.Code}){origin.ChineseName}"
            //    };
            //}).OrderBy(item => item.Code), JsonRequestBehavior.AllowGet);
        }
    }
}