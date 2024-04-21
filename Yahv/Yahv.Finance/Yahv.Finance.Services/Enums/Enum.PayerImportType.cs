using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 借贷标志
    /// </summary>
    public enum PayerImportType
    {
        /// <summary>
        /// 借
        /// </summary>
        [Description("借")]
        Borrow,
        /// <summary>
        /// 贷
        /// </summary>
        [Description("贷")]
        Loan
    }
}