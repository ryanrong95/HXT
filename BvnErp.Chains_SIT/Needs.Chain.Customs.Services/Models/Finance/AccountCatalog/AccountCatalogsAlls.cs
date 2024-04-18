using Layer.Data.Sqls;
using Needs.Ccs.Services.Views.Origins;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 账款类型
    /// </summary>
    public class AccountCatalogsAlls 
    {
        private IEnumerable<nAccountCatalog> data { get; set; }
        private  AccountCatalogsAlls()
        {
            data = (from entity in new AccountCatalogsOrigin()
                    where entity.Status == GeneralStatus.Normal
                    //&& fatherIDs.Contains(entity.FatherID)
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

        public string JsonIn(string fatherName = null)
        {
            List<string> fatherIDs = new List<string>();
            fatherIDs.Add("AccCatType0016");
            fatherIDs.Add("AccCatType0019");
            fatherIDs.Add("AccCatType0023");
            fatherIDs.Add("AccCatType0025");

            var InData = data.Where(t => fatherIDs.Contains(t.FatherID));

            nAccountCatalog a1 = data.Where(t => t.ID == "AccCatType0021").FirstOrDefault();
            nAccountCatalog a2 = data.Where(t => t.ID == "AccCatType0024").FirstOrDefault();
            List<nAccountCatalog> catalogs = new List<nAccountCatalog>();
            catalogs.Add(a1);
            catalogs.Add(a2);
            InData = InData.Except(catalogs);

            var linqs = from currnet in InData
                        join _father in InData on currnet.FatherID equals _father.ID into fathers
                        from father in fathers.DefaultIfEmpty()
                        join son in InData on currnet.ID equals son.FatherID into sons
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

        public string JsonOut(string fatherName = null)
        {
            List<string> fatherIDs = new List<string>();
            fatherIDs.Add("AccCatType0016");
            fatherIDs.Add("AccCatType0020");
            fatherIDs.Add("AccCatType0027");
            fatherIDs.Add("AccCatType0029");
            fatherIDs.Add("AccCatType0087");

            var InData = data.Where(t => fatherIDs.Contains(t.FatherID));

            nAccountCatalog a1 = data.Where(t => t.ID == "AccCatType0026").FirstOrDefault();
            nAccountCatalog a2 = data.Where(t => t.ID == "AccCatType0028").FirstOrDefault();
            List<nAccountCatalog> catalogs = new List<nAccountCatalog>();
            catalogs.Add(a1);
            catalogs.Add(a2);
            InData = InData.Except(catalogs);

            var linqs = from currnet in InData
                        join _father in InData on currnet.FatherID equals _father.ID into fathers
                        from father in fathers.DefaultIfEmpty()
                        join son in InData on currnet.ID equals son.FatherID into sons
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

        //protected override IQueryable<nAccountCatalog> GetIQueryable()
        //{
        //    return data.AsQueryable();
        //}     
        public string catalogName(string catalogID)
        {
           var cata = data.Where(t => t.ID == catalogID).FirstOrDefault();
            if (cata != null)
            {
                return cata.Name;
            }
            else
            {
                return "";
            }
        }

        public string catalogIDIn(string catalogName)
        {
            List<string> fatherIDs = new List<string>();
            fatherIDs.Add("AccCatType0016");
            fatherIDs.Add("AccCatType0019");
            fatherIDs.Add("AccCatType0023");
            fatherIDs.Add("AccCatType0025");

            var InData = data.Where(t => fatherIDs.Contains(t.FatherID));

            var cata = InData.Where(t => t.Name == catalogName).FirstOrDefault();
            if (cata != null)
            {
                return cata.ID;
            }
            else
            {
                return "";
            }
        }

        public string catalogIDOut(string catalogName)
        {
            List<string> fatherIDs = new List<string>();
            fatherIDs.Add("AccCatType0016");
            fatherIDs.Add("AccCatType0020");
            fatherIDs.Add("AccCatType0027");
            fatherIDs.Add("AccCatType0029");
            fatherIDs.Add("AccCatType0087");

            var InData = data.Where(t => fatherIDs.Contains(t.FatherID));

            var cata = data.Where(t => t.Name == catalogName).FirstOrDefault();
            if (cata != null)
            {
                return cata.ID;
            }
            else
            {
                return "";
            }
        }
    }
}
