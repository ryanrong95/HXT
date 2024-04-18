using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 管理员库房
    /// </summary>
    public class AdminWarehouse
    {
        /// <summary>
        /// 管理员Id
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WareHouseID { get; set; }

        /// <summary>
        /// 库房名称
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
    }
}
