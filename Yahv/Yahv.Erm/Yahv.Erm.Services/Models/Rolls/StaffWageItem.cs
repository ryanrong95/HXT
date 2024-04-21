using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 员工工资项
    /// </summary>
    public class StaffWageItem : IUnique
    {
        /// <summary>
        /// StaffID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 工资项ID
        /// </summary>
        public string WageItemID { get; set; }

        /// <summary>
        /// 工资项默认值
        /// </summary>
        public decimal? DefaultValue { get; set; }

        /// <summary>
        /// 工资项名称
        /// </summary>
        public string WageItemName { get; set; }

        /// <summary>
        /// 普通列（Normal）、计算列（Calc）、数据列（Data）
        /// </summary>
        public WageItemType Type { get; set; }

        /// <summary>
        /// 是否可更改
        /// </summary>
        public bool IsImport { get; set; }

        /// <summary>
        /// 公式
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 计算顺序
        /// </summary>
        public int? CalcOrder { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
    }
}