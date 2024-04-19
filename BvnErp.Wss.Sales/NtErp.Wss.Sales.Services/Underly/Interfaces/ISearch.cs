using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 搜索
    /// </summary>
    public interface ISearch<T> where T : class
    {
        T Search(string name, string value);
    }
}
