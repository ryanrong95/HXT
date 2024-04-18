using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetLogListSearchModel
    {
        /// <summary>
        /// page
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// rows
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// ActionNameInt
        /// </summary>
        public string ActionNameInt { get; set; }

        /// <summary>
        /// MainID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// CreateDateBegin
        /// </summary>
        public string CreateDateBegin { get; set; }

        /// <summary>
        /// CreateDateEnd
        /// </summary>
        public string CreateDateEnd { get; set; }
    }

    public class GetLogListReturnModel
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
        /// CreateDateDes
        /// </summary>
        public string CreateDateDes { get; set; }
    }
}