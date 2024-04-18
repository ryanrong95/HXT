using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Model
{
    /// <summary>
    /// 模拟只读接口
    /// </summary>
    public interface IMapTester
    {
        string MainID { get; }
        string SubID { get; }
    }

    /// <summary>
    /// 一般用不上
    /// </summary>
    [Obsolete("理论上永不上，仅仅作为示例使用")]
    public class MapTester : IMapTester
    {
        public string MainID { get; set; }
        public string SubID { get; set; }
    }
}
