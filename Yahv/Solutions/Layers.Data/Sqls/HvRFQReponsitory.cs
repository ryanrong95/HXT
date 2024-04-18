namespace Layers.Data.Sqls
{
    /// <summary>
    /// HvRFQ 基本支持类
    /// </summary>

    public class HvRFQReponsitory : LinqReponsitory<HvRFQ.SqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public HvRFQReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public HvRFQReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }

        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public HvRFQReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }

    }
}
