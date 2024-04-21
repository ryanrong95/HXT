using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    public class Catalogs : IEnumerable<Catalog>
    {
        IEnumerable<Subject> collection;
        public Catalogs(IEnumerable<Subject> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// 可用的
        /// </summary>
        public decimal Available { get; internal set; }

        /// <summary>
        /// 分类索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Catalog this[string catalog]
        {
            get
            {
                return this.Single(item => item.Name == catalog);
            }
        }

        public IEnumerator<Catalog> GetEnumerator()
        {
            var linq = this.collection.GroupBy(item => item.Catalog).Select(item => new Catalog
            {
                Name = item.Key,
                Subjects = item
            });

            return linq.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
