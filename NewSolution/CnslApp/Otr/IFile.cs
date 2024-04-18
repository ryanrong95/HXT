using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr
{
    public interface IFile
    {
        string Name { get; set; }

        string Category { get; set; }

        string Url { get; set; }
    }
}
