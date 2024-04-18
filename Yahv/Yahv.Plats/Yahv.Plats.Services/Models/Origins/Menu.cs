using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Plats.Services.Models.Origins
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

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Status Status { get; set; }

        #endregion

        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加 
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    var list = repository.GetTable<Menus>().Where(item => item.FatherID == this.FatherID);

                    //不存在Name,认为是新增.存在认为更改
                    var menu = list.SingleOrDefault(item => item.Name == this.Name);
                    if (menu != null && menu.RightUrl != this.RightUrl)
                    {
                        repository.Update<Menus>(new
                        {
                            RightUrl = this.RightUrl
                        }, item => item.ID == menu.ID);

                        this.ID = menu.ID;
                    }
                    else
                    {
                        var id = PKeySigner.Pick(PKeyType.Menu);

                        repository.Insert(new Menus()
                        {
                            Status = (int)(Status.Normal),
                            ID = id,
                            Name = this.Name,
                            FatherID = this.FatherID,
                            FirstUrl = this.FirstUrl,
                            IconUrl = this.IconUrl,
                            LogoUrl = this.LogoUrl,
                            OrderIndex = this.OrderIndex,
                            RightUrl = this.RightUrl,
                        });

                        this.ID = id;
                    }
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Status = AdminStatus.Closed
                }, item => item.ID == this.ID);
            }
        }
    }
}