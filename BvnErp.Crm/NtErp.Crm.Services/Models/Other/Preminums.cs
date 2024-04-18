using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class Preminums : IEnumerable<Preminum>
    {
        IEnumerable<Preminum> Items;

        public Preminums(IEnumerable<Preminum> items)
        {
            this.Items = items;
        }

        public Preminum this[string id]
        {
            get
            {
                return this.Single(id);
            }
        }

        virtual protected Preminum Single(string id)
        {
            return this.SingleOrDefault(item => item.ID == id);
        }


        public IEnumerator<Preminum> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
