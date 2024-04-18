using Needs.Overall.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Models
{
    public interface IDevlopNote : Linq.IUnique
    {
        Devloper Devloper { get; }
        string TypeName { get; }
        string MethodName { get; }
        string Context { get; }
        int Number { get; }
        DateTime CreateDate { get; }
        DateTime UpdateDate { get; }

        CsProject CsProject { get; }

        /// <summary>
        /// 更新比较
        /// </summary>
        /// <param name="txt"></param>
        void Note(string txt);
    }
}
