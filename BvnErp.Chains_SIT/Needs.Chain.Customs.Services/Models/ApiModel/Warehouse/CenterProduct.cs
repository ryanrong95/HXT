using Needs.Linq;
using Needs.Utils.Converters;
using System;

namespace Needs.Ccs.Services.Models
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
                    this.id = string.Concat(this.PartNumber, this.Manufacturer, this.PackageCase, this.Packaging).MD5();
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
    }
}
