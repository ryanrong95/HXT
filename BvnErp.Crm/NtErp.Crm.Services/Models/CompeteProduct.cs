using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Underly;
using Needs.Utils.Converters;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.CompeteProductAlls))]
    public class CompeteProduct : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get;set;
        }

        /// <summary>
        /// 原厂地
        /// </summary>
        public string Origin
        {
            get;set;
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// 品牌
        /// </summary>
        public string ManufacturerID
        {
            get;set;
        }

        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging
        {
            get;set;
        }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase
        {
            get;set;
        }

        /// <summary>
        /// 批次
        /// </summary>
        public string Batch
        {
            get;set;
        }
        /// <summary>
        /// 封装批次
        /// </summary>
        public string DateCode
        {
            get;set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get;set;
        }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency
        {
            get;set;
        }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal ?  UnitPrice
        {
            get;set;
        }
        /// <summary>
        /// 原厂注册批复号
        /// </summary>
        public string OriginNumber
        {
            get;set;
        }
        #endregion
    }
}
