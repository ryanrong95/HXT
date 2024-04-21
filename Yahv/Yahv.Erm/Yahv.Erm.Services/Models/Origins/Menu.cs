using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : IUnique
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父亲为空时表示系统级；深度为1时表示业务级；深度为2时表示菜单头；深度为3时表示菜单项
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单引用的地址
        /// </summary>
        public string RightUrl { get; set; }

        /// <summary>
        /// 标签的地址、css样式
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 首页地址，一般只在业务级别使用
        /// </summary>
        public string FirstUrl { get; set; }

        /// <summary>
        /// Logo地址，一般只在业务级别使用
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 排序使用，直接在数据库中进行修改即可
        /// </summary>
        public int? OrderIndex { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// HelpUrl地址，一般只在业务级别使用
        /// </summary>
        public string HelpUrl { get; set; }

        #endregion




    }
}