using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Overall;

namespace NtErp.Wss.Sales.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// Admin登录token
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey("Login", PKeySigner.Mode.Time, 10)]
        LoginToken = 10000,

        /// <summary>
        /// 管理员
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Admin), PKeySigner.Mode.Normal, 10)]
        Admin = 40000,

        /// <summary>
        /// 角色
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Role), PKeySigner.Mode.Normal, 10)]
        Role = 21000,

        /// <summary>
        /// 菜单
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Menu), PKeySigner.Mode.Normal, 10)]
        Menu = 22000,

        /// <summary>
        /// 菜单
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey("RU", PKeySigner.Mode.Normal, 10)]
        RoleUnites = 23000,
        /// <summary>
        /// 用户收入
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("UI", PKeySigner.Mode.Normal, 10)]
        UserInput = 24000,
        /// <summary>
        /// 用户支出
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("UO", PKeySigner.Mode.Normal, 10)]
        UserOutput = 24001,
        /// <summary>
        /// 用户还款
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("UA", PKeySigner.Mode.Normal, 10)]
        UserAccount = 24002,

        /// <summary>
        /// 购物车
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("SN", PKeySigner.Mode.Time, 10)]
        Cart = 25000,
        /// <summary>
        /// 商品帐input
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("CDYI", PKeySigner.Mode.Time, 10)]
        CommodityInput = 26001,
        /// <summary>
        /// 商品帐output
        /// </summary>
        [Repository(typeof(BvOrdersReponsitory))]
        [PKey("CDYO", PKeySigner.Mode.Time, 10)]
        CommodityOutput = 26002
    }
}
