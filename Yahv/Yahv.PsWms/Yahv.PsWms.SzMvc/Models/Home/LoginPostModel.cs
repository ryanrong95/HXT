using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class LoginPostModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RemberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}