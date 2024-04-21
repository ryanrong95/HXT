using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Models.Rolls;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class CooperatersRoll : Yahv.Linq.QueryView<Cooperater, PvbCrmReponsitory>
    {
        Enterprise enterprise;
        public CooperatersRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Cooperater> GetIQueryable()
        {
            var companiesCiew = new Origins.CompaniesOrigin(this.Reponsitory).Where(item => item.CompanyStatus != ApprovalStatus.Deleted);
            //var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsEnterprise>()
                   join companies in companiesCiew on maps.RealID equals companies.ID
                   //join enterprise in enterpriseView on maps.EnterpriseID equals enterprise.ID
                   where maps.EnterpriseID == this.enterprise.ID
                   select new Cooperater
                   {
                       Enterprise = this.enterprise,
                       CooperType = (CooperType)maps.CooperType,
                       Company = companies
                   };
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="CooperType"></param>
        /// <returns></returns>
        public IQueryable<Cooperater> this[CooperType CooperType]
        {
            get
            {
                var arry = CooperType.GetHasFlag(CooperType.None);
                return this.Where(item => arry.Contains(item.CooperType));
            }
        }

    }
}
