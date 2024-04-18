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
            using (var view = new Needs.Ccs.Services.Views.ApiClientsView())
            {
                var client = view.Where(item => item.AccountName == userName && item.Password == userPassword);
                if (client != null)
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