using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr
{
    /// <summary>
    /// 支持的表达式
    /// </summary>
    public class ExpressionMethods
    {
        public const string Where = nameof(Where);
        public const string First = nameof(First);
        public const string FirstOrDefault = nameof(FirstOrDefault);
        public const string Take = nameof(Take);
        public const string Skip = nameof(Skip);
        public const string OrderBy = nameof(OrderBy);
        public const string ThenBy = nameof(ThenBy);
        public const string OrderByDescending = nameof(OrderByDescending);
        public const string ThenByDescending = nameof(ThenByDescending);
        public const string Select = nameof(Select);
        public const string Contains = nameof(Contains);
        public const string StartsWith = nameof(StartsWith);
        public const string EndsWith = nameof(EndsWith);
        public const string GroupBy = nameof(GroupBy);
    }


}
