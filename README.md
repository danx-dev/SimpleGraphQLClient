# SimpleGraphQLClient
GraphQL Client for C#


GraphQL client for GraphQL Rest


#### Usage:


```C#
using (var client = new SimpleGraphQLClient.SimpleGraphQLClient())
            {
                client.ServiceUrl = "https://example.com/rest/query";
                client.OperationName = "getElement";
                client.Parameters.Add("id",16);
                var user = client.Post<User>();
            }
```


