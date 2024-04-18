using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetEnumValueRetuenModel
    {
        /// <summary>
        /// 枚举值
        /// </summary>
        public EnumValue[] EnumValues { get; set; }


        public class EnumValue
        {
            public string Name { get; set; }

            public string Value { get; set; }

            public string Description { get; set; }
        }
    }
}