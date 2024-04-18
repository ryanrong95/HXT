using System.Linq;

namespace Yahv.Linq
{
    /// <summary>
    /// Sql 可查询接口
    /// </summary>
    /// <typeparam name="T">普通接口对象</typeparam>
    public interface ISqlQuery<T> : IQueryable<T>
    {

    }
}
