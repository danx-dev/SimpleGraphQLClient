using System.Collections.Generic;

namespace SimpleGraphQLClient
{
    public interface ISimpleGraphQLClient
    {
        string ServiceUrl { get; set; }
        string OperationName { get; set; }
        Dictionary<string,object> Parameters { get; set; }

        T Post<T>();
    }
}