//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Underly;

//namespace Yahv.Payments
//{
//    public class AccountCatalogs : IEnumerable<AccountCatalog>
//    {
//        IEnumerable<AccountSubject> collection;
//        public AccountCatalogs(IEnumerable<AccountSubject> collection)
//        {
//            this.collection = collection;
//        }

//        SortedList<Currency, AccountCatalogsSubtotal> subtatols;
//        /// <summary>
//        /// 构造器
//        /// </summary>
//        internal AccountCatalogs()
//        {
//            this.subtatols = new SortedList<Currency, AccountCatalogsSubtotal>();
//            foreach (var currency in Enum.GetValues(typeof(Currency)).Cast<Currency>())
//            {
//                this.subtatols.Add(currency, new AccountCatalogsSubtotal());
//            }
//        }

//        /// <summary>
//        /// 获取指定分类
//        /// </summary>
//        /// <param name="catalog">分类</param>
//        /// <returns></returns>
//        public AccountCatalog this[string catalog]
//        {
//            get { return this.Single(item => item.Name == catalog); }
//        }

//        /// <summary>
//        /// 分类索引
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public AccountSubject this[string catalog, string name]
//        {
//            get
//            {
//                if (string.IsNullOrWhiteSpace(name))
//                {
//                    return this.Single(item => item.Name == catalog).Subjects[catalog];
//                }
//                return this.Single(item => item.Name == catalog).Subjects[name];
//            }
//        }

//        public IEnumerator<AccountCatalog> GetEnumerator()
//        {
//            var linq = this.collection.GroupBy(item => item.Catalog).Select(item => new AccountCatalog
//            {
//                Name = item.Key,
//                Subjects = new AccountSubjects(item)
//            });

//            return linq.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
