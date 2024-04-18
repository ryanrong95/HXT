using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientFiles : BaseItems<ClientFile>
    {
        internal ClientFiles(IEnumerable<ClientFile> enums) : base(enums)
        {
        }

        internal ClientFiles(IEnumerable<ClientFile> enums, Action<ClientFile> action) : base(enums, action)
        {
        }

        public override void Add(ClientFile item)
        {
            base.Add(item);
        }

        protected override IEnumerable<ClientFile> GetEnumerable(IEnumerable<ClientFile> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
