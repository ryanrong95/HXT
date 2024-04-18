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
    public class Menu : IUnique //  : IMenu
    {
        public Menu()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = MenuStatus.Normal;
            this.Summary = "";
        }

        #region 属性

        /// <summary>
        /// ID 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父级
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
        /// 说明
        /// </summary>
        public string Summary { get; set; }

        public MenuStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int OrderIndex { get; set; }
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
                    var list = reponsitory.GetTable<Layer.Data.Sqls.BvnErp.Menus>().Where(item => item.FatherID == this.FatherID).ToArray();
                    // 不存在Name和Url,认为是新增.存在认为更改
                    if (list.Count(item => item.Name == this.Name || item.Url == this.Url) > 0)
                    {
                        var menu1 = list.SingleOrDefault(item => item.Name == this.Name);
                        var menu2 = list.SingleOrDefault(item => item.Url == this.Url);
                        if (menu1 == null && menu2 != null)
                        {
                            this.CreateDate = menu2.CreateDate;

                            reponsitory.Update<Layer.Data.Sqls.BvnErp.Menus>(new
                            {
                                Name = this.Name
                            }, item => item.ID == menu2.ID);
                        }
                        else if (menu1 != null && menu2 == null)
                        {
                            this.CreateDate = menu1.CreateDate;

                            reponsitory.Update<Layer.Data.Sqls.BvnErp.Menus>(new
                            {
                                Url = this.Url
                            }, item => item.ID == menu1.ID);
                        }
                    }
                    else
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Menu);
                        reponsitory.Insert(this.ToLinq());
                    }
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvnErp.Menus>(this.ToLinq(), item => item.ID == this.ID);
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
                reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsRoleMenu>(item => item.MenuID == this.ID);
                reponsitory.Delete<Layer.Data.Sqls.BvnErp.Menus>(item => item.ID == this.ID);
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
            var entity = obj as Menu;
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Concat(this.FatherID, this.Name, this.Url).GetHashCode();
        }
        #endregion
    }
}
