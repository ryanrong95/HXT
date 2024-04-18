using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class ProductItemExtend : ProductItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ProjectID
        {
            get; set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public ProjectType ProjectType
        {
            get; set;
        }

        /// <summary>
        /// 客户对象
        /// </summary>
        public string ProjectClientName
        {
            get;set;
        }

        /// <summary>
        /// 公司对象
        /// </summary>
        public string ProjectCompanyName
        {
            get; set;
        }

        public DateTime ProjectUpdateDate
        {
            get;set;
        }

        /// <summary>
        /// 币值
        /// </summary>
        public CurrencyType ProjectCurrency
        {
            get;set;
        }

        /// <summary>
        /// 人员对象
        /// </summary>
        public string ProjectAdminName
        {
            get; set;
        }

        /// <summary>
        /// 人员ID
        /// </summary>
        public string ProjectAdminID
        {
            get; set;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ProjectClientID
        {
            get;set;
        }

        public string ProjectCompanyID
        {
            get;set;
        }
    }


    /// <summary>
    ///销售机会导入
    /// </summary>
    public class ProjectExcelData
    {
        /// <summary>
        /// Excel表格数据
        /// </summary>
        public ExcelProject ExcelProject { get; set; }

        /// <summary>
        /// 项目数据
        /// </summary>
        public NtErp.Crm.Services.Models.Project Project { get; set; }

        /// <summary>
        /// 项目特征值
        /// </summary>
        public string ProjectMD5 { get; set; }

        /// <summary>
        /// 产品项数据
        /// </summary>
        public NtErp.Crm.Services.Models.ProductItem ProductItem { get; set; }
    }
}
