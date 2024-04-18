using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class ApiSupplier
    {
        public EnterpriseObj Enterprise { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public   string EnglishName { get; set; }
       /// <summary>
       /// 中文名
       /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string  CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string  UpdateDate { get; set; }
        /// <summary>
        /// 状态( 200正常400 已删除)
        /// </summary>

        public int  Status { get; set; }
        /// <summary>
        /// 备注(不必填)
        /// </summary>

        public string Summary { get; set; }



    }
}