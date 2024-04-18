using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.Services.Models
{
    public class UploadResult
    {
        public string SessionID { get; set; }
        public string FileID { get; set; }
        public string WebUrl { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}