using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Visitors;

namespace Yahv.PvData.LinqToSolr.Criterias
{
    /// <summary>
    /// 选择查询
    /// </summary>
    public class SolrSelect
    {
        public Type Type { get; set; }
        public Expression Expression { get; set; }

        static readonly Type[] PredefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Guid?),
            typeof(Math),
            typeof(Convert)
        };

        public bool IsSingleField { get; set; }

        public SolrSelect(Expression expression)
        {
            Expression = expression;
            Type = ((LambdaExpression)Expression).Body.Type;
        }

        public string GetSelectFields()
        {
            var str = "*";

            if (Expression != null)
            {
                str = MemberVisitor.GetMembers(((LambdaExpression)Expression).Body);
            }
            IsSingleField = !str.Contains(",") && PredefinedTypes.Contains(Type);
            return str;
        }
    }
}
