using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    public class SupplierSpecialRecord : BaseApplyRecord
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string EnterpriseName { internal set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public nBrandType SpecialType { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { set; get; }
        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
    }
}
