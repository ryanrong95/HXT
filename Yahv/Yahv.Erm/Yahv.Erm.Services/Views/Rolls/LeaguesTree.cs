using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Views.Rolls
{
    public class LeaguesTree : CategoryTree<nLeague, PvbErmReponsitory>
    {
        public LeaguesTree(Category category) : base(category)
        {
        }


        protected override IQueryable<nLeague> GetIQueryable()
        {
            var leaguesView = new LeaguesOrigin(this.Reponsitory);

            return from item in leaguesView
                   where item.Category == this.Category
                   && item.Status == Status.Normal
                   orderby item.Name
                   select new nLeague
                   {
                       ID = item.ID,
                       FatherID = item.FatherID,
                       Name = item.Name,
                       Type = item.Type,
                       Status = item.Status,
                       Category = item.Category,
                   };
        }

        /// <summary>
        /// 生成根节点
        /// </summary>
        protected override void GenRoot()
        {
            string name = "全球";
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Leagues()
            {
                ID = string.Join("", Category, name).MD5(),
                FatherID = null,
                Name = name,
                Status = (int)Status.Normal,
                Type = (int)LeagueType.Area,
                Category = (int)Category,
            });
        }

        /// <summary>
        /// 获取json
        /// </summary>
        /// <returns></returns>
        public string Json()
        {
            return new[] { this.Json(Root) }.Json();
        }

        object Json(nLeague entity)
        {
            List<object> sons = null;

            if (entity.Sons.Count > 0)
            {
                sons = new List<object>();
                foreach (var item in entity.Sons)
                {
                    sons.Add(this.Json(item));
                }
            }

            return new
            {
                id = entity.ID,
                name = entity.Name,
                flag = entity.Type == LeagueType.Position,
                typeName = entity.Type.GetDescription(),
                type = entity.Type,
                children = sons?.ToArray()
            };
        }

        public object Tree()
        {
            return new[] { this.Tree(Root) };
        }

        private object Tree(nLeague entity)
        {
            List<object> sons = null;

            if (entity.Sons.Count > 0)
            {
                sons = new List<object>();
                foreach (var item in entity.Sons)
                {
                    sons.Add(this.Tree(item));
                }
            }

            return new
            {
                id = entity.ID,
                text = entity.Name,
                children = sons?.ToArray()
            };
        }
    }
}
