using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Visitors
{
    internal class MemberVisitor : ExpressionVisitor
    {
        internal IList<string> Members;
        internal MemberVisitor()
        {
            Members = new List<string>();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Members.Add(GetName(node.Member));
            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);

            return base.VisitBinary(node);
        }

        internal static string GetName(MemberInfo member)
        {
            var prop = member;
            var dataMemberAttribute = prop.GetCustomAttribute<JsonPropertyAttribute>();

            return $"{(dataMemberAttribute?.PropertyName ?? prop.Name)}";
        }

        internal static string GetMembers(Expression expression)
        {
            var gm = new MemberVisitor();
            gm.Visit(expression);

            return string.Join(",", gm.Members.ToArray());
        }
    }
}
