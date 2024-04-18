using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Http
{
    public interface ICookieHit
    {
        string this[string index] { get; set; }
    }
}
