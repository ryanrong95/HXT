using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvSsoReponsitory : LinqReponsitory<BvSso.SqlDataContext>
    {
        public BvSsoReponsitory()
        {
        }

        public BvSsoReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
