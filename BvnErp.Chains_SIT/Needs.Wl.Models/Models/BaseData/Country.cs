using Needs.Linq;
using Needs.Utils.Converters;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 国家与地区
    /// </summary>
    public class Country : IUnique
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

        public string Code { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }
    }
}