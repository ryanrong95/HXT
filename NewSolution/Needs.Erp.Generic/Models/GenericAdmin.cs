using Layer.Data.Sqls;
using Needs.Linq;

using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Erp.Generic.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <example>
    /// 这是管理员视图
    /// </example>
    public class GenericAdmin : IUnique, IGenericAdmin
    {
        public string ID { get; set; }
        public string RealName { get; set; }
        public string UserName { get; set; }
        public Status Status { get; set; }

        public bool IsSa
        {
            get
            {
                return this.UserName.Equals("sa", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal GenericAdmin()
        {

        }
    }
}
