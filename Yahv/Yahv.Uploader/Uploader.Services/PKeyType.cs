using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Uploader.Services
{
    /// <summary>
    /// 主键ID
    /// </summary>
    /// <remarks>备用</remarks>
    public enum PKeyType
    {

        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("F", PKeySigner.Mode.Date, 4)]
        FileDecription = 10000,
    }
}
