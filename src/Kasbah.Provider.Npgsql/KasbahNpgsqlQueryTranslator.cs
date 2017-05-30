using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Kasbah.Content.Models;

namespace Kasbah.Provider.Npgsql
{
    public class TranslateResponse
    {
        public string WhereClause = "";
        public string OrderByClause = "";
        public long? Skip = null;
        public long? Take = null;
        public IDictionary<string, object> Parameters { get; internal set; }
    }

    public class KasbahNpgsqlQueryTranslator : ExpressionVisitor
    {
        enum Clause
        {
            Unknown,
            Where,
            Skip,
            Take,
            OrderBy
        }

        readonly StringBuilder _whereClause = new StringBuilder();
        readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();

        Clause _currentClause = Clause.Unknown;
        long? _skip, _take;

        internal KasbahNpgsqlQueryTranslator() { }

        internal TranslateResponse Translate(Expression expression)
        {
            this.Visit(expression);

            return new TranslateResponse
            {
                WhereClause = _whereClause.ToString(),
                Skip = _skip,
                Take = _take,
                Parameters = _parameters
            };
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
            if (m.Method.DeclaringType.FullName == "System.Linq.Queryable")
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        {
                            _currentClause = Clause.Where;

                            LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                            this.Visit(lambda.Body);

                            // TODO: find a way to Visit(m.Arguments[0]) without causing a StakcOverflow

                            return m;
                        }
                    case "Skip":
                        {
                            _currentClause = Clause.Skip;
                            this.Visit(m.Arguments[1]);

                            this.Visit(m.Arguments[0]);

                            return m;
                        }
                    case "Take":
                        {
                            _currentClause = Clause.Take;
                            this.Visit(m.Arguments[1]);

                            this.Visit(m.Arguments[0]);

                            return m;
                        }
                }
            }
            else if (m.Method.DeclaringType == typeof(string))
            {
                switch (m.Method.Name)
                {
                    case "Contains":
                        {
                            // TODO: generate a SQL `like` statement from this

                            return m;
                        }
                }
            }
            throw new NotSupportedException($"The method '{m.Method.Name}' on '{m.Method.DeclaringType.FullName}' is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (_currentClause)
            {
                case Clause.Where:
                    switch (u.NodeType)
                    {
                        case ExpressionType.Not:
                            _whereClause.Append(" not ");
                            this.Visit(u.Operand);
                            break;
                        default:
                            throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported");
                    }
                    break;
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (_currentClause)
            {
                case Clause.Where:
                    _whereClause.Append("(");
                    this.Visit(b.Left);
                    switch (b.NodeType)
                    {
                        case ExpressionType.And:
                        case ExpressionType.AndAlso:
                            _whereClause.Append(" and ");
                            break;
                        case ExpressionType.Or:
                        case ExpressionType.OrElse:
                            _whereClause.Append(" or ");
                            break;
                        case ExpressionType.Equal:
                            _whereClause.Append(" = ");
                            break;
                        case ExpressionType.NotEqual:
                            _whereClause.Append(" <> ");
                            break;
                        case ExpressionType.LessThan:
                            _whereClause.Append(" < ");
                            break;
                        case ExpressionType.LessThanOrEqual:
                            _whereClause.Append(" <= ");
                            break;
                        case ExpressionType.GreaterThan:
                            _whereClause.Append(" > ");
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            _whereClause.Append(" >= ");
                            break;
                        default:
                            throw new NotSupportedException($"The binary operator '{b.NodeType}' is not supported");
                    }
                    this.Visit(b.Right);
                    _whereClause.Append(")");
                    break;
            }
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            switch (_currentClause)
            {
                case Clause.Where:
                    if (c.Value == null)
                    {
                        _whereClause.Append("null");
                    }
                    else
                    {
                        switch (Type.GetTypeCode(c.Value.GetType()))
                        {
                            case TypeCode.Object:
                                if (typeof(IItem).IsAssignableFrom(c.Value.GetType()))
                                {
                                    var parameterName = $"p{_parameters.Count + 1}";
                                    _parameters.Add(parameterName, (c.Value as IItem).Id);
                                    _whereClause.Append($"@{parameterName}");
                                    break;
                                }
                                else
                                {
                                    throw new NotSupportedException($"The constant for '{c.Value}' is not supported");
                                }
                            case TypeCode.Boolean:
                            case TypeCode.String:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                            case TypeCode.DateTime:
                            default:
                                {
                                    var parameterName = $"p{_parameters.Count + 1}";
                                    _parameters.Add(parameterName, c.Value);
                                    _whereClause.Append($"@{parameterName}");
                                    break;
                                }
                        }
                    }
                    break;
                case Clause.Skip:
                    switch (Type.GetTypeCode(c.Value.GetType()))
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            _skip = Convert.ToInt64(c.Value);
                            break;
                    }
                    break;
                case Clause.Take:
                    switch (Type.GetTypeCode(c.Value.GetType()))
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            _take = Convert.ToInt64(c.Value);
                            break;
                    }
                    break;
            }
            return c;
        }


        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _whereClause.Append($"nc.content->>'{m.Member.Name}'");
                return m;
            }
            throw new NotSupportedException($"The member '{m.Member.Name}' is not supported");
        }
    }
}
