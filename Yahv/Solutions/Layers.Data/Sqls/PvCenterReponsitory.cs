
namespace Layers.Data.Sqls
{
    public class PvCenterReponsitory : LinqReponsitory<Sqls.PvCenter.SqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PvCenterReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PvCenterReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }

        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PvCenterReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
