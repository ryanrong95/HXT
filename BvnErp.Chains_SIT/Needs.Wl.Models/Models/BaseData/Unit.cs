using Needs.Linq;
using Needs.Utils.Converters;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 单位
    /// </summary>
    public class Unit : IUnique
    {
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}