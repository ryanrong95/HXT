using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvOrdersReponsitory : LinqReponsitory<BvOrders.SqlDataContext>
    {
        public BvOrdersReponsitory()
        {
        }

        public BvOrdersReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
