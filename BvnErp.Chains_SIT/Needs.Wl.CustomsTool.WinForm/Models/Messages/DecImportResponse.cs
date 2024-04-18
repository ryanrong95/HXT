using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Messages
{
    /// <summary>
    /// 暂存回执数据
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
        //public string TrnPreId { get; set; }
    }
}
