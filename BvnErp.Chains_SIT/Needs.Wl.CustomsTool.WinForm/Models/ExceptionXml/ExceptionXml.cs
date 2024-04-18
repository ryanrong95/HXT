using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    public class ExceptionXml
    {
        public class Root
        {
            public string resultFlag { get; set; } = string.Empty;

            public string failCode { get; set; } = string.Empty;

            public string failInfo { get; set; } = string.Empty;

            public string retData { get; set; } = string.Empty;
        }

        public class DecImportResponse
        {
            public string ResponseCode { get; set; } = string.Empty;

            public string ErrorMessage { get; set; } = string.Empty;

            public string ClientSeqNo { get; set; } = string.Empty;
        }
    }
}
