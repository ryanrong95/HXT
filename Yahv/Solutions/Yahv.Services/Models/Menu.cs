using Yahv.Linq;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : IUnique
    {
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
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

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
        /// HelpUrl地址，一般只在业务级别使用
        /// </summary>
        public string HelpUrl { get; set; }


        /// <summary>
        /// 排序使用，直接在数据库中进行修改即可
        /// </summary>
        public int? OrderIndex { get; set; }

        ///// <summary>
        ///// 0表示系统级；1时表示业务级；2时表示菜单头；3时表示菜单项
        ///// </summary>
        //public int Levels { get; set; }

    }
}
