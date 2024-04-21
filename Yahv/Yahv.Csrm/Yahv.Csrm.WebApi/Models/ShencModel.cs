using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApi.Models
{
    public class ShencModel
    {
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        public Enterprise Enterprise { set; get; }
        public SiteUserXdt SiteUser { set; get; }
        //public WsClient Client { set; get; }
        public WsContact Contact { set; get; }
        public FileDescription BusinessLicense { set; get; }

        

    }


}