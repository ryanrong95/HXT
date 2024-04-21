using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 分类
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public IEnumerable<Subject> Subjects { get; set; }
    }
}
