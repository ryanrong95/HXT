using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 工资项累计值
    /// </summary>
    public class PastsItem : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 工资日期
        /// </summary>
        public int DateIndex { get; set; }

        /// <summary>
        /// 所属地区
        /// </summary>
        public string WorkCityID { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 工资项类型
        /// </summary>
        public WageItemType Type { get; set; }

        /// <summary>
        /// 累计值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 累计次数（根据个税累计次数走）
        /// </summary>
        public int Accumulative { get; set; }
        #endregion
    }
}
