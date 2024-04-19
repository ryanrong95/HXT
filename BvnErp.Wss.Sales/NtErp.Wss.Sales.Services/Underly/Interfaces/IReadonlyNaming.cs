using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 只读命名接口
    /// </summary>
    public interface IReadonlyNaming
    {
        string Name { get; }
    }
}
