using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class Problems : IEnumerable<Problem>
    {
        IEnumerable<Problem> Items;

        public Problems(IEnumerable<Problem> items)
        {
            this.Items = items;
        }
        public Problem this[string id]
        {
            get
            {
                return this.Single(id);
            }
        }
        virtual protected Problem Single(string id)
        {
            return this.SingleOrDefault(item => item.ID == id);
        }
        public IEnumerator<Problem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
