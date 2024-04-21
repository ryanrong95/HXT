using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    //合同
    public class ContractsRoll : Origins.ContractsOrigin
    {
        Enterprise enterprise;
        string companyid;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ContractsRoll(Enterprise Enterprise, string companyid)
        {
            this.enterprise = Enterprise;
            this.companyid = companyid;
        }
        protected override IQueryable<Models.Origins.Contract> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       where entity.Enterprise.ID == this.enterprise.ID && entity.CompanyID == this.companyid
                       select entity;
            return linq;
        }
    }
}

