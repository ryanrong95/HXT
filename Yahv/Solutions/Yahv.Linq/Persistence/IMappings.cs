namespace Yahv.Linq.Persistence
{
    /// <summary>
    /// 关系性
    /// </summary>
    public interface IMappings
    {
        /// <summary>
        /// 增加绑定关系
        /// </summary>
        /// <param name="arry"></param>
        void Binding(params string[] arry);

        /// <summary>
        /// 解绑定
        /// </summary>
        /// <param name="arry">参数数组</param>
        void Unbind(params string[] arry);
    }
}
