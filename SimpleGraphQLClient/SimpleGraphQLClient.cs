using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleGraphQLClient.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SimpleGraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public class SimpleGraphQLClient : ISimpleGraphQLClient, IDisposable
    {
        private readonly StringBuilder _graphQlStringBuilder = new StringBuilder();
        private HttpClient _httpClient;

        public SimpleGraphQLClient()
        {
        }

        public SimpleGraphQLClient(HttpClient httpClient, string url = null)
        {
            _httpClient = httpClient;
            ServiceUrl = url;
        }

        public SimpleGraphQLClient(string url)
        {
            ServiceUrl = url;
        }

        public string OperationName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string ServiceUrl { get; set; }

        public void Dispose()
        {
            OperationName = null;
            Parameters = null;
            _httpClient = null;
        }

        public T Post<T>()
        {
            if (string.IsNullOrWhiteSpace(ServiceUrl))
            {
                throw new ApplicationException("ServiceUrl is required!");
            }

            if (_httpClient == null)
                _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(ServiceUrl);

            Builder<T>();
            var query = _graphQlStringBuilder.ToString();
            var t = new { query };
            var json = JsonConvert.SerializeObject(t);
            var response = _httpClient.PostAsync("",
                new StringContent(json, Encoding.UTF8, "application/json")).Result;

            var content = response.Content.ReadAsStringAsync().Result;

            var jObject = JObject.Parse(content);
            if (jObject["data"]?.HasValues == true)
            {
                var data = jObject["data"].Children().First().First();
                var string1 = data.ToString();
                return JsonConvert.DeserializeObject<T>(data.ToString());
            }

            throw new ApplicationException(content);
        }

        private void AddQueryParameter()
        {
            string pad = new string(' ', 0);
            _graphQlStringBuilder.Append(pad).Append(OperationName);
            if (Parameters?.Count > 0)
            {
                _graphQlStringBuilder.Append("(");
                List<string> parameterStringList = new List<string>();
                foreach (var p in Parameters)
                {
                    if (p.Value is string)
                    {
                        parameterStringList.Add($@"{p.Key}:  ""{p.Value}""");
                    }
                    else if (IsList(p.Value))
                    {
                        var list = (List<string>)p.Value;
                        if (list[0] is string)
                        {
                            var pString = string.Join("\",\"", list);
                            parameterStringList.Add($"{p.Key}:[\"{pString}\"]");
                        }
                    }
                    else
                    {
                        parameterStringList.Add($"{p.Key}: {p.Value}");
                    }
                }
                _graphQlStringBuilder.Append(string.Join(",", parameterStringList));
                _graphQlStringBuilder.Append(")");
            }
        }

        private void Builder<T>()
        {
            _graphQlStringBuilder.Append("query {");
            AddQueryParameter();
            _graphQlStringBuilder.Append(typeof(T).OutputParameters().ToCamelCase());
            _graphQlStringBuilder.Append("}");
        }

        private bool IsList(object o)
        {
            if (o == null) return false;
            Type[] types = { typeof(string), typeof(int[]),
                typeof(ArrayList), typeof(Array)};

            return types.Contains(o.GetType()) || (o is IList
                   && o.GetType().IsGenericType
                   && o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
        }
    }

    // ReSharper disable once InconsistentNaming
}