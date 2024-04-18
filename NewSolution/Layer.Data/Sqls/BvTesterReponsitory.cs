using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvTesterReponsitory : LinqReponsitory<BvTester.SqlDataContext>
    {
        public BvTesterReponsitory()
        {
        }

        public BvTesterReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
