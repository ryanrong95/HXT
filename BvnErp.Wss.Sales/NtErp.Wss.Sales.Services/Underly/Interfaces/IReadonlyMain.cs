using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 只读主对象接口
    /// </summary>
    public interface IReadonlyMain : IUnique
    {
        string Summary { get; }
        DateTime CreateDate { get; }
        DateTime UpdateDate { get; }
        SelfStatus Status { get; }
    }
}
