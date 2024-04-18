using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Models
{
    public interface IVersion : Linq.IUnique
    {
        string Name { get; }
        string Code { get; }
        DateTime LastGenerationDate { get; }

        DateTime CreateDate { get; }
    }
}
