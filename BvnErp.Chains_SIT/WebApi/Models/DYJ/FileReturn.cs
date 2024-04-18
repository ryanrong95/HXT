using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FileReturn
    {
        /// <summary>
        /// 缴款书编号
        /// </summary>
        public string CusTaxNumber { get; set; }

        /// <summary>
        /// 缴款书路径
        /// </summary>
        public string CusTaxPdfPath { get; set; }
    }
}