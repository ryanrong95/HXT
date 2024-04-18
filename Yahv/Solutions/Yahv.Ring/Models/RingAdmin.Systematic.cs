namespace Yahv.Models
{
    /// <summary>
    /// Erp 管理员的系统领域
    /// </summary>
    partial class RingAdmin
    {
        /// <summary>
        /// Erm 领域
        /// </summary>
        public Systematic.WarehousingService WhService
        {
            get
            {
                return new Systematic.WarehousingService(this);
            }
        }
    }
}
