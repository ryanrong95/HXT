using NtErp.Wss.Oss.Services.Models;
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
    /// 商家总视图
    /// </summary>
    public class CompaniesAlls : UniqueView<Company, CvOssReponsitory>
    {
        internal CompaniesAlls()
        {

        }
        protected CompaniesAlls(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<Company> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Companies>()
                       select new Company
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           Type = (CompanyType)entity.Type
                       };

            return linq;
        }

    }
}
