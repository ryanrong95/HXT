using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace WebApi.Models
{
    public class CustomBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        public override bool OnAuthorize(string userName, string userPassword, HttpActionContext actionContext)
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.ApiClients>().Where(item => item.AccountName == userName && item.Password == userPassword).Count();

                if (count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }               
            }
        }
    }
}