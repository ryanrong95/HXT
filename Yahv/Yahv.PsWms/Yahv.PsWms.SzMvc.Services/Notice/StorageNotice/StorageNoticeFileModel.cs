using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Notice
{
    public class StorageNoticeFileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UploadItem[] Upload { get; set; }

        public class File
        {
            /// <summary>
            /// 
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CustomName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SiteuserID { get; set; }
        }

        public class UploadItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string MainID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public File[] Files { get; set; }
        }
    }
}
