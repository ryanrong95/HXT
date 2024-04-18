using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Newtonsoft.Json.Linq;
using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 菜单视图
    /// </summary>
    public class MenusAlls : UniqueView<Menu, BvnErpReponsitory>
    {
        public MenusAlls()
        {

        }

        protected override IQueryable<Menu> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Menus>()
                        select new Menu
                        {
                            ID = entity.ID,
                            FatherID = entity.FatherID,
                            Icon = entity.Icon,
                            Name = entity.Name,
                            Url = entity.Url,
                            Status = (MenuStatus)entity.Status,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                            Summary = entity.Summary,
                            OrderIndex = entity.OrderIndex
                        };

        }

        /// <summary>
        /// 菜单树节点
        /// </summary>
        /// <param name="id">根节点id ,为空则显示全部</param>
        /// <returns></returns>
        public JArray Tree(string id = null)
        {
            System.Linq.Expressions.Expression<Func<Menu, bool>> exp = t => t.FatherID == id;
            if (id == null)
            {
                exp = t => t.FatherID == null;
            }
            JArray arry = new JArray();
            var list = this.GetIQueryable().Where(exp).ToArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj.Add("id", item.ID);
                obj.Add("text", item.Name);

                var children = Tree(item.ID);
                if (children != null && children.Count > 0)
                {
                    obj.Add("children", children);
                }
                arry.Add(obj);
            }

            return arry;
        }

        /// <summary>
        /// 菜单treegrid数据
        /// </summary>
        /// <param name="id">根节点id ,为空则显示全部</param>
        /// <returns></returns>
        public JArray Treegrid(string id, bool isRoot = true)
        {
            System.Linq.Expressions.Expression<Func<Menu, bool>> exp = t => t.FatherID == null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                exp = t => t.FatherID == id;
                if (isRoot)
                {
                    exp = t => t.ID == id;
                }
            }

            JArray arry = new JArray();
            var list = this.GetIQueryable().Where(exp).OrderBy(t => t.OrderIndex).ToArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj.Add("id", item.ID);
                obj.Add("name", item.Name);
                obj.Add(nameof(item.Url), item.Url);
                obj.Add(nameof(item.OrderIndex), item.OrderIndex);
                obj.Add(nameof(item.CreateDate), item.CreateDate);

                var children = this.Treegrid(item.ID, false);
                if (children != null && children.Count > 0)
                {
                    obj.Add("children", children);
                }
                arry.Add(obj);
            }

            return arry;
        }

    }
}
