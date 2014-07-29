using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactGraph.Internals
{
    internal class ExpressionParser
    {
        public NodeInfo[] GetSourceVerticies<TProp>(Expression<Func<TProp>> formula)
        {
            var body = (MethodCallExpression)formula.Body;
            return body.Arguments.SelectMany(a =>
            {
                var propertyExpression = a as MemberExpression;
                if (propertyExpression != null)
                {
                    var expression = propertyExpression.Expression as MemberExpression;
                    var constantExpression = (ConstantExpression)expression.Expression;
                    var parentInstance = constantExpression.Value;
                    var instance = ((FieldInfo)expression.Member).GetValue(parentInstance);
                    return new[] { new NodeInfo(instance, propertyExpression.Member as PropertyInfo, null) };
                }

                throw new NotSupportedException("Cannot deal with expression");
            }).ToArray();
        }

        public NodeInfo GetNodeInfo<TProp>(Expression<Func<TProp>> target, Expression<Func<TProp>> formula)
        {
            var propertyExpression = target.Body as MemberExpression;
            if (propertyExpression != null)
            {
                var expression = propertyExpression.Expression as MemberExpression;
                var constantExpression = (ConstantExpression)expression.Expression;
                var parentInstance = constantExpression.Value;
                var instance = ((FieldInfo)expression.Member).GetValue(parentInstance);
                var propertyInfo = propertyExpression.Member as PropertyInfo;
                var getVal = formula.Compile();
                return new NodeInfo(instance, propertyInfo, () =>
                {
                    propertyInfo.SetValue(instance, getVal(), null);
                });
            }

            throw new NotSupportedException("Cannot deal with expression");
        }
    }
}