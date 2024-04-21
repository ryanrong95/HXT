using Layers.Data.Sqls;
using System.Linq;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class EnterprisesRoll : Origins.EnterprisesOrigin
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EnterprisesRoll()
        {

        }
        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
