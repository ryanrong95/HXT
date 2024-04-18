
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    /// <summary>
    /// Sql唯一可查询接口
    /// </summary>
    /// <typeparam name="T">有唯一标识接口对象</typeparam>
    public interface ILinqUnique<T> : ISqlQuery<T> where T : IUnique
    {
        T this[string id]
        {
            get;
        }
    }

}
