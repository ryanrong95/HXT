using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetLogDetailSearchModel
    {
        public string LogID { get; set; }
    }

    public class GetLogDetailReturnModel
    {
        /// <summary>
        /// LogID
        /// </summary>
        public string LogID { get; set; }

        /// <summary>
        /// ActionName
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// MainID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// CreateDateDes
        /// </summary>
        public string CreateDateDes { get; set; }
    }
}