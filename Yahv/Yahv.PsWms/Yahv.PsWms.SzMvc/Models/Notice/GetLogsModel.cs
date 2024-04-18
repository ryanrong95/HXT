using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetLogsSearchModel
    {
        /// <summary>
        /// page
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// rows
        /// </summary>
        public int rows { get; set; }
    }
}