using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.CusService.Models
{
    /// <summary>
    /// 报关单导入系统响应报文
    /// </summary>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.chinaport.gov.cn/dec")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.chinaport.gov.cn/dec", IsNullable = false)]
    public class DecImportResponse
    {
        public string ResponseCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ClientSeqNo { get; set; }
        public string SeqNo { get; set; }
        public string TrnPreId { get; set; }

        /// <summary>
        /// 拓展-真实回执时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }

        /// <summary>
        /// 拓展-回执文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 拓展-回执文件备份路径
        /// </summary>
        public string BackupUrl { get; set; }
    }
}
