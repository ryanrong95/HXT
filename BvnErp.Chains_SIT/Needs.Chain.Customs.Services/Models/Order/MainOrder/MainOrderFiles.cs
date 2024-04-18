using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class MainOrderFiles : BaseItems<MainOrderFile>
    {
        internal MainOrderFiles(IEnumerable<MainOrderFile> enums) : base(enums)
        {
        }

        internal MainOrderFiles(IEnumerable<MainOrderFile> enums, Action<MainOrderFile> action) : base(enums, action)
        {
        }

        public override void Add(MainOrderFile item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
