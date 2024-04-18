using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class CvSsoReponsitory : LinqReponsitory<CvSso.SqlDataContext>
    {
        public CvSsoReponsitory()
        {
        }

        public CvSsoReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
