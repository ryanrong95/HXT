using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Projects
{
    /// <summary>
    /// 销售机会产品型号
    /// </summary>
    public class ProjectProductItem
    {
        public ProjectProductItem()
        {

        }

        /// <summary>
        /// 销售机会
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public ProductItem ProductItem { get; set; }
    }


}
