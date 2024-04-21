using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class EnterprisesOrigin : Yahv.Linq.UniqueView<Models.Enterprise, PvcCrmReponsitory>
    {
        public EnterprisesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal EnterprisesOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Enterprise> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.Enterprises>()
                   select new Models.Enterprise
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       AdminCode = entity.AdminCode,
                       District = entity.District,
                       Status = (ApprovalStatus)entity.Status,
                       Corporation = entity.Corporation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc
                   };
        }
    }
}
