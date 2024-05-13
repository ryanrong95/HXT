//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
////using WebApp.Services;
//using Yahv.Underly;

//namespace MvcApp.Controllers
//{
//    public class FromTypeController : Controller
//    {
//        // GET: FromType   js引用
//        public string Index()
//        {
//            var values = Enum.GetValues(typeof(WebApp.Services.FromType));


//            StringBuilder content = new StringBuilder();
//            content.Append("var fromtype={");

//            var scheme = nameof(WebApp.Services.FromType.Scheme);
//            foreach (var item in values)
//            {
                
//                if (item.ToString() == scheme)
//                {
//                    content.Append(string.Concat(item.ToString().ToLower(), ":\"", ((WebApp.Services.FromType)item).GetDescription().ToLower(), "\","));
//                }
//                else
//                {
//                    content.Append(string.Concat(item.ToString().ToLower(), ":\"", string.Concat(WebApp.Services.FromType.Scheme.GetDescription().ToLower(), "://",((WebApp.Services.FromType)item).GetDescription().ToLower()), "\","));
//                }
//            }
//            content.Append("}");
//            return content.ToString();
//        }
//    }
//}