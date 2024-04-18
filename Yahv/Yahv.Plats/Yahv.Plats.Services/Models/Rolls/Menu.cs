using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Plats.Services.Models.Rolls
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : IUnique
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Menu()
        {

        }

        #region 属性

        /// <summary>
        /// ID 唯一标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderIndex { get; set; }

        public Menu[] Sons { get; set; }

        #endregion


        public Staff Staff { get; set; }


        public void Enter()
        {
            using (var r = new Layers.Data.Sqls.PvbErmReponsitory())
            {
                this.Staff.Enter();
            }
        }


    }
}
