using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Needs.FrontEnd
{
    abstract public class ClientController : Controller
    {
        protected ClientController()
        {

        }

        JsonResult jsonResult;

        JsonResult eJson(object data)
        {
            return this.jsonResult = this.Json(data) ?? this.jsonResult;
        }

        public ActionResult eContent(string relativeUri, object model = null)
        {
            string html = FedItems.Current[relativeUri].Html;
            if (model != null)
            {
                StringBuilder builder = new StringBuilder();
                string json = JsonConvert.SerializeObject(model, Formatting.None);
                Regex regex = new Regex("<head.*?>");
                Match match = regex.Match(html);
                int startIndex = match.Index + match.Value.Length;
                builder.AppendLine().Append('\t');
                builder.Append("<script>")
                    .Append("var ").Append("Model").Append('=').Append(json).Append(';')
                .Append("</script>");
                html = html.Insert(startIndex, builder.ToString());
            }

            return Content(html, "text/html", FedItems.Current.Encoding);
        }
    }



    abstract public class ValidatesController : Controller
    {
        protected ValidatesController()
        {

        }

        JsonResult jsonResult;

        virtual public ActionResult ValidateUserName(object data)
        {
            return this.jsonResult = this.Json(data) ?? this.jsonResult;
        }
        virtual public ActionResult ValidateMoblie(object data)
        {
            return this.jsonResult = this.Json(data) ?? this.jsonResult;
        }

    }


    abstract public class LoginController : ValidatesController
    {
        protected LoginController()
        {

        }

        public override ActionResult ValidateUserName(object data)
        {
            return base.ValidateUserName(data);
        }
    }
}