using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class DeclareProducts : IEnumerable<DeclareProduct>
    {

        IEnumerable<DeclareProduct> Declares;

        DeclareProducts()
        {

        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeclareProduct this[string id]
        {
            get
            {
                return Single(id);
            }
        }

        virtual protected DeclareProduct Single(string id)
        {
            return this.SingleOrDefault(item => item.ID == id);
        }

        public DeclareProducts(IEnumerable<DeclareProduct> declares)
        {
            this.Declares = declares;
        }

        public IEnumerator<DeclareProduct> GetEnumerator()
        {
            return Declares.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
