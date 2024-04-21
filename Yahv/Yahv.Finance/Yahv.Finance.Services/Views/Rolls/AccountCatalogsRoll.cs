using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 收付款类型
    /// </summary>
    public class AccountCatalogsRoll : UniqueView<AccountCatalog, PvFinanceReponsitory>
    {
        public AccountCatalogsRoll() { }


        public AccountCatalogsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AccountCatalog> GetIQueryable()
        {
            return new AccountCatalogsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public void Add(IEnumerable<AccountCatalog> list)
        {
            if (list.Any())
            {
                var entities = list.Select(item => new Layers.Data.Sqls.PvFinance.AccountCatalogs()
                {
                    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.AccCatType),
                    Name = item.Name,
                    FatherID = item.FatherID,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreatorID = item.CreatorID,
                    ModifierID = item.ModifierID,
                    Status = (int)GeneralStatus.Normal,
                });
                this.Reponsitory.Insert(entities);
            }
        }

        /// <summary>
        /// 获取tree下的项
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public IEnumerable<AccountCatalog> Get(string first, string second)
        {
            var list = new List<AccountCatalog>();

            var data = (from entity in this
                        where entity.Status == GeneralStatus.Normal
                        orderby entity.ID
                        select new nAccountCatalog()
                        {
                            ID = entity.ID,
                            Status = entity.Status,
                            Name = entity.Name,
                            FatherID = entity.FatherID,
                        }).ToArray();

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
            }).Single(item => item.Name == first);

            list = model?.Sons?.FirstOrDefault(item => item.Name == second)?
                .Sons?.Select(item => new AccountCatalog()
                {
                    ID = item.ID,
                    FatherID = item.FatherID,
                    Name = item.Name,
                }).ToList();

            return list;
        }

        /// <summary>
        /// 获取tree下的项
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public IEnumerable<AccountCatalog> Get(params string[] names)
        {
            var list = new List<AccountCatalog>();

            var data = (from entity in this
                        where entity.Status == GeneralStatus.Normal
                        orderby entity.ID
                        select new nAccountCatalog()
                        {
                            ID = entity.ID,
                            Status = entity.Status,
                            Name = entity.Name,
                            FatherID = entity.FatherID,
                        }).ToArray();

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
            }).Single(item => item.Name == names[0]);

            for (int i = 1; i < names.Length; i++)
            {
                model = model?.Sons?.FirstOrDefault(item => item.Name == names[i]);
            }

            list = model?.Sons.Select(item => new AccountCatalog()
            {
                ID = item.ID,
                FatherID = item.FatherID,
                Name = item.Name,
            }).ToList();

            return list;
        }

        /// <summary>
        /// 获取账款分类ID
        /// </summary>
        /// <param name="names">分类名称数组（"付款类型","综合业务","费用","银行手续费"）</param>
        /// <returns></returns>
        public string GetID(params string[] names)
        {
            var array = new string[names.Length - 1];
            for (int i = 0; i < names.Length - 1; i++)
            {
                array[i] = names[i];
            }

            return this.Get(array).FirstOrDefault(item => item.Name == names.LastOrDefault())?.ID;
        }

        public void Map(string adminID, params string[] arry)
        {
            using (PvFinanceReponsitory repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvFinance.MapsAccountCatalog>(item => item.AdminID == adminID);
                var list = new List<Layers.Data.Sqls.PvFinance.MapsAccountCatalog>();
                foreach (var item in arry.Distinct())
                {
                    list.Add(new Layers.Data.Sqls.PvFinance.MapsAccountCatalog()
                    {
                        AdminID = adminID,
                        AccountCatalogID = item
                    });
                }
                if (list.Count > 0)
                    repository.Insert<Layers.Data.Sqls.PvFinance.MapsAccountCatalog>(list);
            }
        }
    }
}