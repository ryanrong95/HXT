using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetEnumsReturnModel
    {
        /// <summary>
        /// 枚举信息
        /// </summary>
        public EnumInfo[] EnumInfos { get; set; }


        public class EnumInfo
        {

            /// <summary>
            /// 枚举名称
            /// </summary>
            public string EnumName { get; set; }
        }
    }
}