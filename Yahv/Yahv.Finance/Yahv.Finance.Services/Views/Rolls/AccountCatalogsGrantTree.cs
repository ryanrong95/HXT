using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class AccountCatalogsGrantTree : QueryView<nAccountCatalog, PvFinanceReponsitory>
    {
        private IEnumerable<nAccountCatalog> data { get; set; }
        private IEnumerable<string> maps { get; set; }
        public AccountCatalogsGrantTree(string adminID)
        {
            this.maps = new MapsAccountCatalogOrigin().Where(item => item.AdminID == adminID).Select(item => item.AccountCatalogID).ToArray();

            data = (from entity in new AccountCatalogsOrigin()
                    where entity.Status == GeneralStatus.Normal
                    orderby entity.ID
                    select new nAccountCatalog()
                    {
                        ID = entity.ID,
                        Status = entity.Status,
                        Name = entity.Name,
                        FatherID = entity.FatherID,
                    }).ToArray();
        }

        public string Json()
        {
            var linqs = from currnet in data
                        join _father in data on currnet.FatherID equals _father.ID into fathers
                        from father in fathers.DefaultIfEmpty()
                        join son in data on currnet.ID equals son.FatherID into sons
                        select new
                        {
                            currnet,
                            father,
                            sons
                        };
            var model = linqs.Select(item =>
            {
                item.currnet.Father = item.father;
                item.currnet.Sons = new SubTree<nAccountCatalog>(item.sons.ToArray());

                return item.currnet;
            }).Single(item => item.Father == null);
            return new[] { this.Json(model) }.Json();
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
                text = entity.Name,
                children = sons?.ToArray(),
                @checked = maps.Contains(entity.ID),
            };
        }

        protected override IQueryable<nAccountCatalog> GetIQueryable()
        {
            data = (from entity in new AccountCatalogsOrigin()
                    where entity.Status == GeneralStatus.Normal
                    orderby entity.ID
                    select new nAccountCatalog()
                    {
                        ID = entity.ID,
                        Status = entity.Status,
                        Name = entity.Name,
                        FatherID = entity.FatherID,
                    }).ToArray();

            return data.AsQueryable();
        }
    }
}