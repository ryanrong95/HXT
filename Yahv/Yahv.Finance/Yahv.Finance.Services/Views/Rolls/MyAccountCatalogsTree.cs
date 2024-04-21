using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 账款类型
    /// </summary>
    public class MyAccountCatalogsTree : QueryView<nAccountCatalog, PvFinanceReponsitory>
    {
        IErpAdmin _admin;
        private IEnumerable<nAccountCatalog> data { get; set; }
        private IEnumerable<string> maps { get; set; }
        public MyAccountCatalogsTree(IErpAdmin admin)
        {
            this._admin = admin;
            this.maps = new MapsAccountCatalogOrigin().Where(item => item.AdminID == admin.ID).Select(item => item.AccountCatalogID).ToArray();

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

        public string Json(string fatherName = null)
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
            }).Single(item => item.Name == fatherName);

            if (!IsHave(model))
            {
                return string.Empty;
            }

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
                    if (IsHave(item))
                    {
                        sons.Add(this.Json(item));
                    }

                }
            }

            return new
            {
                id = entity.ID,
                text = entity.Name,
                children = sons?.ToArray()
            };
        }

        /// <summary>
        /// 是否包含 该账款分类
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns></returns>
        bool IsHave(nAccountCatalog accountCatalog)
        {
            if (maps.Contains(accountCatalog.ID))
            {
                return true;
            }

            if (accountCatalog.Sons.Count > 0)
            {
                foreach (var accountCatalogSon in accountCatalog.Sons)
                {
                    if (IsHave(accountCatalogSon))
                    {
                        return true;
                    }
                }
            }

            return false;
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