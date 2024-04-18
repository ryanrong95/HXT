using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class BvnVrsReponsitory : LinqReponsitory<BvnVrs.SqlDataContext>
    {
        public BvnVrsReponsitory()
        {
        }

        public BvnVrsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
