using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{


    /// <summary>
    /// 指定持久化
    /// </summary>
    public interface _IPersistence<T> where T : IUnique
    {
        /// <summary>
        /// 持久化
        /// </summary>
        T Enter();
    }

    /// <summary>
    /// 指定持久化
    /// </summary>
    public interface IPersistence<T> : _IPersistence<T>, IPersistence where T : IUnique
    {
    }
}
