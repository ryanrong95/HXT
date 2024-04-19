using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Wss.Oss.Services.Models;
using Layer.Data.Sqls;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 供应商视图
    /// </summary>
    public class SuppliersView : CompaniesViewBase
    {
        internal SuppliersView() : base(CompanyType.Supplier)
        {

        }

        internal SuppliersView(CvOssReponsitory reponsitory) : base(CompanyType.Supplier, reponsitory)
        {
        }
    }
}
