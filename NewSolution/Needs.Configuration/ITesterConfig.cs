using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Configuration
{
    /// <summary>
    /// 测试使用
    /// </summary>
    public interface ITesterConfig : IConfiguration
    {
        /// <summary>
        /// 上确界
        /// </summary>
        int Supremum { get; }
    }
}
