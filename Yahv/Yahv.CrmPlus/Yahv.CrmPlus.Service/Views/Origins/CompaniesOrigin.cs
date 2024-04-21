using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class CompaniesOrigin : Yahv.Linq.UniqueView<Models.Origins.Company, PvdCrmReponsitory>
    {
        internal CompaniesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CompaniesOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Company> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            //var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Companies>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   //join register in registerView on entity.ID equals register.ID
                   join admin in adminsView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Company
                   {
                       ID = entity.ID,
                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Status = enterprise.Status,
                       District = enterprise.District,
                       Place = enterprise.Place,
                       Grade = enterprise.Grade,
                       Summary = enterprise.Summary,
                       CreateDate = entity.CreateDate,
                       EnterpriseRegister = enterprise.EnterpriseRegister,
                       CreatorID = admin.ID,
                       Creator = admin,
                       CompanyCreateDate = entity.CreateDate,
                       CompanyStatus = (DataStatus)entity.Status
                   };
        }
    }


}

