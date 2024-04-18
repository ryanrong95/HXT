using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Visitors
{
    internal class BooleanVisitor : ExpressionVisitor
    {
        int _bypass;

        internal static Expression Process(Expression expression)
        {
            return new BooleanVisitor().Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (_bypass == 0 && node.Type == typeof(bool))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                        _bypass++;
                        var result = base.VisitBinary(node);
                        _bypass--;
                        return result;
                }
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (_bypass == 0 && node.Type == typeof(bool))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Not:
                        _bypass++;
                        var result = Expression.NotEqual(
                            Visit(node.Operand),
                            Expression.Constant(true));
                        _bypass--;
                        return result;
                }
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (_bypass == 0 && node.Type == typeof(bool))
            {
                return Expression.Equal(base.VisitMember(node), Expression.Constant(true));
            }
            return base.VisitMember(node);
        }
    }
}
