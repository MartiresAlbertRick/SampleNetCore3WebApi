using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AD.CAAPS.Importer.Logic
{
    public sealed class CsvMap<T> : ClassMap<T>
    {
        public CsvMap(Dictionary<string, string> map)
        {
            var newMapper = new Dictionary<string, string>(map, StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (newMapper.ContainsKey(prop.Name))
                { 
                    var parameterExpression = Expression.Parameter(typeof(T), "x");
                    var memberExpression = Expression.PropertyOrField(parameterExpression, prop.Name);
                    var memberExpressionConversion = Expression.Convert(memberExpression, memberExpression.Type);
                    //default type is string
                    dynamic lambda;
                    if (memberExpression.Type.Equals(typeof(string)))
                    {
                        lambda = Expression.Lambda<Func<T, string>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(bool?)))
                    {
                        lambda = Expression.Lambda<Func<T, bool?>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(int?)))
                    {
                        lambda = Expression.Lambda<Func<T, int?>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(int)))
                    {
                        lambda = Expression.Lambda<Func<T, int>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(double?)))
                    {
                        lambda = Expression.Lambda<Func<T, double?>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(decimal?)))
                    {
                        lambda = Expression.Lambda<Func<T, decimal?>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                    if (memberExpression.Type.Equals(typeof(DateTime?)))
                    {
                        lambda = Expression.Lambda<Func<T, DateTime?>>(memberExpressionConversion, parameterExpression);
                        Map(lambda).Name(newMapper[prop.Name]);
                        continue;
                    }
                }
            }
        }
    }
}