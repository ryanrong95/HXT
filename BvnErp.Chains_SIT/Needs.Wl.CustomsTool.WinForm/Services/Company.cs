using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class Company
    {
        public string Key { get; set; }

        /// <summary>
        /// 公司名称简称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public string FileServerUrl { get; set; }
    }
}