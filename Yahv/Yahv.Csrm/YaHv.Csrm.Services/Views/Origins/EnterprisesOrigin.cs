using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class EnterprisesOrigin : Yahv.Linq.UniqueView<Models.Origins.Enterprise, PvbCrmReponsitory>
    {
        public EnterprisesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal EnterprisesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
            Anonymous();
        }
        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                   select new Enterprise
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       AdminCode = entity.AdminCode,
                       District = entity.District,
                       Status = (ApprovalStatus)entity.Status,
                       Corporation = entity.Corporation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Place = entity.Place
                   };
        }
        /// <summary>
        /// 匿名企业
        /// </summary>
        void Anonymous()
        {
            var enterprise = new Enterprise {ID= "A235E8E7773EDB26697E7E771915197D", Name = "匿名" };
            if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == enterprise.ID))
            {
                Reponsitory.Insert(enterprise.ToLinq());
            }
        }
    }
}
