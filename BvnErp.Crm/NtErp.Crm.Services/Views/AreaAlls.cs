using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class AreaAlls : UniqueView<Area, BvCrmReponsitory>, Needs.Underly.IFkoView<Area>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public AreaAlls()
        {

        }
         
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal AreaAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取区域数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Area> GetIQueryable()
        {
            return from area in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Areas>()
                   select new Area
                   {
                       ID = area.ID,
                       FatherID = area.FatherID,
                       Name = area.Name,
                       CreateDate = area.CreateDate,
                   };
        }
    }
    /// <summary>
    /// 区域树
    /// </summary>
    public class AreaTree
    {
        /// <summary>
        /// 所有区域
        /// </summary>
        private Area[] areas;

        /// <summary>
        /// 区域树结构
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
        public AreaTree()
        {
            this.areas = new AreaAlls().ToArray();
        }

        /// <summary>
        /// 区域分类树形集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private JArray GetTree(string id = null)
        {
            Func<Area, bool> exp = t => t.FatherID == id;
            if (id == null)
            {
                exp = t => t.FatherID == null;
            }
            JArray arry = new JArray();
            var list = this.areas.Where(exp).ToArray();
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
