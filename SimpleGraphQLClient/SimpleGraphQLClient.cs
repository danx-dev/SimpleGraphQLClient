using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleGraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public class SimpleGraphQLClient:ISimpleGraphQLClient,IDisposable
    {
        private HttpClient _httpClient = null;

        public SimpleGraphQLClient() { }

        public SimpleGraphQLClient(HttpClient httpClient, string url = null)
        {
            _httpClient = httpClient;
            ServiceUrl = url;
        }

        public SimpleGraphQLClient(string url)
        {
            ServiceUrl = url;
        }

        public string ServiceUrl { get; set; }
        public string OperationName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        private readonly StringBuilder _graphQlStringBuilder = new StringBuilder();

        public T Post<T>()
        {
            if (string.IsNullOrWhiteSpace(this.OperationName))
            {
                throw new ApplicationException("OperationName is required!");
            }

            if (string.IsNullOrWhiteSpace(this.ServiceUrl))
            {
                throw new ApplicationException("ServiceUrl is required!");
            }

            if(_httpClient == null)
                _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(ServiceUrl);
            Builder<T>();
            var requestBody = JsonConvert.SerializeObject(new {query = _graphQlStringBuilder.ToString()});
            var response = _httpClient.PostAsync("",
                new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;

            var content = response.Content.ReadAsStringAsync().Result;

            var jObject = JObject.Parse(content);
            if (jObject["data"] != null && jObject["data"].HasValues)
            {
                var data = jObject["data"].Children().First().First();
                return JsonConvert.DeserializeObject<T>(data.ToString());
            }

            throw new ApplicationException(content);
        }

        private void AddQueryParameter()
        {
            string pad = new string(' ', 0);
            _graphQlStringBuilder.Append(pad + $"{OperationName}");
            if (Parameters != null && Parameters.Any())
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
                        if (list.First() is string)
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

        private bool IsList(object o)
        {

            if (o == null) return false;
            Type[] types = { typeof(String), typeof(int[]),
                typeof(ArrayList), typeof(Array)};

            if(types.Contains(o.GetType()) ||(o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
            {
                return true;
            }
            return false;
        }

        private void Builder<T>()
        {
            _graphQlStringBuilder.Append("query {");
            AddQueryParameter();
            _graphQlStringBuilder.Append(typeof(T).OutputParameters().ToLower());
            _graphQlStringBuilder.Append("}");
        }

        public void Dispose()
        {
            OperationName = null;
            Parameters = null;
            _httpClient = null;
        }
    }


    // ReSharper disable once InconsistentNaming
}
