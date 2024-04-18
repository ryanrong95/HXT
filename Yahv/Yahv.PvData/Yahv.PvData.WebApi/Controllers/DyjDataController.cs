using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using Yahv.Web.Mvc.Filters;
using YaHv.DyjData.Services;
using YaHv.PvData.Services.Models;

namespace Yahv.PvData.WebApi.Controllers
{
    /// <summary>
    /// 大赢家数据接口
    /// </summary>
    public class DyjDataController : ClientController
    {
        /// <summary>
        /// 品牌数据
        /// </summary>
        /// <param name="key">索引 :Key for Name</param>
        /// <returns>品牌数据</returns>
        /// <remarks>
        /// http://hv.erp.b1b.com/PvDataApi/DyjData/Manufacturers?key=Apex
        /// </remarks>
        public ActionResult Manufacturers(string key = null)
        {
            string callback = Request.QueryString["callback"];

            string json = DyjManufacturerView.Current[key].Json();

            //建议用执行过滤器实现
            return Content($"{callback}({json})", "application/json", Encoding.UTF8);
        }

        [HttpPayload]
        public ActionResult ManufacturersValidator(JArrayPost jarry)
        {
            var arry = jarry.ToObject<string[]>();

            string callback = Request.QueryString["callback"];

            //string json = DyjManufacturerView.Current[key].Json();

            //建议用执行过滤器实现
            //return Content($"{callback}({json})", "application/json", Encoding.UTF8);
            return Content($"{callback}", "application/json", Encoding.UTF8);
        }
    }
}