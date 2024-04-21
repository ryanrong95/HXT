using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class CompaniesOrigin : Yahv.Linq.UniqueView<Models.Origins.Company, PvbCrmReponsitory>
    {
        internal CompaniesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CompaniesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Company> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Companies>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new Company
                   {
                       ID = entity.ID,
                       Enterprise = enterprises,
                       Range = (AreaType)entity.Range,
                       Type = (CompanyType)entity.Type,
                       CompanyStatus = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CreatorID = admin.ID,
                       Admin = admin
                   };
        }
    }
}
