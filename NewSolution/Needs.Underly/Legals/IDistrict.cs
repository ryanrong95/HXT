using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Legals
{
    public interface IDistrict : ILegal
    {
        string ShortName { get; }
        string Name { get; }
        string Domain { get; }
        string ShowName { get; }
    }

}
