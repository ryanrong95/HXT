using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvCrmReponsitory : LinqReponsitory<BvCrm.SqlDataContext>
    {
        public BvCrmReponsitory()
        {
        }

        public BvCrmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
