using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 账款分类 树
    /// </summary>
    public class AccountCatalogsTree : Tree<nAccountCatalog, PvFinanceReponsitory>
    {
        protected override IQueryable<nAccountCatalog> GetIQueryable()
        {
            var view = new AccountCatalogsOrigin();

            return from entity in view
                   where entity.Status == GeneralStatus.Normal
                   orderby entity.ID
                   select new nAccountCatalog()
                   {
                       ID = entity.ID,
                       Status = entity.Status,
                       Name = entity.Name,
                       FatherID = entity.FatherID,
                   };
        }

        protected override void GenRoot()
        {
            string name = "全部";
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvFinance.AccountCatalogs()
            {
                ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.AccCatType),
                FatherID = null,
                Name = name,
                CreatorID = Npc.Robot.Obtain(),
                ModifierID = Npc.Robot.Obtain(),
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = (int)GeneralStatus.Normal,
            });
        }

        public string Json()
        {
            return new[] { this.Json(Root) }.Json();
        }

        object Json(nAccountCatalog entity)
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
                children = sons?.ToArray()
            };
        }
    }
}