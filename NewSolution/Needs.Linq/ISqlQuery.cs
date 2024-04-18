 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    /// <summary>
    /// Sql 可查询接口
    /// </summary>
    /// <typeparam name="T">普通接口对象</typeparam>
    public interface ISqlQuery<T> : IQueryable<T>
    {

    }
}
