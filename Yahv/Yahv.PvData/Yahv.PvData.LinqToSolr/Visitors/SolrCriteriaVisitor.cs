using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Criterias;

namespace Yahv.PvData.LinqToSolr.Visitors
{
    internal class SolrCriteriaVisitor : ExpressionVisitor
    {
        private StringBuilder sb;
        private bool _inRangeQuery;
        private bool _inRangeEqualQuery;
        private ISolrService _service;
        private bool isRedudant;
        private bool isNotEqual;
        private List<string> sortings;
        private Type elementType;
        private bool IsMultiList;

        internal SolrCriteriaVisitor(ISolrService query)
        {
            _service = query;
            elementType = GetElementType(_service.ElementType);
            sortings = new List<string>();
        }

        internal SolrCriteriaVisitor(ISolrService query, Type elementType)
        {
            _service = query;
            this.elementType = GetElementType(elementType);
            sortings = new List<string>();
            this.elementType = elementType;
        }

        internal string GetFieldName(MemberInfo member)
        {
            var dataMemberAttribute = member.GetCustomAttribute<JsonPropertyAttribute>();
            var fieldName = dataMemberAttribute?.PropertyName ?? member.Name;

            //TODO:SolrFacet实现后在此处添加判断if(_service.Criteria.FacetsToIgnore.Any()) { ... }

            return fieldName;
        }

        private Type GetElementType(Type type)
        {
            if (type.Name == "IGrouping`2")
            {
                return type.GetGenericArguments()[1];
            }
            return type;
        }

        internal string Translate(Expression expression)
        {
            isRedudant = true;
            sb = new StringBuilder();
            Visit(expression);
            return sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == ExpressionMethods.Where || m.Method.Name == ExpressionMethods.First || m.Method.Name == ExpressionMethods.FirstOrDefault)
            {
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                var solrQueryTranslator = new SolrCriteriaVisitor(_service);
                var fq = solrQueryTranslator.Translate(lambda.Body);
                sb.AppendFormat("&fq={0}", fq);

                var arr = StripQuotes(m.Arguments[0]);
                Visit(arr);
                return m;
            }
            if (m.Method.Name == ExpressionMethods.Take)
            {
                _service.Criteria.Take = (int)((ConstantExpression)m.Arguments[1]).Value;
                Visit(m.Arguments[0]);
                return m;
            }
            if (m.Method.Name == ExpressionMethods.Skip)
            {
                _service.Criteria.Skip = (int)((ConstantExpression)m.Arguments[1]).Value;
                Visit(m.Arguments[0]);
                return m;
            }

            if (m.Method.Name == ExpressionMethods.OrderBy || m.Method.Name == ExpressionMethods.ThenBy)
            {
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                _service.Criteria.AddSorting(lambda.Body, SolrSortType.Asc);

                Visit(m.Arguments[0]);

                return m;
            }

            if (m.Method.Name == ExpressionMethods.OrderByDescending || m.Method.Name == ExpressionMethods.ThenByDescending)
            {
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                _service.Criteria.AddSorting(lambda.Body, SolrSortType.Desc);

                Visit(m.Arguments[0]);

                return m;
            }

            if (m.Method.Name == ExpressionMethods.Select)
            {
                _service.Criteria.Select = new SolrSelect(StripQuotes(m.Arguments[1]));
                Visit(m.Arguments[0]);

                return m;
            }

            if (m.Method.Name == ExpressionMethods.Contains)
            {
                if (m.Method.DeclaringType == typeof(string))
                {
                    var str = string.Format("*{0}*", ((ConstantExpression)StripQuotes(m.Arguments[0])).Value);

                    Visit(Expression.Equal(m.Object, Expression.Constant(str)));

                    return m;
                }
                else
                {
                    var arr = (ConstantExpression)StripQuotes(m.Arguments[0]);
                    Expression lambda;

                    if (m.Arguments.Count == 2)
                    {
                        lambda = StripQuotes(m.Arguments[1]);
                        Visit(lambda);
                        Visit(arr);
                    }
                    else
                    {
                        var newExpr = Expression.Equal(m.Object, m.Arguments[0]);
                        var expr = new SolrCriteriaVisitor(_service, m.Arguments[0].Type);
                        expr.IsMultiList = true;
                        var multilistfq = expr.Translate(newExpr);
                        sb.AppendFormat("{0}", multilistfq);
                    }

                    return m;
                }
            }

            if (m.Method.Name == ExpressionMethods.StartsWith)
            {
                if (m.Method.DeclaringType == typeof(string))
                {
                    var str = string.Format("{0}*", ((ConstantExpression)StripQuotes(m.Arguments[0])).Value);
                    Visit(BinaryExpression.Equal(m.Object, ConstantExpression.Constant(str)));

                    return m;
                }
            }
            if (m.Method.Name == ExpressionMethods.EndsWith)
            {
                if (m.Method.DeclaringType == typeof(string))
                {
                    var str = string.Format("*{0}", ((ConstantExpression)StripQuotes(m.Arguments[0])).Value);
                    Visit(BinaryExpression.Equal(m.Object, ConstantExpression.Constant(str)));

                    return m;
                }
            }

            if (m.Method.Name == ExpressionMethods.GroupBy)
            {
                var arr = StripQuotes(m.Arguments[1]);
                var solrQueryTranslator = new SolrCriteriaVisitor(_service, ((MemberExpression)((LambdaExpression)arr).Body).Member.ReflectedType);

                _service.Criteria.GroupFields.Add(solrQueryTranslator.Translate(arr));
                Visit(m.Arguments[0]);

                return m;
            }

            throw new NotSupportedException(string.Format("不支持方法调用 '{0}'", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append("-");
                    Visit(u.Operand);
                    break;

                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("不支持一元运算符 '{0}'", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            isRedudant = false;
            sb.Append("(");

            if (b.Left is ConstantExpression)
            {
                throw new System.Data.InvalidExpressionException("无法解析表达式, 请确保Solr字段始终位于比较运算符的左侧.");
            }
            if (b.NodeType == ExpressionType.NotEqual)
            {
                isNotEqual = true;
            }
            Visit(b.Left);

            if (isNotEqual && b.Left.NodeType == ExpressionType.Call)
            {
                isNotEqual = false;
                sb.Append(")");
                return b;
            }

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    sb.Append(":");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(":");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(":[");
                    _inRangeQuery = true;
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(":[*");
                    _inRangeQuery = true;
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(":{");
                    _inRangeEqualQuery = true;
                    break;
                case ExpressionType.LessThan:
                    sb.Append(":{*");
                    _inRangeEqualQuery = true;
                    break;

                default:
                    throw new NotSupportedException(string.Format("不支持二元运算符 '{0}'", b.NodeType));
            }

            Visit(b.Right);
            sb.Append(")");

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;
            if (q == null)
            {
                if (_inRangeQuery || _inRangeEqualQuery)
                {
                    if (sb[sb.Length - 1] == '*')
                    {
                        sb.Append(" TO  ");
                        AppendConstValue(c.Value);
                    }
                    else
                    {
                        AppendConstValue(c.Value);
                        sb.Append(" TO *");
                    }
                    sb.Append(_inRangeEqualQuery ? "}" : "]");
                    _inRangeQuery = false;
                }
                else
                {
                    AppendConstValue(c.Value);
                }
            }

            return c;
        }

        private void AppendConstValue(object val)
        {
            var isArray = val.GetType().GetInterface("IEnumerable`1") != null;

            //设置Solr日期格式 2020-03-11T14:27:20.000Z
            if (val.GetType() == typeof(DateTime))
            {
                sb.Append(((DateTime)val).ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
            }
            else if (!(val is string) && isArray)
            {
                var array = (IEnumerable)val;
                var arrstring = string.Join(" OR ", array.Cast<object>().Select(x => string.Format("\"{0}\"", x)).ToArray());
                sb.AppendFormat(": ({0})", arrstring);
            }
            else
            {
                if (val is string)
                {
                    if (IsMultiList)
                    {
                        sb.Append(string.Format("({0})", val));
                    }
                    else
                    {
                        //TODO: 特殊字符待处理
                        //sb.Append(val.ToString().Replace(" ", "\\ "));
                        //sb.Append(val.ToString().Replace("&", "\\&").Replace("+", "\\+"));
                        sb.Append(val.ToString());
                    }
                }
                else
                {
                    sb.Append(val);
                }
            }
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {

                var fieldName = GetFieldName(m.Member);
                sb.Append(isNotEqual ? string.Format("-{0}", fieldName) : fieldName);
                return m;
            }
            if (m.Expression != null)
            {
                if (m.Expression.NodeType == ExpressionType.Constant)
                {
                    var ce = (ConstantExpression)m.Expression;
                    sb.Append(ce.Value);
                }
                else if (m.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    //TODO:SolrJoin实现后在此处添加相关业务处理
                }

                return m;
            }

            throw new NotSupportedException(string.Format("未支持的成员 '{0}'", m.Member.Name));
        }
    }
}
