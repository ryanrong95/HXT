using System.Data.Linq;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class CvOssReponsitory : LinqReponsitory<CvOss.SqlDataContext>
    {
        public CvOssReponsitory()
        {
        }

        public CvOssReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
