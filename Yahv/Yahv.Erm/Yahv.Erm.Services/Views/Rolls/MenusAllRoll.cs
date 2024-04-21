using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Newtonsoft.Json.Linq;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 菜单视图
    /// </summary>
    public class MenusAllRoll : UniqueView<Models.Origins.Menu, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MenusAllRoll()
        {
        }

        protected override IQueryable<Models.Origins.Menu> GetIQueryable()
        {
            return new Origins.MenusOrigin().Where(item => item.Status != Status.Delete);
        }

        /// <summary>
        /// 菜单树节点
        /// </summary>
        /// <param name="id">根节点id ,为空则显示全部</param>
        /// <returns></returns>
        public JArray Tree(string id = null)
        {
            System.Linq.Expressions.Expression<Func<Models.Origins.Menu, bool>> exp = t => t.FatherID == id;
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
            System.Linq.Expressions.Expression<Func<Models.Origins.Menu, bool>> exp = t => t.FatherID == null;

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
                obj.Add(nameof(item.RightUrl), item.RightUrl);
                obj.Add(nameof(item.OrderIndex), item.OrderIndex);

                var children = this.Treegrid(item.ID, false);
                if (children != null && children.Count > 0)
                {
                    obj.Add("children", children);
                }
                arry.Add(obj);
            }

            return arry;
        }

        public string Test()
        {
            string result = string.Empty;

            var view = this.GetIQueryable();
            List<Business> list = new List<Business>();
            string rootId = string.Empty;

            if (view != null)
            {
                rootId = view.FirstOrDefault(item => item.FatherID == null).ID;

                var busMenu = view.Where(item => item.FatherID == rootId);

                foreach (var menu in busMenu)
                {
                    Business bus = new Business();
                    bus.Name = menu.Name;
                    bus.ID = menu.ID;
                    bus.IconUrl = menu.IconUrl;
                    bus.LogoUrl = menu.LogoUrl;
                    bus.FirstUrl = menu.FirstUrl;

                    int i = 0;
                    bus.Menu = new List<FirstMenu>();
                    foreach (var m in view.Where(item => item.FatherID == menu.ID))
                    {
                        var first = new FirstMenu();
                        first.text = m.Name;

                        int n = 0;
                        first.children = new List<ChildMenu>();
                        foreach (var c in view.Where(item => item.FatherID == m.ID))
                        {
                            first.children.Add(new ChildMenu()
                            {
                                text = c.Name,
                                url = c.RightUrl,
                            });

                            n++;
                        }

                        bus.Menu.Add(first);
                        i++;
                    }

                    list.Add(bus);
                }
            }

            if (list.Count > 0)
            {
                result = JsonSerializerExtend.Json(list);
            }

            return result;
        }
    }
}