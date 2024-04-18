using Layers.Data.Sqls;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// 系统平台
    /// </summary>
    public class Plat : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Plat(IErpAdmin admin)
        {
            this.admin = admin;
        }


        #region Views

        /// <summary>
        /// 我的菜单
        /// </summary>
        public Plats.Services.Views.MyMenus MyMenus
        {
            get
            {
                return new Plats.Services.Views.MyMenus(this.admin);
            }
        }

        /// <summary>
        /// 我的业务菜单
        /// </summary>
        public Yahv.Erm.Services.MyRingRole MyBusinessMenus
        {
            get
            {
                return new Yahv.Erm.Services.MyRingRole(this.admin);
            }
        }
        #endregion

        #region IAction

        public void Logs_Error(Logs_Error log)
        {
            using (var repository = new PvbErmReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.PvbErm.Logs_Errors>(new Layers.Data.Sqls.PvbErm.Logs_Errors
                {
                    AdminID = admin.ID,
                    Page = log.Page,
                    Message = log.Message,
                    Codes = log.Codes,
                    Source = log.Source,
                    Stack = log.Stack,
                    CreateDate = log.CreateDate
                });
            }
        }

        #endregion

    }
}
