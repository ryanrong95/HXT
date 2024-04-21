using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Views.Rolls;

namespace Yahv.Finance.WebApi.Filter
{
    /// <summary>
    /// 接口验证
    /// </summary>
    public class SenderAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = false;

            var stream = httpContext.Request.InputStream;
            var reader = new System.IO.StreamReader(stream);
            var json = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(json))
            {
                var input = JsonConvert.DeserializeObject<InputParam<object>>(json);

                if (input != null && !string.IsNullOrEmpty(input.Sender))
                {
                    using (var sendersView = new SendersRoll())
                    {
                        result = sendersView.Any(
                            item => item.ID == input.Sender || item.SecretKey == input.Sender);
                    }
                }
            }

            //不重新设置位置，接口获取不到数据
            httpContext.Request.InputStream.Position = 0;

            return result;
        }
    }
}