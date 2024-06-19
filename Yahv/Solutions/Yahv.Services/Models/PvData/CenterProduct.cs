using System;
using Yahv.Linq;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 中心产品信息
    /// </summary>
    public class CenterProduct : IUnique
    {
        private string id;
        public string ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.id))
                {
                    this.id = string.Concat(this.PartNumber, this.Manufacturer).MD5();
                }
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string PackageCase { get; set; }
        public string Packaging { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string CustomsName { get; set; }
    }
}
