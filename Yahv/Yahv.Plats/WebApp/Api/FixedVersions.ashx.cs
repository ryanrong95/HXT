using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Utils.Serializers;

namespace WebApp.Api
{
    /// <summary>
    /// FixedVersions 的摘要说明
    /// </summary>
    public class FixedVersions : Yahv.Web.Handlers.JsonHandler
    {

        protected override void OnProcessRequest(HttpContext context)
        {
            string json = Yahv.Underly.DomainConfig.FixedVersion.Json();
            context.Response.Write(json);
        }
    }
}