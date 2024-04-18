namespace Layers.Data.Sqls
{
    /// <summary>
    /// Overalls 基本支持类
    /// </summary>

    public class OverallsReponsitory : LinqReponsitory<Overalls.SqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public OverallsReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public OverallsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }

        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public OverallsReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
