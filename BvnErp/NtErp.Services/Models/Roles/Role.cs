using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 权限
    /// </summary>
    public sealed class Role : Needs.Linq.IUnique, Needs.Linq.IPersistence, Needs.Linq.IFulError, Needs.Linq.IFulSuccess
    {
        public Role()
        {
            this.Status = RoleStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public RoleStatus Status { get; set; }
        /// <summary>
        /// 摘要说明
        /// </summary>
        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        #endregion

        #region 权限

        /// <summary>
        /// 菜单
        /// </summary>
        public Views.MenusForRoleView Menus
        {
            get
            {
                return new Views.MenusForRoleView(this);
            }
        }

        /// <summary>
        /// 颗粒化
        /// </summary>
        public Views.UnitesForRoleView Unites
        {
            get
            {
                return new Views.UnitesForRoleView(this);
            }
        }

        #endregion

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvnErpReponsitory())
            {

                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Role);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
            if (this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            if (this.ID != Needs.Settings.SettingsManager<Needs.Settings.IAdminSettings>.Current.SuperRoleID)
            {
                using (var reponsitory = new Layer.Data.Sqls.BvnErpReponsitory())
                {
                    reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsAdminRole>(item => item.RoleID == this.ID);
                    reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsRoleMenu>(item => item.RoleID == this.ID);
                    reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsRoleUnite>(item => item.RoleID == this.ID);
                    reponsitory.Delete<Layer.Data.Sqls.BvnErp.Roles>(item => item.ID == this.ID);
                }

                if (this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
            else
            {
                if (this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs(this.ID));
                }
                throw new Exception("不可删除");
            }
        }
        #endregion 

    }
}
