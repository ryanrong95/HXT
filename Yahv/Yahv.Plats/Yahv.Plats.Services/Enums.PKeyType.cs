using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.Plats.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Repository(typeof(PvbErmReponsitory))]
        [PKey("Menu", PKeySigner.Mode.Normal, 5)]
        Menu = 10000,
    }
}