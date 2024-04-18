namespace Yahv.Linq
{
    /// <summary>
    /// Sql唯一可查询接口
    /// </summary>
    /// <typeparam name="T">实现唯一性接口的对象</typeparam>
    public interface ILinqUnique<T> : ISqlQuery<T> where T : IUnique
    {
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id">指定ID</param>
        /// <returns>指定ID的实现唯一性接口的对象</returns>
        T this[string id]
        {
            get;
        }
    }

}
