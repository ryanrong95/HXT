using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 公司视图
    /// </summary>
    public class CompaniesView : UniqueView<Models.Company, CvOssReponsitory>
    {
        internal CompaniesView()
        {

        }
        internal CompaniesView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Company> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Companies>()
                       select new Models.Company
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           Address =entity.Address,
                           Code = entity.Code,
                           Type = (CompanyType)entity.Type,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate
                       };

            return linq;
        }

    }
}
