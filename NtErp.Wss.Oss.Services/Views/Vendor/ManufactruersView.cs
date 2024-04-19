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
    /// 制造商视图
    /// </summary>
    public class ManufactruersView : CompaniesViewBase
    {
        internal ManufactruersView() : base(CompanyType.Manufactruer)
        {

        }
        internal ManufactruersView(CvOssReponsitory reponsitory) : base(CompanyType.Manufactruer, reponsitory)
        {
        }


    }
}
