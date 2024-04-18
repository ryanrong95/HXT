using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvEesReponsitory : LinqReponsitory<BvnEes.SqlDataContext>
    {
        public BvEesReponsitory()
        {
        }

        public BvEesReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
