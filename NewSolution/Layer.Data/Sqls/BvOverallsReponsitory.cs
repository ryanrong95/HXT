using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvOverallsReponsitory : LinqReponsitory<BvOveralls.SqlDataContext>
    {
        public BvOverallsReponsitory()
        {
        }

        public BvOverallsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
