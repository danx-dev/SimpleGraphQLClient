using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleGraphQLClient
{
    public static class Extensions
    {
        public static string OutputParameters(this Type instance)
        {
            var simpleTypes = new Type[] { typeof(string), typeof(int), typeof(DateTime), typeof(decimal), typeof(Guid), typeof(bool) };
            var props = instance.GetProperties();
            List<string> propNames = new List<string>();
            var simpleProps = props.Where(x => simpleTypes.Contains(x.PropertyType));
            var complexProps = props.Where(x => !simpleTypes.Contains(x.PropertyType)).ToList();
            propNames.AddRange(simpleProps.Select(x => x.Name));
            foreach (var prop in complexProps)
            {
                string complexOutput = "";
                if (prop.PropertyType.IsGenericType)
                {
                    var genericType = prop.PropertyType.GenericTypeArguments.First();
                    complexOutput = genericType.OutputParameters();
                }
                else
                {
                    complexOutput = prop.PropertyType.OutputParameters();
                }

                string propName = prop.Name + complexOutput;
                propNames.Add(propName);
            }

            var result = "{" + string.Join(",", propNames) + "}";

            return result;
        }
    }
}