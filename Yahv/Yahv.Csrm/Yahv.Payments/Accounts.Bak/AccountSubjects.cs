//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yahv.Payments
//{
//    public class AccountSubjects : IEnumerable<AccountSubject>
//    {
//        IEnumerable<AccountSubject> collection;
//        public AccountSubjects(IEnumerable<AccountSubject> collection)
//        {
//            this.collection = collection;
//        }

//        /// <summary>
//        /// 可用的
//        /// </summary>
//        public decimal Available { get; internal set; }

//        /// <summary>
//        /// 分类索引
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public AccountSubject this[string index]
//        {
//            get { return this.Single(item => item.Name == index); }
//        }

//        public IEnumerator<AccountSubject> GetEnumerator()
//        {
//            return this.collection.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
