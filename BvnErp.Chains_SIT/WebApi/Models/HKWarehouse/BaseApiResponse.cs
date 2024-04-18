using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class BaseApiResponse
    {
        public bool Success { get; set; } = true;

        public string Msg { get; set; } = string.Empty;
    }

    public class BasePageApiResponse : BaseApiResponse
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int NCount { get; set; }
    }
}