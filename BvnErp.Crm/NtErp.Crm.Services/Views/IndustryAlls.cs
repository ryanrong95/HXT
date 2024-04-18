using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class IndustryAlls : UniqueView<Industry, BvCrmReponsitory>, Needs.Underly.IFkoView<Industry>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public IndustryAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal IndustryAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取行业分类数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Industry> GetIQueryable()
        {
            return from entity in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Industries>()
                   select new Industry
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       FatherID = entity.FatherID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       EnglishName=entity.EnglishName
                   };
        }
    }

    /// <summary>
    /// 行业树
    /// </summary>
    public class IndustryTree
    {
        /// <summary>
        /// 所有行业
        /// </summary>
        private Industry[] industries;

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
        public IndustryTree()
        {
            this.industries = new IndustryAlls().ToArray();
        }

        /// <summary>
        /// 行业分类树形集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private JArray GetTree(string id = null)
        {
            Func<Industry, bool> exp = t => t.FatherID == id;
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
