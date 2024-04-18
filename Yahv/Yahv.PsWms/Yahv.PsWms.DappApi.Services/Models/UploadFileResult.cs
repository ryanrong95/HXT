using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class UploadFileResult
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 文件的真实可访问的WebUrl地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string CustomName { get; set; }
    }
}
