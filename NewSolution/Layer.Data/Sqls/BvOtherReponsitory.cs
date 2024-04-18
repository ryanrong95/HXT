using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvOtherReponsitory : LinqReponsitory<BvOthers.SqlDataContext>
    {
        public BvOtherReponsitory()
        {
        }

        public BvOtherReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
