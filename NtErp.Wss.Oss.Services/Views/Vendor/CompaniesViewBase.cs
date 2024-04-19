using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;

namespace NtErp.Wss.Oss.Services.Views
{
    abstract public class CompaniesViewBase : CompaniesAlls
    {
        public CompanyType Type { get; private set; }

        protected CompaniesViewBase(CompanyType type)
        {
            this.Type = type;
        }

        protected CompaniesViewBase(CompanyType type, CvOssReponsitory reponsitory) : base(reponsitory)
        {
            this.Type = type;
        }

        protected override IQueryable<Company> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Type == Type
                   select entity;
        }

    }
}
