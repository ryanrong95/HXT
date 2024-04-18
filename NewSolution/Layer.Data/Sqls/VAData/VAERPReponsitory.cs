using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class VAERPReponsitory : LinqReponsitory<VAData.SqlDataContext>
    {
        public VAERPReponsitory()
        {
        }

        public VAERPReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
