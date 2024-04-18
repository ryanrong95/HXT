using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class PvbCrmReponsitory : LinqReponsitory<PvbCrm.sqlDataContext>
    {
        public PvbCrmReponsitory()
        {
        }

        public PvbCrmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
