using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    ///企业信息
    /// </summary>
    public class EnterpriseObj
    {
      
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 管理编码(保障局部唯一)
        /// </summary>
        public string AdminCode { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { get; set; }

        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }

     

        /// <summary>
        /// 
        /// </summary>
        public int ServiceType { set; get; }

        public int? StorageType { set; get; }


    }
}