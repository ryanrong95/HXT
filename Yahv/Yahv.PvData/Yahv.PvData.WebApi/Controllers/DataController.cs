using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.PvData.Services;
using YaHv.PvData.Services.Views.Alls;

namespace Yahv.PvData.WebApi.Controllers
{
    /// <summary>
    /// 数据接口
    /// </summary>
    public class DataController : ClientController
    {
        /// <summary>
        /// 根据型号、品牌查询归类信息、Eccn编码
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ClassifiedInfo(string partNumber, string manufacturer)
        {
            var data = SqlView.ClassifiedInfo(partNumber, manufacturer);
            if (data == null)
            {
                return Json(new JSingle<object>
                {
                    code = 100,
                    success = false,
                    data = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
            //未查询到数据
            else
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询标准历史价格
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult StandardPastQuotes(string partNumber, string manufacturer)
        {
            var data = SqlView.StandardPastQuotes(partNumber, manufacturer);
            if (data.Any())
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data.Select(item => new
                    {
                        item.PartNumber,
                        item.Manufacturer,
                        item.Currency,
                        item.UnitPrice,
                        item.Quantity,
                        CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JSingle<object>
                {
                    code = 100,
                    success = false,
                    data = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询标准型号
        /// </summary>
        /// <param name="name">型号名称</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult StandardPartnumbers(string name)
        {
            var data = new StandardPartnumbersAll()[name].Select(item => item.Name).ToArray();
            if (data.Any())
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JSingle<object>
                {
                    code = 100,
                    success = false,
                    data = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询标准品牌
        /// </summary>
        /// <param name="name">品牌名称</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult StandardManufacturers(string name)
        {
            var data = new StandardManufacturersAll()[name].Select(item => item.Name).ToArray();
            if (data.Any())
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JSingle<object>
                {
                    code = 100,
                    success = false,
                    data = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询Eccn编码
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Eccn(string partNumber)
        {
            var data = new EccnsAll()[partNumber].ToArray();
            if (data.Any())
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data.First()
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JSingle<object>
                {
                    code = 100,
                    success = false,
                    data = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 提供标准型号搜索
        /// </summary>
        /// <param name="name">型号名称（做前缀匹配）</param>
        /// <returns>默认返回50条数据，做自动完成的备选列表</returns>
        /// <remarks>
        /// 请王增超协助完成，响应时间争取控制在100-200ms
        /// </remarks>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SearchStandardPartnumbers(string name)
        {
            var data = SqlView.StandardPartnumbersForShow(name);
            if (data.Any())
            {
                return Json(new JSingle<object>
                {
                    code = 200,
                    success = true,
                    data = data.GroupBy(item => item.Partnumber).Select(item => item.First())
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // 从 [PvData].[dbo].[StandardPartnumbers]表中获取
                // 判断@@ROWCOUNT; 可以试验一下这个语法
                // 返回 {  "Partnumber":"asdf"}


                return Json(new
                {
                    code = 100,
                    success = false,
                    message = "未查询到数据"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}