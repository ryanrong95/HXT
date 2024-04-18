using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class ExpressMethodsRequestModel
    {
        /// <summary>
        /// 快递公司
        /// </summary>
        public string[] ExpressCompanies { get; set; }
    }

    public class ExpressMethodsReturnModel
    {
        ///// <summary>
        ///// 快递方式
        ///// </summary>
        //public ExpressMethodSingle[] ExpressMethods { get; set; }

        public class ExpressMethodSingle
        {
            /// <summary>
            /// 快递公司名称
            /// </summary>
            public string ExpressName { get; set; }

            /// <summary>
            /// 快递方式 value text
            /// </summary>
            public Content[] Values { get; set; }
        }

        public class Content
        {
            public string value { get; set; }

            public string text { get; set; }
        }
    }
}