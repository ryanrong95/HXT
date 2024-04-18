using System;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 运单费用
    /// </summary>
    public class WayCost :  IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 运单编码
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 科目（只能由我们公司规定并执行的科目）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 运单的价值
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        #endregion

        public WayCost()
        {
            this.CreateDate = DateTime.Now;
        }
    }
}
