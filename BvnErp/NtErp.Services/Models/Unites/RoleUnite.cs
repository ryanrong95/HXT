using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{

    /// <summary>
    /// 颗粒化对象
    /// </summary>
    public sealed class RoleUnite : IRoleUnite
    {

        public RoleUnite()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性

        string id;
        /// <summary>
        /// ID 唯一标识
        /// </summary>
        public string ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.id))
                {
                    this.id = string.Concat("RU", string.Concat(this.Type, this.Menu, this.Name).MD5());
                }
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 配置类型
        /// </summary>
        public RoleUniteType Type { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Menu { get; set; }

        /// <summary>
        /// 标签Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// /// <summary>
        /// 标签Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 当前页面url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

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
                if (reponsitory.GetTable<Layer.Data.Sqls.BvnErp.RoleUnites>().Count(item => item.Type == (int)this.Type && item.Menu == this.Menu && item.Name == this.Name) == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.RoleUnites);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
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
            using (var reponsitory = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsRoleUnite>(item => item.RoleUniteID == this.ID);
                reponsitory.Delete<Layer.Data.Sqls.BvnErp.RoleUnites>(item => item.ID == this.ID);
            }

            if (this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }


        #endregion

        #region override Equals
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            var entity = obj as RoleUnite;
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Concat(this.Type, this.Menu, this.Name, this.Title, this.Url).GetHashCode();
        }
        #endregion
    }


}
