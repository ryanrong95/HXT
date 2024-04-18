using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvnOpsReponsitory : LinqReponsitory<BvnOps.SqlDataContext>
    {
        public BvnOpsReponsitory()
        {
        }

        public BvnOpsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
