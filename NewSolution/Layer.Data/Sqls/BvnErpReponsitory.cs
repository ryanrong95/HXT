using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvnErpReponsitory : LinqReponsitory<BvnErp.SqlDataContext>
    {
        public BvnErpReponsitory()
        {
        }

        public BvnErpReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
