using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;

namespace Yahv.PvWsOrder.Services.Views.Rolls.Document
{
    public class vCatalogTree
    {
        /// <summary>
        /// 分类
        /// </summary>
        private vCatalog[] industries;

        /// <summary>
        /// 行业树结构
        /// </summary>
        public JArray tree
        {
            get
            {
                return this.GetTree();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public vCatalogTree()
        {
            this.industries = new vCatalogsOrigin().ToArray();
        }

        /// <summary>
        /// 分类树形集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private JArray GetTree(string id = null)
        {
            Func<vCatalog, bool> exp = t => t.FatherID == id;
            if (id == null)
            {
                exp = t => t.FatherID == null;
            }
            JArray arry = new JArray();
            var list = this.industries.Where(exp).ToArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj.Add("id", item.ID);
                obj.Add("text", item.Name);

                var children = GetTree(item.ID);
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