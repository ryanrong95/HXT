using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.PvData.Services.Models;

namespace Yahv.PvData.WebApi.Controllers
{
    public class LinqController : ClientController
    {
        [HttpPost]
        public ActionResult ValidateExpression(string expression)
        {
            //ExpressionSerializer serializer = new ExpressionSerializer(new JsonSerializer());
            //var expressionDeserialize = serializer.DeserializeText(expressionText) as Expression<Func<OData.StandardProducts, bool>>;

            var linq = expression.JsonTo<YaHv.PvData.Services.Views.Alls.ProductControlsAll>();

            var json = new JMessage()
            {
                code = 200,
                success = true,
                data = "提交成功"
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}

namespace OData
{
    public class StandardProducts
    {
        public string ID { get; set; }

        public string PartNumber { get; set; }

        public string Manufacturer { get; set; }

        public string PackageCase { get; set; }

        public string Packaging { get; set; }

        public System.DateTime CreateDate { get; set; }
    }
}

