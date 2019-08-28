using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleGraphQLClient.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Takes an instance of an object and generates a graphql query string.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string OutputParameters(this Type instance)
        {
            List<Type> simpleTypes = new List<Type> { typeof(DateTimeOffset), typeof(string), typeof(int), typeof(DateTime), typeof(decimal), typeof(Guid), typeof(bool) };
            var props = instance.GetProperties();
            var simpleTypesList = simpleTypes.ToList();
            List<string> propNames = new List<string>();
            List<PropertyInfo> simpleProps = new List<PropertyInfo>();
            List<PropertyInfo> complexProps = new List<PropertyInfo>();
            SortProperties(props, simpleTypesList, simpleProps, complexProps);
            propNames.AddRange(simpleProps.Select(x => x.Name));
            GoThroughComplexProperties(propNames, complexProps);
            return $" {{ {string.Join(" ", propNames)} }}";
        }

        public static string ToCamelCase(this string theString)
        {
            if (theString == null || theString.Length < 2)
                return theString;

            string[] words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            string result = "";
            foreach (var word in words)
            {
                if (IsAllUpper(word))
                {
                    result += word + " ";
                    continue;
                }
                var camelCasedWord = word.Substring(0, 1).ToLower();
                result += camelCasedWord + word.Substring(1) + " ";
            }

            return result.Trim();
        }

        private static bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        private static void GoThroughComplexProperties(List<string> propNames, List<PropertyInfo> complexProps)
        {
            foreach (var prop in complexProps)
            {
                string complexOutput = "";

                if (prop.PropertyType.IsGenericType)
                {
                    var genericType = prop.PropertyType.GetGenericArguments().First();
                    complexOutput = genericType.OutputParameters();
                }
                else
                {
                    if (prop.PropertyType.GetProperties().Length == 0)
                    {
                        continue;
                    }
                    complexOutput = prop.PropertyType.OutputParameters();
                }

                string propName = prop.Name + complexOutput;
                propNames.Add(propName);
            }
        }

        private static void SortProperties(PropertyInfo[] props, List<Type> simpleTypesList, List<PropertyInfo> simpleProps, List<PropertyInfo> complexProps)
        {
            foreach (var prop in props)
            {
                if (simpleTypesList.Contains(prop.PropertyType) || prop.PropertyType.IsPrimitive || typeof(Enum).IsAssignableFrom(prop.PropertyType))
                {
                    simpleProps.Add(prop);
                }
                else
                {
                    complexProps.Add(prop);
                }
            }
        }
    }
}