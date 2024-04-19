using NtErp.Wss.Sales.Services.Underly.Products;
using NtErp.Wss.Sales.Services.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class StandardProduct : IUnique, INaming, IStandardProduct, IProduct
    {

        Document properties;

        public StandardProduct()
        {
            this.properties = new Document();
        }

        public Document Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties = value;
            }
        }

        public Elements this[string index]
        {
            get { return this.properties[index]; }
            set { this.properties[index] = value; }
        }

        protected string GenID(params string[] arry)
        {
            string keyid = string.Concat(arry).MD5();
            string setid = this["Set ID"];

            if (string.IsNullOrWhiteSpace(setid))
            {
                return keyid;
            }

            if (setid == keyid)
            {
                return keyid;
            }

            return setid;
        }

        /// <summary>
        /// 这个容后设计
        /// </summary>
        virtual public string ID
        {
            get
            {
                return this.GenID(this.Name, this.B1bSign, this.Manufacturer);
            }
            set
            {
                this["Set ID"] = this[nameof(this.ID)] = value;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        [OldNaming("Title")]
        [StandardNaming("Manufacturer Part Number")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Name
        {
            get { return this[nameof(this.Name)]; }
            set
            {
                this[nameof(this.Name)] = value;
            }
        }

        /// <summary>
        /// b1b 标识
        /// </summary>
        [StandardNaming("B1b Part Number")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string B1bSign
        {
            get { return this[nameof(this.B1bSign)]; }
            set
            {
                this[nameof(this.B1bSign)] = value;
            }
        }

        /// <summary>
        /// 制造商
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Manufacturer
        {
            get { return this[nameof(this.Manufacturer)]; }
            set
            {
                this[nameof(this.Manufacturer)] = value;
            }
        }

        /// <summary>
        /// 暂时进行综合扩展
        /// </summary>
        public Catalogs Catalogs { get; set; }


        /// <summary>
        /// 禁运 （暂时放在这里）
        /// </summary>
        public Embargos Embargos { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Weight { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public Attachments Attachments { get; set; }
    }
}
