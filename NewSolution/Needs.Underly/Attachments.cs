using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    public class NetFile
    {
        public string Name { get; set; }

        public string Url { get; set; }

       
    }    

    /// <summary>
    /// 产品附件
    /// </summary>
    public class Attachments
    {
        /// <summary>
        /// 肖像
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public NetFile Image
        {
            get
            {
                return this.Images.FirstOrDefault() ?? new NetFile
                {
                    Name = "",
                    Url = ""
                };
            }
        }

        /// <summary>
        /// 图片组
        /// </summary>
        [System.Xml.Serialization.XmlElement("Images")]
        public NetFile[] Images { get; set; }

        /// <summary>
        /// 文档组
        /// </summary>
        [System.Xml.Serialization.XmlElement("Files")]
        public NetFile[] Files { get; set; }

        public Attachments()
        {
            this.Images = new NetFile[0];
            this.Files = new NetFile[0];
        }
    }
}
