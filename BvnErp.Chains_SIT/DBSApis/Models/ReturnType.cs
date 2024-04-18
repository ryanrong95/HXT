using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Models
{
    public class ReturnType
    {
        public bool IsSuccess { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}