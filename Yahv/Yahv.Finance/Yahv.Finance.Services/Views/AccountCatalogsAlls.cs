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
    /// 账款类型
    /// </summary>
    public class AccountCatalogsAlls : QueryView<nAccountCatalog, PvFinanceReponsitory>
    {
        private IEnumerable<nAccountCatalog> data { get; set; }
        AccountCatalogsAlls()
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
        }

        static object locker = new object();
        static private AccountCatalogsAlls _current;
        static public AccountCatalogsAlls Current
        {
            get
            {
                if (_current == null)
                {
                    lock (locker)
                    {
                        if (_current == null)
                        {
                            _current = new AccountCatalogsAlls();
                        }
                    }
                }
                return _current;
            }
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

            return new[] { this.Json(model) }.Json();
        }

        public object ToObject(string fatherName = null)
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

            return this.Json(model);
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
                children = sons?.ToArray()
            };
        }

        protected override IQueryable<nAccountCatalog> GetIQueryable()
        {
            return data.AsQueryable();
        }
    }

    #region _bak
    //public class AccountCatalogsComboTree : Tree<nAccountCatalog, PvFinanceReponsitory>
    //{
    //    protected override IQueryable<nAccountCatalog> GetIQueryable()
    //    {
    //        var view = new AccountCatalogsOrigin();

    //        return from entity in view
    //               where entity.Status == GeneralStatus.Normal
    //               orderby entity.ID
    //               select new nAccountCatalog()
    //               {
    //                   ID = entity.ID,
    //                   Status = entity.Status,
    //                   Name = entity.Name,
    //                   FatherID = entity.FatherID,
    //               };
    //    }

    //    public string Json(string fatherName = null)
    //    {
    //        var arry = this.ToArray();
    //        var linqs = from currnet in arry
    //                    join _father in arry on currnet.FatherID equals _father.ID into fathers
    //                    from father in fathers.DefaultIfEmpty()
    //                    join son in arry on currnet.ID equals son.FatherID into sons
    //                    select new
    //                    {
    //                        currnet,
    //                        father,
    //                        sons
    //                    };

    //        var model = linqs.Select(item =>
    //        {
    //            item.currnet.Father = item.father;
    //            item.currnet.Sons = new SubTree<nAccountCatalog>(item.sons.ToArray());

    //            return item.currnet;
    //        }).Single(item => item.Name == fatherName);

    //        return new[] { this.Json(model) }.Json();
    //    }

    //    object Json(nAccountCatalog entity)
    //    {
    //        List<object> sons = null;

    //        if (entity.Sons.Count > 0)
    //        {
    //            sons = new List<object>();
    //            foreach (var item in entity.Sons)
    //            {
    //                sons.Add(this.Json(item));
    //            }
    //        }

    //        return new
    //        {
    //            id = entity.ID,
    //            text = entity.Name,
    //            children = sons?.ToArray()
    //        };
    //    }
    //}
    #endregion
}