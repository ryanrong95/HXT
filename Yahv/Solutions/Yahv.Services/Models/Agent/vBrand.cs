using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services.Models
{
    public class vBrand : IUnique
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 标准品牌ID
        /// </summary>
        public string BrandID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { set; get; }
        /// <summary>
        /// 品牌负责人ID
        /// </summary>
        public string AdminID { set; get; }
        #endregion
    }
}
