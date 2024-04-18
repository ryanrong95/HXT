using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterFeeFile
    {
        public string FileName { get; set; }
        public string FileFormat { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// 1:费用附件，2：支付附件
        /// </summary>
        public int FileType { get; set; }
    }

    public enum CenterFeeFileType
    {
        
        FeeFile = 1,

     
        PayFile = 2
    }
}
