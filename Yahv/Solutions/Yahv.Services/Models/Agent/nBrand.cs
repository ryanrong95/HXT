using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class nBrand : IUnique
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 品牌ID
        /// </summary>
        public string BrandID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { internal set; get; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { set; get; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { internal set; get; }
        #endregion
    }
}
