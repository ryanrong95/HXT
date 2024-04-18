namespace Yahv.Linq.Persistence
{
    /// <summary>
    /// 集合持久化（调用接口时一定要使用事件）
    /// 
    /// 持久化接口不做要求
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    public interface ISetPersistence<T> where T : class, IUnique
    {
        /// <summary>
        /// 保存指定类型的对象
        /// </summary>
        /// <param name="entity">指定类型的对象</param>
        void Enter(T entity);

        /// <summary>
        /// 移除指定ID的数据
        /// </summary>
        /// <param name="id">ID</param>
        void Remove(string id);
    }
}
