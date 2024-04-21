using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls;

namespace Yahv.CrmPlus.Service
{
    static public class Files
    {
        /// <summary>
        /// 所有的企业文件
        /// </summary>
        /// <returns></returns>
        static public IQueryable<Models.Origins.FilesDescription> SupplierFiles(string enterpriseid)
        {
            return new FilesDescriptionRoll(enterpriseid).Where(item => item.Type != Underly.CrmFileType.VisitingCard &&item.Type!= Underly.CrmFileType.PricingRules && item.Status == Underly.DataStatus.Normal);
        }
    }
}
