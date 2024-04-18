namespace Layers.Data.Sqls
{
    public class PvdCrmReponsitory : LinqReponsitory<PvdCrm.sqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PvdCrmReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PvdCrmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }


        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PvdCrmReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
